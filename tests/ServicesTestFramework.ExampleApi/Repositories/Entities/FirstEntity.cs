using System;

namespace ServicesTestFramework.ExampleApi.Repositories.Entities;

public class FirstEntity
{
    public uint Id { get; init; }

    public string Name { get; init; }

    public DateTime SomeDate { get; init; }

    public bool Active { get; init; }
}