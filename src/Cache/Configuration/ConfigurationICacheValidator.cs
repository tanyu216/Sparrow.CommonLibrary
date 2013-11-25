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
            return type.GetInterfaces().Any(x => x == typeof(ICache));
        }
        public override void Validate(object value)
        {
        }
    }
}
