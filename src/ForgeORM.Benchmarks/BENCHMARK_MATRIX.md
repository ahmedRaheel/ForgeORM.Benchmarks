# ForgeORM Benchmark Matrix

This benchmark project is frozen around the validated GetById pattern:

- `ForgeDbContext` is created once in setup and reused.
- Benchmark methods do not create `ForgeDbContext`.
- RuntimeEmit/MSIL materialization remains the only ForgeORM benchmark path.
- Dapper and ForgeORM use the same SQL text and parameters where possible.
- EF Core uses `AsNoTracking()` and projected DTOs for fairness.

## Scenarios

- Query_By_Id
- Query_First
- Query_List
- Search_Paged
- Insert_Single
- Insert_Bulk
- Update
- Delete
- Graph_Insert
- Graph_Update
- Split_Query
- Record_DTO
- Enum_Mapping
- Streaming
- Async_Streaming
- Stored_Procedure

Run all:

```powershell
dotnet run -c Release --project src/ForgeORM.Benchmarks -- --filter *
```

Run one group:

```powershell
dotnet run -c Release --project src/ForgeORM.Benchmarks -- --filter *Query_By_Id*
```
