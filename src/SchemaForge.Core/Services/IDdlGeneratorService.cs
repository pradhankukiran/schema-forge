using SchemaForge.Core.Models;

namespace SchemaForge.Core.Services;

public interface IDdlGeneratorService
{
    SqlDialect Dialect { get; }
    string GenerateFullDdl(SchemaDocument schema);
    string GenerateCreateTable(TableDefinition table, SchemaDocument schema);
    string GenerateCreateIndex(IndexDefinition index, TableDefinition table);
    string GenerateDropTable(string tableName);
}
