using BenchmarkDotNet.Running;
using ForgeORM.Benchmarks.Benchmarks;

BenchmarkRunner.Run<QueryByIdBenchmarks>();
BenchmarkRunner.Run<SearchAndPagingBenchmarks>();
BenchmarkRunner.Run<InsertBenchmarks>();
BenchmarkRunner.Run<ForgeExpressionVsSqlBenchmarks>();
