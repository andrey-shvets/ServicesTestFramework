using ServicesTestFramework.WebAppTools.Exceptions;

namespace ServicesTestFramework.WebAppTools.Tests.Common;

public class ExceptionInterceptorTests
{
    private Exception ExpectedException { get; } = new CustomTestException($"message-{DateTimeOffset.Now.Ticks}", "additional data");

    [Before(Test)]
    public void TestSetup() => ExceptionInterceptor.InitializeExceptionCapture();

    [After(Test)]
    public async Task TestTeardown()
    {
        await Assert.That(ExceptionInterceptor.LastCapturedException).IsEqualTo(ExpectedException);
    }

    [Test]
    public void ExceptionInterceptor_StoresExceptionThrownByTest()
    {
        try
        {
            throw ExpectedException;
        }
        catch
        {
        }

        ExceptionInterceptor.LastCapturedException.Should().Be(ExpectedException);
    }

    [Test]
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
        catch
        {
        }
    }

    public class CustomTestException : Exception
    {
        public string AdditionalData { get; }

        public CustomTestException(string message, string additionalData) : base(message)
            => AdditionalData = additionalData;
    }
}
