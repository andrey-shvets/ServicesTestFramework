using System.Threading.Tasks;

namespace ServicesTestFramework.DatabaseContainers
{
    public static class DatabaseContainerExtensions
    {
        public static async Task StopContainer(this DatabaseContainer container)
        {
            if (container == null)
                return;

            container.Connection.Dispose();
            await container.Container.DisposeAsync().ConfigureAwait(false);
        }
    }
}
