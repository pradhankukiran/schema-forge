using SchemaForge.Core.Models;

namespace SchemaForge.Core.Services;

public interface ISqlExecutionService
{
    bool IsInitialized { get; }
    Task InitializeAsync();
    Task<QueryResult> ExecuteAsync(string sql);
    Task<List<QueryResult>> ExecuteBatchAsync(string sql);
    Task ResetAsync();
}
