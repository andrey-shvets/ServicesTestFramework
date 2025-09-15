using RestEase;
using ServicesTestFramework.WebAppTools.Exceptions;

namespace ServicesTestFramework.WebAppTools.Tests;

public abstract class BaseTest
{
    [Before(Assembly)]
    public static void AssemblySetup() => ExceptionInterceptor.InitializeExceptionCapture();

    [After(Test)]
    public async Task TestTeardown()
    {
        if (ExceptionInterceptor.LastCapturedException is ApiException apiException && !string.IsNullOrWhiteSpace(apiException.Content))
            Console.WriteLine($"ApiException content: {apiException.Content}");

        await Task.FromResult(0);
    }
}
