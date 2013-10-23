using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.Linq.Expressions;
using System.CodeDom.Compiler;
using System.Threading;
using System.Reflection;
using Sparrow.CommonLibrary.Data.Mapper.Metadata;
using Sparrow.CommonLibrary.Data.Database;
using Sparrow.CommonLibrary.Data.Mapper;

namespace Sparrow.CommonLibrary.Data.Entity
{
    /// <summary>
    /// 实例对象生成器，依据<typeparamref name="T"/>生成<typeparamref name="T"/>类型的子类，子类在继承 <typeparamref name="T"/>实现覆盖它属性的同时还实现<see cref="IMapper"/>接口。
    /// </summary>
    public class EntityBuilder
    {

        private static Func<EntityBuilder> _creater;
        /// <summary>
        /// 设置<see cref="Sparrow.CommonLibrary.Data.Entity.EntityBuilder"/>新的实例化方法
        /// </summary>
        /// <param name="creater"></param>
        public static void ResetCreater(Func<EntityBuilder> creater)
        {
            _creater = creater;
        }

        /// <summary>
        /// 生成一个实体的继承类，重写实体中属性成员，并继承IEntity接口。
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="metaInfo"></param>
        /// <returns></returns>
        public static Type BuilderEntityClass(Type baseType, IMetaInfo metaInfo)
        {
            if (baseType == null)
                throw new ArgumentNullException("baseType");
            if (metaInfo == null)
                throw new ArgumentNullException("metaInfo");
            if (_creater != null)
                return _creater().BuildEntityType(baseType, metaInfo);
            return new EntityBuilder().BuildEntityType(baseType, metaInfo);
        }

        private static int _next = -1;
        //
        private CodeNamespace _codeNamespace;
        private CodeTypeDeclaration _codeTypeDeclaration;
        private Type _baseType;
        private IMetaInfo _metaInfo;

        protected EntityBuilder() { }

        /// <summary>
        /// 生成一个继承自实体的类
        /// </summary>
        /// <returns></returns>
        public virtual Type BuildEntityType(Type baseType, IMetaInfo metaInfo)
        {
            if (!(metaInfo is IMapper))
                throw new ArgumentException(string.Format("metaInfo未能实现{0}", typeof(IMapper).FullName));

            _baseType = baseType;
            _metaInfo = metaInfo;
            //
            var index = Interlocked.Increment(ref _next).ToString();//加上一个全局的标识，防止类名重名。
            _codeTypeDeclaration = new CodeTypeDeclaration(string.Concat(_baseType.Name, "_Entity_", index));
            _codeTypeDeclaration.Attributes = MemberAttributes.Public;
            //
            _codeNamespace = new CodeNamespace(_baseType.Namespace);
            _codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
            _codeNamespace.Imports.Add(new CodeNamespaceImport(_baseType.Namespace));
            _codeNamespace.Types.Add(_codeTypeDeclaration);
            //
            BuildEntity();
            // 编译
            return Compile();
        }

        private Type Compile()
        {

            CodeCompileUnit unit = new CodeCompileUnit();
            unit.Namespaces.Add(_codeNamespace);

            // 编译的选项参数
            CompilerParameters option = new CompilerParameters();
            option.GenerateExecutable = false;
#if DEBUG
            option.GenerateInMemory = false;
            option.IncludeDebugInformation = true;
#else
            option.GenerateInMemory = true;
            option.IncludeDebugInformation = false;
#endif
            option.ReferencedAssemblies.Add("System.dll");
            option.ReferencedAssemblies.Add("System.Core.dll");
            option.ReferencedAssemblies.Add(_baseType.Assembly.Location);
            option.ReferencedAssemblies.Add(typeof(EntityBuilder).Assembly.Location);
#if DEBUG
            string code = GenerateCode(unit);
#endif
            CompilerResults result = CodeDomProvider.CreateProvider("C#").CompileAssemblyFromDom(option, unit);
            foreach (CompilerError error in result.Errors)
            {
                if (!error.IsWarning)
                {
                    throw new MapperException(string.Format("代码生成失败。\n\t错误号：{1}。\n\t错误消息：\n\t{2}。\n\t源码：\n\t{3}", _baseType.FullName, error.ErrorNumber, error.ErrorText, GenerateCode(unit)));
                }
            }
            return result.CompiledAssembly.GetType(string.Concat(_codeNamespace.Name, ".", _codeTypeDeclaration.Name));
        }

