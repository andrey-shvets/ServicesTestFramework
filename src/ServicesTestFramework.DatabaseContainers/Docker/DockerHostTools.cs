using System.Linq;
using System.Text.RegularExpressions;
using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Model.Containers;
using Ductus.FluentDocker.Services;

namespace ServicesTestFramework.DatabaseContainers.Docker;

internal static class DockerHostTools
{
    public static Container GetContainer(string containerName)
    {
        var pattern = $"^{containerName}$";
        var containerInfo = GetContainerByRegex(pattern);

        return containerInfo;
    }

    public static Container GetContainerByRegex(string pattern)
    {
        var dockerHost = DiscoverDockerHost();

        if (dockerHost is null)
            return null;

        var containers = dockerHost!.Host.InspectContainers(dockerHost.Certificates).Data;
        var containerInfo = containers.FirstOrDefault(c => Regex.IsMatch(c.Name, pattern, RegexOptions.IgnoreCase));

        return containerInfo;
    }

    public static void StopContainer(string containerName)
    {
        var containerInfo = GetContainer(containerName);
        var dockerHost = DiscoverDockerHost();

        dockerHost.Host.Stop(containerInfo.Id, certificates: dockerHost.Certificates);
    }

    public static IHostService DiscoverDockerHost()
    {
        var hosts = new Hosts().Discover();
        var dockerHost = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

        return dockerHost;
    }
}