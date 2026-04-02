using SchemaForge.Core.Models;
using SchemaForge.Core.Services;

namespace SchemaForge.Core.DdlGeneration;

public class DdlGeneratorFactory
{
    private readonly Dictionary<SqlDialect, IDdlGeneratorService> _generators = [];

    public DdlGeneratorFactory(IEnumerable<IDdlGeneratorService> generators)
    {
        foreach (var gen in generators)
            _generators[gen.Dialect] = gen;
    }

    public IDdlGeneratorService GetGenerator(SqlDialect dialect) =>
        _generators.TryGetValue(dialect, out var generator)
            ? generator
            : throw new NotSupportedException($"DDL generator for {dialect} is not registered.");

    public IEnumerable<SqlDialect> SupportedDialects => _generators.Keys;
}
