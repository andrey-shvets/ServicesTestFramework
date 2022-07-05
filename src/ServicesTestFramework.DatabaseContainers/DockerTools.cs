using System;
using System.Linq;
using System.Text.RegularExpressions;
using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Model.Containers;
using Ductus.FluentDocker.Services;

namespace ServicesTestFramework.DatabaseContainers
{
    public static class DockerTools
    {
        public static void StopContainerIfRunning(string containerName)
        {
            var dockerHost = DiscoverDockerHost();

            if (dockerHost is null)
                return;

            var containers = dockerHost!.Host.InspectContainers(dockerHost.Certificates).Data;
            var containerInfo = containers.FirstOrDefault(c => string.Equals(c.Name, containerName, StringComparison.InvariantCultureIgnoreCase));

            if (containerInfo is null)
                return;

            dockerHost.Host.Stop(containerInfo.Id, certificates: dockerHost.Certificates);
            dockerHost.Host.RemoveContainer(containerInfo.Id, certificates: dockerHost.Certificates);
        }

        public static bool ContainerExists(string containerName)
        {
            var pattern = $"^{containerName}$";
            var container = GetContainerByRegex(pattern);

            return container is not null;
        }

        public static int GetMysqlPortForContainer(string containerName)
        {
            var pattern = $"^{containerName}$";

            return GetMysqlPortForContainerByRegex(pattern);
        }

        public static int GetMysqlPortForContainerByRegex(string pattern)
        {
            var containerInfo = GetContainerByRegex(pattern);

            if (containerInfo is null)
                throw new InvalidOperationException($"Couldn't find container with the name like '{pattern}', please check if it is running");

            var mysqlPortKey = "3306/tcp";

            if (!containerInfo.NetworkSettings.Ports.ContainsKey(mysqlPortKey))
                throw new InvalidOperationException($"Couldn't find open '{mysqlPortKey}' port in the container '{containerInfo.Name}', please check if mysql is running in the container.");

            var port = containerInfo!.NetworkSettings.Ports[mysqlPortKey].First().Port;

            return port;
        }

        private static Container GetContainerByRegex(string pattern)
        {
            var dockerHost = DiscoverDockerHost();

            if (dockerHost is null)
                return null;

            var containers = dockerHost!.Host.InspectContainers(dockerHost.Certificates).Data;
            var containerInfo = containers.FirstOrDefault(c => Regex.IsMatch(c.Name, pattern, RegexOptions.IgnoreCase));

            return containerInfo;
        }

        private static IHostService DiscoverDockerHost()
        {
            var hosts = new Hosts().Discover();
            var dockerHost = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

            return dockerHost;
        }
    }
}
