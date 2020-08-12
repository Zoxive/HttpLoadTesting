using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Zoxive.HttpLoadTesting.Client.Framework.Extensions
{
    public static class HttpContextExtensions
    {
        private static bool _debuggingEnabled;

        static HttpContextExtensions()
        {
            SetDebuggingEnabled();
        }

        [Conditional("DEBUG")]
        private static void SetDebuggingEnabled()
        {
            _debuggingEnabled = true;
        }

        public static bool IsDebuggingEnabled(this HttpContext context)
        {
            return _debuggingEnabled;
        }
    }
}