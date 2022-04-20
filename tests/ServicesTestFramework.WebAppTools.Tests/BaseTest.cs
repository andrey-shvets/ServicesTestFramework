using System;
using RestEase;
using ServicesTestFramework.WebAppTools.Exceptions;
using Xunit.Abstractions;

namespace ServicesTestFramework.WebAppTools.Tests
{
    public abstract class BaseTest : IDisposable
    {
        protected ITestOutputHelper OutputHelper { get; }

        protected BaseTest(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        public virtual void Dispose()
        {
            if (ExceptionInterceptor.CurrentTestException is ApiException apiException &&
                !string.IsNullOrWhiteSpace(apiException.Content))
                OutputHelper?.WriteLine($"ApiException content: {apiException.Content}");
        }
    }
}
