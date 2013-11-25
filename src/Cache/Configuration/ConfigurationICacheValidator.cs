using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Cache.Configuration
{
    public class ConfigurationICacheValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type)
        {
            return type == typeof(Type);
        }
        public override void Validate(object value)
        {
            if (!((Type)value).GetInterfaces().Any(x => x == typeof(ICache)))
                throw new ConfigurationErrorsException("type类型未实现接口" + typeof(ICache).FullName);
        }
    }
}
