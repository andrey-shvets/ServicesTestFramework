using FluentAssertions;
using ServicesTestFramework.WebAppTools.Exceptions;
using Xunit;

namespace ServicesTestFramework.WebAppTools.Tests.Common
{
    public class ExceptionInterceptorTests : IDisposable
    {
        private Exception ExpectedException { get; } = new CustomTestException($"message-{DateTimeOffset.Now.Ticks}", "additional data");

        public ExceptionInterceptorTests() => ExceptionInterceptor.InitializeExceptionCapture();

        [Fact]
        public void ExceptionInterceptor_StoresExceptionThrownByTest()
        {
            try
            {
                throw ExpectedException;
            }
            catch
            { }

            ExceptionInterceptor.LastCapturedException.Should().Be(ExpectedException);
        }

        [Fact]
        public async Task ExceptionInterceptor_StoresExceptionThrownByAsyncMethod()
        {
            await ExceptionThrower(ExpectedException);

            ExceptionInterceptor.LastCapturedException.Should().Be(ExpectedException);
        }

        private static async Task ExceptionThrower(Exception expectedException)
        {
            await Task.CompletedTask;

            try
            {
                throw expectedException;
            }
            catch { }
        }

        public void Dispose()
        {
            ExceptionInterceptor.LastCapturedException.Should().Be(ExpectedException);

            GC.SuppressFinalize(this);
        }

        public class CustomTestException : Exception
        {
            public string AdditionalData { get; }

            public CustomTestException(string message, string additionalData) : base(message)
                => AdditionalData = additionalData;
        }
    }
}