        /// <summary>
        /// 生成C#语法代码(文本)
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private string GenerateCode(CodeCompileUnit unit)
        {
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                CodeDomProvider.CreateProvider("C#").GenerateCodeFromCompileUnit(unit, sw, null);
                return sw.ToString();
            }
        }

        private void BuildEntity()
        {
            _codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference(_baseType));
            _codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference(typeof(IEntity)));

            BuildIEntityMembers();
            BuildOverrideProperties();
        }

        private void BuildIEntityMembers()
        {
            List<CodeTypeMember> members = new List<CodeTypeMember>();

            // 
            var thisRef = new CodeThisReferenceExpression();
            var propertySetValue = new CodePropertySetValueReferenceExpression();

            #region DataState OperationState{get;set;}

            CodeMemberField fieldOperationState = new CodeMemberField(new CodeTypeReference(typeof(DataState)), "_optState");
            CodeMemberProperty ptyOperationState = new CodeMemberProperty();
            ptyOperationState.Name = "OperationState";
            ptyOperationState.Type = new CodeTypeReference(typeof(DataState));
            ptyOperationState.PrivateImplementationType = new CodeTypeReference(typeof(IEntity));
            // code: return _OperationState;
            ptyOperationState.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(thisRef, fieldOperationState.Name)));
            // code: _OperationState = value;
            ptyOperationState.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisRef, fieldOperationState.Name), propertySetValue));
            members.Add(fieldOperationState);
            members.Add(ptyOperationState);

            #endregion

            #region FieldFlag _indexer

            CodeMemberField fieldIndexer = new CodeMemberField(new CodeTypeReference(typeof(FieldFlag)), "_indexer");
            _codeTypeDeclaration.Members.Add(fieldIndexer);
            CodeFieldReferenceExpression _indexer = new CodeFieldReferenceExpression(thisRef, fieldIndexer.Name);

            #endregion

            #region bool IsSetted(int index);

            CodeMemberMethod methodIsSetted = new CodeMemberMethod();
            methodIsSetted.Name = "IsSetted";
            methodIsSetted.PrivateImplementationType = new CodeTypeReference(typeof(IEntity));
            methodIsSetted.ReturnType = new CodeTypeReference(typeof(bool));
            methodIsSetted.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "index"));
            methodIsSetted.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(_indexer, "HasMarked", new CodeVariableReferenceExpression("index"))));

            #endregion

            #region bool AnySetted();

            CodeMemberMethod methodAnySetted = new CodeMemberMethod();
            methodAnySetted.Name = "AnySetted";
            methodAnySetted.PrivateImplementationType = new CodeTypeReference(typeof(IEntity));
            methodAnySetted.ReturnType = new CodeTypeReference(typeof(bool));
            methodAnySetted.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "index"));
            methodAnySetted.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(_indexer, "HasMarked")));

            #endregion

            #region Type EntityType  { get; }

            CodeMemberProperty ptyEntityTypeName = new CodeMemberProperty();
            ptyEntityTypeName.Name = "EntityType";
            ptyEntityTypeName.Type = new CodeTypeReference(typeof(Type));
            ptyEntityTypeName.PrivateImplementationType = new CodeTypeReference(typeof(IEntity));
            // code: return _entityType;
            ptyEntityTypeName.GetStatements.Add(new CodeMethodReturnStatement(new CodeTypeOfExpression(_baseType)));
            members.Add(ptyEntityTypeName);

            #endregion

            #region void Importing();

            CodeMemberField importingField = new CodeMemberField(typeof(bool), "_Importing");
            members.Add(importingField);

            CodeMemberMethod methodImporting = new CodeMemberMethod();
            methodImporting.Name = "Importing";
            methodImporting.PrivateImplementationType = new CodeTypeReference(typeof(IEntity));
            methodImporting.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisRef, importingField.Name), new CodePrimitiveExpression(true)));
            methodImporting.Statements.Add(new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                    new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), fieldIndexer.Name), "Clean")));
            //
            members.Add(methodImporting);

            #endregion

            #region void Imported();

            CodeMemberMethod methodImported = new CodeMemberMethod();
            methodImported.Name = "Imported";
            methodImported.PrivateImplementationType = new CodeTypeReference(typeof(IEntity));
            methodImported.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisRef, importingField.Name), new CodePrimitiveExpression(false)));
            //
            members.Add(methodImported);

            #endregion

            #region .ctor


            CodeConstructor defaultCtor = new CodeConstructor();
            defaultCtor.Attributes = MemberAttributes.Public;
            defaultCtor.Statements.Add(new CodeAssignStatement(
                                            new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldIndexer.Name),
                                            new CodeObjectCreateExpression(typeof(FieldFlag), new CodePrimitiveExpression(_metaInfo.FieldCount))
                                        ));
            members.Add(defaultCtor);

            #endregion

            //
            _codeTypeDeclaration.Members.AddRange(members.ToArray());
        }

        private void BuildOverrideProperties()
        {
            IList<CodeTypeMember> members = new List<CodeTypeMember>(_metaInfo.FieldCount);

            // 重写基类（实体对象）的属性。
            foreach (var field in _metaInfo.GetFields())
            {
                var propertyInfo = field.PropertyInfo;
                string propertyName = propertyInfo.Name;
                if (propertyInfo.CanWrite == false || propertyInfo.CanRead == false)
                    throw new ArgumentException(string.Format("类型[{0}]中的属性[{1}]必须支持读写访问器。", propertyInfo.PropertyType.FullName, propertyName));
                if (!propertyInfo.GetSetMethod().IsVirtual)
                    throw new ArgumentException(string.Format("类型[{0}]中的属性[{1}]必须标记为[virtual]。", propertyInfo.PropertyType.FullName, propertyName));

                //
                var property = new CodeMemberProperty();
                property.Name = propertyName;
                property.Type = new CodeTypeReference(propertyInfo.PropertyType);
                property.Attributes = MemberAttributes.Override | MemberAttributes.Public;

                // 向属性的get方法增加代码
                // code: return base.PropertyName;
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeBaseReferenceExpression(), propertyName)));
                // 向属性的set方法增加代码
                // code:
                // if (_Importing){
                //      base.PropertyName == value;
                //      return;
                // }
                // if (base.PropertyName == value){
                //      if(value == default(type))
                //          this._index.SetValue(index of field);
                //      return;
                // }
                // this._index.SetValue(index of field);
                // base.PropertyName = value;
                property.SetStatements.Add(
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_Importing"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)),
                        new CodeAssignStatement(
                            new CodeFieldReferenceExpression(new CodeBaseReferenceExpression(), propertyName),
                            new CodePropertySetValueReferenceExpression()
                        ),
                        new CodeMethodReturnStatement()
                    )
                );
                var methodIndexerCall = new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_indexer"),
                        "SetValue"),
                        new CodePrimitiveExpression(((IMapper)field.MetaInfo).IndexOf(field.FieldName)));
                var condtion1 = new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodePropertySetValueReferenceExpression(),
                        CodeBinaryOperatorType.IdentityEquality,
                        new CodeDefaultValueExpression(property.Type))
                    );
                condtion1.TrueStatements.Add(methodIndexerCall);

                property.SetStatements.Add(
                    new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeBaseReferenceExpression(), propertyName), CodeBinaryOperatorType.ValueEquality, new CodePropertySetValueReferenceExpression()),
                        condtion1,
                        new CodeMethodReturnStatement()
                    )
                );
                property.SetStatements.Add(methodIndexerCall);
                property.SetStatements.Add(new CodeAssignStatement(
                    new CodeFieldReferenceExpression(new CodeBaseReferenceExpression(), propertyName),
                    new CodePropertySetValueReferenceExpression()));

                //
                members.Add(property);
            }
            //
            _codeTypeDeclaration.Members.AddRange(members.ToArray());
        }
    }
}
