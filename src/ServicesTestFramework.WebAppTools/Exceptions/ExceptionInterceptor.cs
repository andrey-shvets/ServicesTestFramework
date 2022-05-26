using System;
using System.Threading;

namespace ServicesTestFramework.WebAppTools.Exceptions
{
    public static class ExceptionInterceptor
    {
        private static ThreadLocal<Exception> CurrentTestExceptionHandler { get; } = new ThreadLocal<Exception>();
        public static Exception LastCapturedException => CurrentTestExceptionHandler.Value;

        public static void InitializeExceptionCapture()
        {
            AppDomain.CurrentDomain.FirstChanceException += (_, e) =>
            {
                CurrentTestExceptionHandler.Value = e.Exception;
            };
        }
    }
}
