# Benchmark stability updates

This patch reduces timing variance without changing ForgeORM runtime behavior.

Changes:

- Increased BenchmarkDotNet warmup count from 3 to 10.
- Increased BenchmarkDotNet iteration count from 10 to 30.
- Added Min, Max, Median and Rank columns so DB benchmark noise is easier to interpret.
- Warmed Dapper, EF Core and ForgeORM paths in `GlobalSetup` before measurement.
- Forced GC after warmup to keep measured allocations focused on benchmark methods.
- Fixed `ForgeORM_Query_By_Id` return type to `Task<Order?>`.

These changes do not modify hot ORM code. They only make benchmark measurements more stable and fair.
