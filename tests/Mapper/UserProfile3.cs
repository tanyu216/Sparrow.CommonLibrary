using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Mapper.Metadata;

namespace Sparrow.CommonLibrary.Test.Mapper
{
    public class UserProfile3
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual short Sex { get; set; }
        public virtual string Email { get; set; }
        public virtual string FixPhone { get; set; }

        public static IObjectAccessor<UserProfile3> GetObjectAccessor()
        {
            return ObjectAccessorExtenssions.Create<UserProfile3>()
                .AppendProperty(x => x.Id, "Id")
                .AppendProperty(x => x.Name, "Name")
                .AppendProperty(x => x.Sex, "Sex")
                .AppendProperty(x => x.Email, "Email")
                .AppendProperty(x => x.FixPhone, "FixPhone")
                .Complete();

        }
    }
}
