using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Logging.Filter;
using Sparrow.CommonLibrary.Logging.Writer;

namespace Sparrow.CommonLibrary.Logging.Configuration
{
    public class ConfigurationILogFilterValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(ILogFilter));
        }
        public override void Validate(object value)
        {
        }
    }
    public class ConfigurationILogWriterValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(ILogWriter));
        }
        public override void Validate(object value)
        {
        }
    }
}
