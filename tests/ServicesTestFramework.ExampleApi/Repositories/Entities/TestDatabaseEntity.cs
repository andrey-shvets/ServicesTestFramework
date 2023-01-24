namespace ServicesTestFramework.ExampleApi.Repositories.Entities;

public class TestDatabaseEntity
{
    public TestDatabaseEntity(Guid id, string name, int intData)
    {
        Id = id;
        Name = name;
        IntData = intData;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int IntData { get; private set; }
}
