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
            return type == typeof(Type);
        }
        public override void Validate(object value)
        {
            if (!((Type)value).GetInterfaces().Any(x => x == typeof(ICollecter)))
                throw new ConfigurationErrorsException("type类型未实现接口" + typeof(ICollecter).FullName);
        }
    }
    public class ConfigurationIWeblogWriterValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type)
        {
            return type == typeof(Type);
        }
        public override void Validate(object value)
        {
            if (!((Type)value).GetInterfaces().Any(x => x == typeof(IWeblogWriter)))
                throw new ConfigurationErrorsException("type类型未实现接口" + typeof(IWeblogWriter).FullName);
        }
    }
}
