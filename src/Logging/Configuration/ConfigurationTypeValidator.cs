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
            return type == typeof(Type);
        }
        public override void Validate(object value)
        {
            if (!((Type)value).GetInterfaces().Any(x => x == typeof(ILogFilter)))
                throw new ConfigurationErrorsException("type类型未实现接口" + typeof(ILogFilter).FullName);
        }
    }
    public class ConfigurationILogWriterValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type)
        {
            return type == typeof(Type);
        }
        public override void Validate(object value)
        {
            if (!((Type)value).GetInterfaces().Any(x => x == typeof(ILogWriter)))
                throw new ConfigurationErrorsException("type类型未实现接口" + typeof(ILogWriter).FullName);
        }
    }
}
