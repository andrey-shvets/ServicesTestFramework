using System;
using RestEase;
using ServicesTestFramework.WebAppTools.Exceptions;
using Xunit.Abstractions;

namespace ServicesTestFramework.DatabaseContainers.Tests
{
    public abstract class BaseTest : IDisposable
    {
        protected ITestOutputHelper OutputHelper { get; }

        static BaseTest()
        {
            ExceptionInterceptor.InitializeExceptionCapture();
        }

        protected BaseTest(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        public virtual void Dispose()
        {
            if (ExceptionInterceptor.LastCapturedException is ApiException apiException &&
                !string.IsNullOrWhiteSpace(apiException.Content))
                OutputHelper?.WriteLine($"ApiException content: {apiException.Content}");
        }
    }
}
