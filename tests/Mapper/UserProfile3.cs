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

        public static IMapper<UserProfile3> GetIMapper()
        {
            return new DataMapper<UserProfile3>()
                .AppendField(x => x.Id, "Id").MakeKey().MakeIncrement()
                .AppendField(x => x.Name, "Name")
                .AppendField(x => x.Sex, "Sex")
                .AppendField(x => x.Email, "Email")
                .AppendField(x => x.FixPhone, "FixPhone")
                .Complete();

        }
    }
}
