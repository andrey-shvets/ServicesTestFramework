namespace ServicesTestFramework.WebAppTools.Exceptions;

public static class ExceptionInterceptor
{
    private static ThreadLocal<Exception> CurrentThreadTestExceptionHandler { get; } = new ThreadLocal<Exception>(trackAllValues: true);
    public static Exception LastCapturedException => CurrentThreadTestExceptionHandler.Value ??
                                                     CurrentThreadTestExceptionHandler.Values.FirstOrDefault(v => v is not null);

    public static void InitializeExceptionCapture()
    {
        AppDomain.CurrentDomain.FirstChanceException += (_, e) =>
        {
            if (Equals(CurrentThreadTestExceptionHandler.Value, e.Exception))
                return;

            CurrentThreadTestExceptionHandler.Value = e.Exception;
        };
    }
}
