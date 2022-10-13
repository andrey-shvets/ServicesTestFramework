using Ductus.FluentDocker.Commands;

namespace ServicesTestFramework.DatabaseContainers.Docker;

public static class DockerTools
{
    /// <summary>
    /// Stops and removes container with the name specified by <paramref name="containerName"/> if it exists.
    /// </summary>
    /// <param name="containerName">Container name.</param>
    public static void RemoveContainerIfExists(string containerName)
    {
        var dockerHost = DockerHostTools.DiscoverDockerHost();

        if (dockerHost is null)
            return;

        var containers = dockerHost!.Host.InspectContainers(dockerHost.Certificates).Data;
        var containerInfo = containers.FirstOrDefault(c => string.Equals(c.Name, containerName, StringComparison.InvariantCultureIgnoreCase));

        if (containerInfo is null)
            return;

        dockerHost.Host.Stop(containerInfo.Id, certificates: dockerHost.Certificates);
        dockerHost.Host.RemoveContainer(containerInfo.Id, certificates: dockerHost.Certificates);
    }

    /// <summary>
    /// Checks if container exists.
    /// </summary>
    /// <param name="containerName">Container name.</param>
    /// <returns>Returns true if container specified by <paramref name="containerName"/> exists (in any state), and if there is no container with such name - false.</returns>
    public static bool ContainerExists(string containerName)
    {
        var container = DockerHostTools.GetContainer(containerName);

        return container is not null;
    }

    /// <summary>
    /// Checks if container exists and running, stopped containers considered not reusable too.
    /// </summary>
    /// <param name="containerName">Container name.</param>
    /// <returns>Returns true if container specified by <paramref name="containerName"/> is running.</returns>
    public static bool ContainerIsReusable(string containerName)
    {
        var container = DockerHostTools.GetContainer(containerName);

        return container is not null && container.State.Running;
    }

    public static int GetMysqlPortForContainer(string containerName)
    {
        var pattern = $"^{containerName}$";

        return GetMysqlPortForContainerByRegex(pattern);
    }

    public static int GetMysqlPortForContainerByRegex(string pattern)
    {
        var containerInfo = DockerHostTools.GetContainerByRegex(pattern);

        if (containerInfo is null)
            throw new InvalidOperationException($"Couldn't find container with the name like '{pattern}', please check if it is running");

        var mysqlPortKey = "3306/tcp";

        if (!containerInfo.NetworkSettings.Ports.ContainsKey(mysqlPortKey))
            throw new InvalidOperationException($"Couldn't find open '{mysqlPortKey}' port in the container '{containerInfo.Name}', please check if mysql is running in the container.");

        var port = containerInfo!.NetworkSettings.Ports[mysqlPortKey].First().Port;

        return port;
    }
}