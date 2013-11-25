using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Weblog.Collect;
using Sparrow.CommonLibrary.Weblog.Writer;

namespace Sparrow.CommonLibrary.Weblog.Configuration
{
    public class ConfigurationICollecterValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(ICollecter));
        }
        public override void Validate(object value)
        {
        }
    }
    public class ConfigurationIWeblogWriterValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IWeblogWriter));
        }
        public override void Validate(object value)
        {
        }
    }
}
