using IdGen;

namespace Application.Infrastructure.Services;

public interface ISnowflakeService
{
    public ulong GenerateId();
}

public class SnowflakeService : ISnowflakeService
{
    private readonly IdGenerator _generator;

    public SnowflakeService()
    {
        var epoch = new DateTime(2025, 10, 18, 0, 0, 0, DateTimeKind.Utc);
        var structure = new IdStructure(42, 10, 11);
        var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));
        _generator = new IdGenerator(0, options);
    }

    public ulong GenerateId()
    {
        return (ulong)_generator.CreateId();
    }
}

