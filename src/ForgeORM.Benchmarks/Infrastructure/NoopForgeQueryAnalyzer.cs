using ForgeORM.Abstractions;

namespace ForgeORM.Benchmarks.Infrastructure;

public sealed class NoopForgeQueryAnalyzer : IForgeQueryAnalyzer
{
    public ForgeQueryAnalysis Analyze(string sql) => new();
}
