using BenchmarkDotNet.Running;
using ForgeORM.Benchmarks.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(CoreScenarioBenchmarks).Assembly).Run(args);
