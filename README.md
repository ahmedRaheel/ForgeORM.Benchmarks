# ForgeORM Benchmarks

Separate BenchmarkDotNet solution for testing ForgeORM against Dapper and EF Core, plus ForgeORM expression-style versus SQL-style benchmarks.

## What is included

- `QueryByIdBenchmarks`
- `SearchAndPagingBenchmarks`
- `InsertBenchmarks`
- `ForgeExpressionVsSqlBenchmarks`
- SQL Server schema and seed script: `sql/ForgeOrmBenchmarks.SchemaAndData.sql`
- Matching models: `Customer`, `Category`, `Product`, `Order`, `OrderItem`, `Payment`
- ForgeORM adapter classes:
  - `ForgeOrderReader`
  - `ForgeMethodReader`
  - `ForgeDbContextFactory`

## Important ForgeORM adapter point

Your current context constructor is:

```csharp
public ForgeDbContext(
    string connectionString,
    IForgeDatabaseProvider provider,
    IForgeEntityMetadataResolver metadata,
    IForgeQueryAnalyzer analyzer)
    : base(connectionString, provider, metadata, analyzer)
{
}
```

So this solution creates it in `Forge/ForgeDbContextFactory.cs`:

```csharp
return new ForgeDbContext(
    settings.ConnectionString,
    provider,
    metadata,
    analyzer);
```

## Configuration

Edit `src/ForgeORM.Benchmarks/appsettings.json`:

```json
{
  "Benchmark": {
    "ConnectionString": "Server=localhost;Database=ForgeOrmBenchmarks;Trusted_Connection=True;TrustServerCertificate=True;",
    "SeedCustomers": 1000,
    "SeedOrdersPerCustomer": 20,
    "QueryCustomerId": 10,
    "QueryOrderId": 100
  }
}
```

The settings loader intentionally avoids `.Get<BenchmarkSettings>()`; it reads values manually, so it does not depend on binder extension usage.

## Run database script

Open SQL Server Management Studio or Azure Data Studio and run:

```text
sql/ForgeOrmBenchmarks.SchemaAndData.sql
```

## Run benchmarks

Run from terminal, not Visual Studio debugger:

```bash
dotnet run -c Release --project src/ForgeORM.Benchmarks/ForgeORM.Benchmarks.csproj
```

## Expression vs SQL benchmark coverage

`ForgeExpressionVsSqlBenchmarks` covers:

- `Where`
- `OrderBy`
- `OrderByDescending`
- `Skip`
- `Take`
- `FirstAsync`
- `FirstOrDefaultAsync`
- `SingleAsync`
- `SingleOrDefaultAsync`
- `AnyAsync`
- `CountAsync`
- `SumAsync`
- `AverageAsync`
- `MinAsync`
- `MaxAsync`
- SQL `GroupBy` + `Having`
- SQL join/projection
- SQL insert/update/delete helper methods in `ForgeMethodReader`

If your real ForgeORM method signatures are slightly different, update only `Forge/ForgeMethodReader.cs`. Keep benchmark classes stable.
