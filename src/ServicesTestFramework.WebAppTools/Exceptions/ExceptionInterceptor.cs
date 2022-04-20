using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ServicesTestFramework.WebAppTools.Exceptions
{
    public static class ExceptionInterceptor
    {
        private static ThreadLocal<Exception> CurrentTestExceptionHandler { get; } = new ThreadLocal<Exception>();
        public static Exception CurrentTestException => CurrentTestExceptionHandler.Value;

        [ModuleInitializer]
        public static void InitializeExceptionCapture()
        {
            AppDomain.CurrentDomain.FirstChanceException += (_, e) =>
            {
                CurrentTestExceptionHandler.Value = e.Exception;
            };
        }
    }
}
