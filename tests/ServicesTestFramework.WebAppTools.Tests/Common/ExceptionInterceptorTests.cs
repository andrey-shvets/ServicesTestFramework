using System;
using FluentAssertions;
using ServicesTestFramework.WebAppTools.Exceptions;
using Xunit;

namespace ServicesTestFramework.WebAppTools.Tests.Common
{
    public class ExceptionInterceptorTests : IDisposable
    {
        private Exception ExpectedException { get; } = new CustomTestException($"message-{DateTimeOffset.Now.Ticks}", "additional data");

        public ExceptionInterceptorTests()
        {
            ExceptionInterceptor.InitializeExceptionCapture();
        }

        [Fact]
        public void ExceptionInterceptor_StoresExceptionThrownByTest()
        {
            try
            {
                throw ExpectedException;
            }
            catch
            {
                ExceptionInterceptor.LastCapturedException.Should().Be(ExpectedException);
            }
        }

        public void Dispose()
        {
            ExceptionInterceptor.LastCapturedException.Should().Be(ExpectedException);
        }

        public class CustomTestException : Exception
        {
            public string AdditionalData { get; }

            public CustomTestException(string message, string additionalData) : base(message)
            {
                AdditionalData = additionalData;
            }
        }
    }
}
