using System;
using System.Web;

namespace Sparrow.CommonLibrary.Utility.Extenssions
{
    public static class WebExts
    {
        public static string GetClientIp(this HttpRequest request)
        {
            if (request == null)
                return String.Empty;
            var clientIp = request.Headers.Get("HTTP_X_FORWARDED_FOR");
            if (String.IsNullOrEmpty(clientIp))
                clientIp = request.Headers.Get("X-Forwarded-For");
            if (String.IsNullOrEmpty(clientIp))
                clientIp = request.UserHostAddress;
            return clientIp ?? String.Empty;
        }
    }
}
