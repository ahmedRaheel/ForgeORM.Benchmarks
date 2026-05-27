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
```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.8457)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.300
  [Host]     : .NET 8.0.27 (8.0.2726.22922), X64 RyuJIT AVX2 [AttachedDebugger]
  Job-HEIGNX : .NET 8.0.27 (8.0.2726.22922), X64 RyuJIT AVX2

IterationCount=10  WarmupCount=3  

```
| Method                    | Take | Mean        | Error     | StdDev    | Ratio  | RatioSD | Gen0     | Gen1     | Allocated  | Alloc Ratio |
|-------------------------- |----- |------------:|----------:|----------:|-------:|--------:|---------:|---------:|-----------:|------------:|
| **Dapper_Query_By_Id**        | **10**   |    **187.5 μs** |  **10.16 μs** |   **6.05 μs** |   **1.00** |    **0.04** |   **1.7090** |        **-** |     **7.7 KB** |        **1.00** |
| EF_Core_Query_By_Id       | 10   |    254.0 μs |  11.90 μs |   7.87 μs |   1.36 |    0.06 |   4.3945 |        - |   19.82 KB |        2.57 |
| ForgeORM_Query_By_Id      | 10   |    186.2 μs |   4.22 μs |   2.51 μs |   0.99 |    0.03 |   1.7090 |        - |    7.71 KB |        1.00 |
| Dapper_Query_First        | 10   |    192.2 μs |   2.87 μs |   1.50 μs |   1.03 |    0.03 |   1.7090 |        - |    7.72 KB |        1.00 |
| EF_Core_Query_First       | 10   |    321.0 μs |  70.23 μs |  46.46 μs |   1.71 |    0.24 |   4.8828 |        - |   21.28 KB |        2.76 |
| ForgeORM_Query_First      | 10   |    207.0 μs |  22.43 μs |  13.35 μs |   1.11 |    0.08 |   1.7090 |        - |    7.73 KB |        1.00 |
| Dapper_Query_List         | 10   |    249.9 μs |  16.69 μs |  11.04 μs |   1.33 |    0.07 |   2.4414 |        - |   11.23 KB |        1.46 |
| EF_Core_Query_List        | 10   |    390.8 μs |  27.85 μs |  16.57 μs |   2.09 |    0.11 |   8.7891 |        - |   39.63 KB |        5.14 |
| ForgeORM_Query_List       | 10   |    225.5 μs |   5.54 μs |   3.66 μs |   1.20 |    0.04 |   2.4414 |        - |   10.42 KB |        1.35 |
| Dapper_Search_Paged       | 10   |    226.6 μs |   7.65 μs |   4.55 μs |   1.21 |    0.04 |   2.4414 |        - |   11.58 KB |        1.50 |
| EF_Core_Search_Paged      | 10   |    398.3 μs |  40.05 μs |  23.83 μs |   2.13 |    0.14 |   9.7656 |        - |   41.94 KB |        5.44 |
| ForgeORM_Search_Paged     | 10   |    228.5 μs |   7.66 μs |   4.56 μs |   1.22 |    0.04 |   2.9297 |        - |   12.73 KB |        1.65 |
| Dapper_Insert_Single      | 10   |    348.7 μs |  12.06 μs |   7.98 μs |   1.86 |    0.07 |   4.8828 |        - |   20.71 KB |        2.69 |
| EF_Core_Insert_Single     | 10   |    402.1 μs |  13.43 μs |   7.99 μs |   2.15 |    0.08 |   5.8594 |        - |   26.59 KB |        3.45 |
| ForgeORM_Insert_Single    | 10   |    353.0 μs |   8.31 μs |   4.35 μs |   1.88 |    0.06 |   4.8828 |        - |   20.69 KB |        2.69 |
| Dapper_Insert_Bulk        | 10   |          NA |        NA |        NA |      ? |       ? |       NA |       NA |         NA |           ? |
| EF_Core_Insert_Bulk       | 10   |    962.9 μs |  16.12 μs |  10.66 μs |   5.14 |    0.17 |  37.1094 |   3.9063 |   157.4 KB |       20.43 |
| ForgeORM_Insert_Bulk      | 10   |  3,488.6 μs | 125.79 μs |  83.20 μs |  18.62 |    0.72 |  46.8750 |        - |  205.48 KB |       26.67 |
| Dapper_Update             | 10   |    665.9 μs |  17.47 μs |   9.14 μs |   3.55 |    0.12 |   5.8594 |        - |      26 KB |        3.37 |
| EF_Core_Update            | 10   |    746.9 μs | 206.22 μs | 136.40 μs |   3.99 |    0.71 |   7.8125 |        - |    34.4 KB |        4.47 |
| ForgeORM_Update           | 10   |    678.6 μs |  12.54 μs |   8.30 μs |   3.62 |    0.12 |   5.8594 |        - |   25.98 KB |        3.37 |
| Dapper_Delete             | 10   |    699.9 μs |  17.08 μs |  10.17 μs |   3.74 |    0.13 |   5.8594 |        - |   25.68 KB |        3.33 |
| EF_Core_Delete            | 10   |  1,080.2 μs | 114.59 μs |  75.79 μs |   5.77 |    0.43 |  11.7188 |        - |   52.79 KB |        6.85 |
| ForgeORM_Delete           | 10   |    747.1 μs |  62.10 μs |  41.07 μs |   3.99 |    0.24 |   5.8594 |        - |   25.67 KB |        3.33 |
| ForgeORM_Graph_Insert     | 10   |  1,146.8 μs |  24.54 μs |  14.60 μs |   6.12 |    0.20 |  15.6250 |        - |    70.3 KB |        9.13 |
| ForgeORM_Graph_Update     | 10   |  2,466.7 μs | 213.97 μs | 127.33 μs |  13.17 |    0.76 |  31.2500 |        - |  141.57 KB |       18.38 |
| Dapper_Split_Query        | 10   |    334.3 μs |  37.19 μs |  22.13 μs |   1.78 |    0.13 |  22.4609 |   3.9063 |    92.5 KB |       12.01 |
| EF_Core_Split_Query       | 10   |    474.7 μs |  30.34 μs |  20.07 μs |   2.53 |    0.13 |   6.8359 |        - |   30.35 KB |        3.94 |
| ForgeORM_Split_Query      | 10   |    399.3 μs |  22.83 μs |  13.59 μs |   2.13 |    0.10 |   3.9063 |        - |   15.92 KB |        2.07 |
| Dapper_Record_DTO         | 10   |    199.1 μs |   2.68 μs |   1.40 μs |   1.06 |    0.03 |   1.7090 |        - |     7.7 KB |        1.00 |
| ForgeORM_Record_DTO       | 10   |    190.0 μs |   3.28 μs |   1.72 μs |   1.01 |    0.03 |   1.7090 |        - |    7.71 KB |        1.00 |
| Dapper_Enum_Mapping       | 10   |    218.0 μs |  22.80 μs |  13.57 μs |   1.16 |    0.08 |   1.4648 |        - |    6.55 KB |        0.85 |
| ForgeORM_Enum_Mapping     | 10   |    251.2 μs |  21.06 μs |  13.93 μs |   1.34 |    0.08 |   1.4648 |        - |     6.6 KB |        0.86 |
| Dapper_Streaming          | 10   |    249.3 μs |  24.88 μs |  14.81 μs |   1.33 |    0.09 |   1.7090 |        - |    7.52 KB |        0.98 |
| EF_Core_Async_Streaming   | 10   |    376.5 μs | 108.98 μs |  72.08 μs |   2.01 |    0.37 |   6.8359 |        - |    29.5 KB |        3.83 |
| ForgeORM_Async_Streaming  | 10   |    331.1 μs |  24.05 μs |  14.31 μs |   1.77 |    0.09 |   2.4414 |        - |   10.42 KB |        1.35 |
| Dapper_Stored_Procedure   | 10   |    202.1 μs |   3.70 μs |   1.93 μs |   1.08 |    0.03 |   1.7090 |        - |    7.48 KB |        0.97 |
| ForgeORM_Stored_Procedure | 10   |    221.8 μs |   9.28 μs |   5.52 μs |   1.18 |    0.05 |   1.4648 |        - |    7.71 KB |        1.00 |
|                           |      |             |           |           |        |         |          |          |            |             |
| **Dapper_Query_By_Id**        | **50**   |    **199.7 μs** |   **3.43 μs** |   **2.04 μs** |   **1.00** |    **0.01** |   **1.7090** |        **-** |     **7.7 KB** |        **1.00** |
| EF_Core_Query_By_Id       | 50   |    264.1 μs |   5.80 μs |   3.84 μs |   1.32 |    0.02 |   4.3945 |        - |   19.74 KB |        2.56 |
| ForgeORM_Query_By_Id      | 50   |    197.1 μs |   2.42 μs |   1.44 μs |   0.99 |    0.01 |   1.7090 |        - |    7.71 KB |        1.00 |
| Dapper_Query_First        | 50   |    209.4 μs |   4.79 μs |   3.17 μs |   1.05 |    0.02 |   1.7090 |        - |    7.72 KB |        1.00 |
| EF_Core_Query_First       | 50   |    280.1 μs |   7.30 μs |   4.34 μs |   1.40 |    0.02 |   4.8828 |        - |   21.17 KB |        2.75 |
| ForgeORM_Query_First      | 50   |    203.0 μs |   2.19 μs |   1.15 μs |   1.02 |    0.01 |   1.7090 |        - |    7.73 KB |        1.00 |
| Dapper_Query_List         | 50   |    258.3 μs |   4.35 μs |   2.88 μs |   1.29 |    0.02 |   3.4180 |        - |   14.71 KB |        1.91 |
| EF_Core_Query_List        | 50   |    423.8 μs |  32.82 μs |  19.53 μs |   2.12 |    0.10 |  10.7422 |        - |   43.79 KB |        5.68 |
| ForgeORM_Query_List       | 50   |    267.9 μs |   9.60 μs |   5.02 μs |   1.34 |    0.03 |   2.9297 |        - |   12.61 KB |        1.64 |
| Dapper_Search_Paged       | 50   |    263.5 μs |   8.14 μs |   4.85 μs |   1.32 |    0.03 |   3.4180 |        - |   15.06 KB |        1.96 |
| EF_Core_Search_Paged      | 50   |    418.6 μs |  13.07 μs |   6.84 μs |   2.10 |    0.04 |  10.7422 |        - |    46.3 KB |        6.01 |
| ForgeORM_Search_Paged     | 50   |    261.8 μs |   6.85 μs |   3.58 μs |   1.31 |    0.02 |   3.4180 |        - |   14.92 KB |        1.94 |
| Dapper_Insert_Single      | 50   |    364.9 μs |   5.52 μs |   3.28 μs |   1.83 |    0.02 |   4.8828 |        - |   20.71 KB |        2.69 |
| EF_Core_Insert_Single     | 50   |    431.6 μs |  23.61 μs |  14.05 μs |   2.16 |    0.07 |   5.8594 |        - |   26.59 KB |        3.45 |
| ForgeORM_Insert_Single    | 50   |    365.9 μs |   5.55 μs |   3.30 μs |   1.83 |    0.02 |   4.8828 |        - |   20.69 KB |        2.69 |
| Dapper_Insert_Bulk        | 50   |          NA |        NA |        NA |      ? |       ? |       NA |       NA |         NA |           ? |
| EF_Core_Insert_Bulk       | 50   |  4,252.8 μs | 163.14 μs | 107.90 μs |  21.29 |    0.55 | 171.8750 |  54.6875 |  744.57 KB |       96.65 |
| ForgeORM_Insert_Bulk      | 50   | 17,783.1 μs | 284.48 μs | 169.29 μs |  89.04 |    1.18 | 250.0000 |        - | 1026.59 KB |      133.25 |
| Dapper_Update             | 50   |    723.9 μs |  71.10 μs |  47.03 μs |   3.62 |    0.23 |   5.8594 |        - |      26 KB |        3.37 |
| EF_Core_Update            | 50   |    615.9 μs |   6.94 μs |   3.63 μs |   3.08 |    0.03 |   7.8125 |        - |    34.4 KB |        4.47 |
| ForgeORM_Update           | 50   |    692.2 μs |   8.67 μs |   5.16 μs |   3.47 |    0.04 |   5.8594 |        - |   25.98 KB |        3.37 |
| Dapper_Delete             | 50   |    730.9 μs |  14.00 μs |   8.33 μs |   3.66 |    0.05 |   5.8594 |        - |   25.68 KB |        3.33 |
| EF_Core_Delete            | 50   |  1,047.0 μs |  51.50 μs |  26.93 μs |   5.24 |    0.14 |  11.7188 |        - |   52.79 KB |        6.85 |
| ForgeORM_Delete           | 50   |    729.1 μs |  63.23 μs |  33.07 μs |   3.65 |    0.16 |   5.8594 |        - |   25.67 KB |        3.33 |
| ForgeORM_Graph_Insert     | 50   |  1,191.8 μs |  34.41 μs |  20.48 μs |   5.97 |    0.11 |  15.6250 |        - |    70.3 KB |        9.13 |
| ForgeORM_Graph_Update     | 50   |  2,529.0 μs |  72.09 μs |  37.71 μs |  12.66 |    0.22 |  31.2500 |        - |  141.46 KB |       18.36 |
| Dapper_Split_Query        | 50   |    333.6 μs |  15.75 μs |  10.41 μs |   1.67 |    0.05 |  22.4609 |   3.9063 |    92.5 KB |       12.01 |
| EF_Core_Split_Query       | 50   |    457.5 μs |  29.21 μs |  19.32 μs |   2.29 |    0.09 |   6.8359 |        - |   30.46 KB |        3.95 |
| ForgeORM_Split_Query      | 50   |    393.8 μs |   3.81 μs |   2.26 μs |   1.97 |    0.02 |   3.9063 |        - |   15.92 KB |        2.07 |
| Dapper_Record_DTO         | 50   |    196.0 μs |   2.70 μs |   1.79 μs |   0.98 |    0.01 |   1.7090 |        - |     7.7 KB |        1.00 |
| ForgeORM_Record_DTO       | 50   |    197.6 μs |   4.98 μs |   2.96 μs |   0.99 |    0.02 |   1.7090 |        - |    7.71 KB |        1.00 |
| Dapper_Enum_Mapping       | 50   |    202.3 μs |   7.39 μs |   4.89 μs |   1.01 |    0.03 |   1.4648 |        - |    6.55 KB |        0.85 |
| ForgeORM_Enum_Mapping     | 50   |    198.9 μs |   6.50 μs |   3.87 μs |   1.00 |    0.02 |   1.4648 |        - |     6.6 KB |        0.86 |
| Dapper_Streaming          | 50   |    243.3 μs |   4.70 μs |   3.11 μs |   1.22 |    0.02 |   1.4648 |        - |    7.52 KB |        0.98 |
| EF_Core_Async_Streaming   | 50   |    414.6 μs |  93.39 μs |  61.77 μs |   2.08 |    0.30 |   7.8125 |        - |    33.8 KB |        4.39 |
| ForgeORM_Async_Streaming  | 50   |    261.7 μs |   7.74 μs |   4.61 μs |   1.31 |    0.03 |   2.9297 |        - |   12.61 KB |        1.64 |
| Dapper_Stored_Procedure   | 50   |    195.9 μs |   4.53 μs |   2.99 μs |   0.98 |    0.02 |   1.7090 |        - |    7.48 KB |        0.97 |
| ForgeORM_Stored_Procedure | 50   |    205.5 μs |   1.95 μs |   1.02 μs |   1.03 |    0.01 |   1.7090 |        - |    7.71 KB |        1.00 |
|                           |      |             |           |           |        |         |          |          |            |             |
| **Dapper_Query_By_Id**        | **100**  |    **198.8 μs** |   **8.62 μs** |   **5.70 μs** |   **1.00** |    **0.04** |   **1.7090** |        **-** |     **7.7 KB** |        **1.00** |
| EF_Core_Query_By_Id       | 100  |    263.5 μs |   7.04 μs |   4.19 μs |   1.33 |    0.04 |   4.3945 |        - |   19.74 KB |        2.56 |
| ForgeORM_Query_By_Id      | 100  |    201.5 μs |   6.18 μs |   3.68 μs |   1.01 |    0.03 |   1.7090 |        - |    7.71 KB |        1.00 |
| Dapper_Query_First        | 100  |    214.1 μs |   2.98 μs |   1.77 μs |   1.08 |    0.03 |   1.7090 |        - |    7.72 KB |        1.00 |
| EF_Core_Query_First       | 100  |    283.6 μs |   8.16 μs |   4.86 μs |   1.43 |    0.04 |   4.8828 |        - |   21.25 KB |        2.76 |
| ForgeORM_Query_First      | 100  |    211.0 μs |   5.99 μs |   3.57 μs |   1.06 |    0.03 |   1.7090 |        - |    7.73 KB |        1.00 |
| Dapper_Query_List         | 100  |    267.1 μs |   4.52 μs |   2.69 μs |   1.34 |    0.04 |   3.4180 |        - |   14.71 KB |        1.91 |
| EF_Core_Query_List        | 100  |    409.1 μs |  42.49 μs |  25.29 μs |   2.06 |    0.13 |  10.7422 |        - |   43.68 KB |        5.67 |
| ForgeORM_Query_List       | 100  |    269.4 μs |  10.06 μs |   6.66 μs |   1.36 |    0.05 |   2.9297 |        - |   12.61 KB |        1.64 |
| Dapper_Search_Paged       | 100  |    266.1 μs |   5.15 μs |   3.41 μs |   1.34 |    0.04 |   3.4180 |        - |   15.06 KB |        1.96 |
| EF_Core_Search_Paged      | 100  |    433.8 μs |  27.55 μs |  16.40 μs |   2.18 |    0.10 |  10.7422 |        - |   46.37 KB |        6.02 |
| ForgeORM_Search_Paged     | 100  |    273.3 μs |   6.51 μs |   3.87 μs |   1.38 |    0.04 |   3.4180 |        - |   14.92 KB |        1.94 |
| Dapper_Insert_Single      | 100  |    368.8 μs |   3.60 μs |   1.88 μs |   1.86 |    0.05 |   4.8828 |        - |   20.71 KB |        2.69 |
| EF_Core_Insert_Single     | 100  |    432.1 μs |  18.31 μs |  10.89 μs |   2.17 |    0.08 |   5.8594 |        - |   26.59 KB |        3.45 |
| ForgeORM_Insert_Single    | 100  |    368.3 μs |   3.71 μs |   2.21 μs |   1.85 |    0.05 |   4.8828 |        - |   20.69 KB |        2.69 |
| Dapper_Insert_Bulk        | 100  |          NA |        NA |        NA |      ? |       ? |       NA |       NA |         NA |           ? |
| EF_Core_Insert_Bulk       | 100  |  7,386.7 μs | 242.66 μs | 144.40 μs |  37.18 |    1.21 | 281.2500 | 234.3750 | 1480.11 KB |      192.12 |
| ForgeORM_Insert_Bulk      | 100  | 35,957.5 μs | 515.05 μs | 340.68 μs | 180.99 |    5.10 | 500.0000 |        - | 2052.98 KB |      266.48 |
| Dapper_Update             | 100  |    689.7 μs |  12.19 μs |   7.25 μs |   3.47 |    0.10 |   5.8594 |        - |      26 KB |        3.37 |
| EF_Core_Update            | 100  |    808.2 μs |  93.64 μs |  61.94 μs |   4.07 |    0.32 |   7.8125 |        - |    34.4 KB |        4.46 |
| ForgeORM_Update           | 100  |    683.7 μs |   9.52 μs |   5.67 μs |   3.44 |    0.10 |   5.8594 |        - |   25.98 KB |        3.37 |
| Dapper_Delete             | 100  |    732.4 μs |  24.84 μs |  14.78 μs |   3.69 |    0.12 |   5.8594 |        - |   25.68 KB |        3.33 |
| EF_Core_Delete            | 100  |  1,074.1 μs |  72.89 μs |  43.37 μs |   5.41 |    0.25 |  11.7188 |        - |   52.79 KB |        6.85 |
| ForgeORM_Delete           | 100  |    731.1 μs |  13.00 μs |   8.60 μs |   3.68 |    0.11 |   5.8594 |        - |   25.67 KB |        3.33 |
| ForgeORM_Graph_Insert     | 100  |  1,166.8 μs |  27.53 μs |  14.40 μs |   5.87 |    0.17 |  15.6250 |        - |    70.3 KB |        9.13 |
| ForgeORM_Graph_Update     | 100  |  2,532.8 μs |  66.59 μs |  34.83 μs |  12.75 |    0.38 |  31.2500 |        - |  141.46 KB |       18.36 |
| Dapper_Split_Query        | 100  |    334.1 μs |   7.63 μs |   5.04 μs |   1.68 |    0.05 |  22.4609 |   3.9063 |    92.5 KB |       12.01 |
| EF_Core_Split_Query       | 100  |    445.4 μs |  11.12 μs |   5.82 μs |   2.24 |    0.07 |   6.8359 |        - |   30.27 KB |        3.93 |
| ForgeORM_Split_Query      | 100  |    400.8 μs |  10.16 μs |   6.72 μs |   2.02 |    0.06 |   3.9063 |        - |   15.92 KB |        2.07 |
| Dapper_Record_DTO         | 100  |    198.4 μs |   4.25 μs |   2.81 μs |   1.00 |    0.03 |   1.4648 |        - |     7.7 KB |        1.00 |
| ForgeORM_Record_DTO       | 100  |    198.2 μs |   3.49 μs |   2.08 μs |   1.00 |    0.03 |   1.7090 |        - |    7.71 KB |        1.00 |
| Dapper_Enum_Mapping       | 100  |    199.7 μs |   4.59 μs |   2.73 μs |   1.00 |    0.03 |   1.4648 |        - |    6.55 KB |        0.85 |
| ForgeORM_Enum_Mapping     | 100  |    200.3 μs |   2.23 μs |   1.16 μs |   1.01 |    0.03 |   1.4648 |        - |     6.6 KB |        0.86 |
| Dapper_Streaming          | 100  |    245.9 μs |   4.15 μs |   2.74 μs |   1.24 |    0.04 |   1.4648 |        - |    7.52 KB |        0.98 |
| EF_Core_Async_Streaming   | 100  |    356.5 μs |  34.01 μs |  17.79 μs |   1.79 |    0.10 |   7.8125 |        - |    33.8 KB |        4.39 |
| ForgeORM_Async_Streaming  | 100  |    265.5 μs |   6.37 μs |   3.79 μs |   1.34 |    0.04 |   2.9297 |        - |   12.61 KB |        1.64 |
| Dapper_Stored_Procedure   | 100  |    197.9 μs |   5.77 μs |   3.82 μs |   1.00 |    0.03 |   1.7090 |        - |    7.48 KB |        0.97 |
| ForgeORM_Stored_Procedure | 100  |    208.4 μs |   2.31 μs |   1.21 μs |   1.05 |    0.03 |   1.7090 |        - |    7.71 KB |        1.00 |

Benchmarks with issues:
  CoreScenarioBenchmarks.Dapper_Insert_Bulk: Job-HEIGNX(IterationCount=10, WarmupCount=3) [Take=10]
  CoreScenarioBenchmarks.Dapper_Insert_Bulk: Job-HEIGNX(IterationCount=10, WarmupCount=3) [Take=50]
  CoreScenarioBenchmarks.Dapper_Insert_Bulk: Job-HEIGNX(IterationCount=10, WarmupCount=3) [Take=100]
