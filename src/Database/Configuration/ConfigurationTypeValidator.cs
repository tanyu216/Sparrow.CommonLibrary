using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Database.DbCommon;
using Sparrow.CommonLibrary.Database.SqlBuilder;

namespace Sparrow.CommonLibrary.Database.Configuration
{
    public class ConfigurationICommandExecuterTypeValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(ICommandExecuter));
        }
        public override void Validate(object value)
        {
        }
    }
    public class ConfigurationImporterTypeValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(Importer));
        }
        public override void Validate(object value)
        {
        }
    }
    public class ConfigurationISqlBuilderTypeValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(ISqlBuilder));
        }
        public override void Validate(object value)
        {
        }
    }
}
