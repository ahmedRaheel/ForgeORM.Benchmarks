namespace ForgeORM.Benchmarks.Sql;

public static class BenchmarkSql
{
    public const string CreateSchema = """
IF OBJECT_ID('dbo.OrderItems', 'U') IS NOT NULL DROP TABLE dbo.OrderItems;
IF OBJECT_ID('dbo.Payments', 'U') IS NOT NULL DROP TABLE dbo.Payments;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;

CREATE TABLE dbo.Customers
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(50) NULL,
    City NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.Categories
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL
);

CREATE TABLE dbo.Products
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CategoryId INT NOT NULL,
    Sku NVARCHAR(100) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    StockQuantity INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id)
);

CREATE TABLE dbo.Orders
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    OrderNo NVARCHAR(50) NOT NULL,
    CustomerId INT NOT NULL,
    OrderDate DATETIME2 NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    SubTotal DECIMAL(18,2) NOT NULL,
    Tax DECIMAL(18,2) NOT NULL,
    GrandTotal DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id)
);

CREATE TABLE dbo.OrderItems
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
);

CREATE TABLE dbo.Payments
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    OrderId INT NOT NULL,
    PaymentDate DATETIME2 NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Method NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id)
);

CREATE INDEX IX_Products_CategoryId ON dbo.Products(CategoryId);
CREATE INDEX IX_Orders_CustomerId ON dbo.Orders(CustomerId);
CREATE INDEX IX_Orders_Status ON dbo.Orders(Status);
CREATE INDEX IX_Orders_Total ON dbo.Orders(GrandTotal);
CREATE INDEX IX_OrderItems_OrderId ON dbo.OrderItems(OrderId);
CREATE INDEX IX_OrderItems_ProductId ON dbo.OrderItems(ProductId);
CREATE INDEX IX_Payments_OrderId ON dbo.Payments(OrderId);
""";

    public const string QueryById = """
SELECT TOP (1)
    o.Id,
    o.OrderNo,
    o.CustomerId,
    c.FullName AS CustomerName,
    o.GrandTotal,
    o.Status,
    o.OrderDate
FROM dbo.Orders o
INNER JOIN dbo.Customers c ON c.Id = o.CustomerId
WHERE o.Id = @Id;
""";

    public const string SearchPaged = """
SELECT
    o.Id,
    o.OrderNo,
    o.CustomerId,
    c.FullName AS CustomerName,
    o.GrandTotal,
    o.Status,
    o.OrderDate
FROM dbo.Orders o
INNER JOIN dbo.Customers c ON c.Id = o.CustomerId
WHERE o.CustomerId = @CustomerId
ORDER BY o.Id DESC
OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
""";

    public const string WhereSql = """
SELECT o.Id, o.OrderNo, o.CustomerId, o.OrderDate, o.Status, o.SubTotal, o.Tax, o.GrandTotal, o.CreatedAt
FROM dbo.Orders o
WHERE o.CustomerId = @CustomerId;
""";

    public const string WhereAndOrderSql = """
SELECT o.Id, o.OrderNo, o.CustomerId, o.OrderDate, o.Status, o.SubTotal, o.Tax, o.GrandTotal, o.CreatedAt
FROM dbo.Orders o
WHERE o.CustomerId = @CustomerId AND o.Status = @Status
ORDER BY o.Id DESC;
""";

    public const string FirstSql = """
SELECT TOP (1) o.Id, o.OrderNo, o.CustomerId, o.OrderDate, o.Status, o.SubTotal, o.Tax, o.GrandTotal, o.CreatedAt
FROM dbo.Orders o
WHERE o.Id = @Id;
""";

    public const string SingleSql = """
SELECT TOP (2) o.Id, o.OrderNo, o.CustomerId, o.OrderDate, o.Status, o.SubTotal, o.Tax, o.GrandTotal, o.CreatedAt
FROM dbo.Orders o
WHERE o.Id = @Id;
""";

    public const string AnySql = """
SELECT CASE WHEN EXISTS(SELECT 1 FROM dbo.Orders o WHERE o.CustomerId = @CustomerId AND o.Status = @Status) THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END;
""";

    public const string CountSql = """
SELECT COUNT(1) FROM dbo.Orders o WHERE o.CustomerId = @CustomerId;
""";

    public const string SumSql = """
SELECT SUM(o.GrandTotal) FROM dbo.Orders o WHERE o.CustomerId = @CustomerId;
""";

    public const string AverageSql = """
SELECT AVG(o.GrandTotal) FROM dbo.Orders o WHERE o.CustomerId = @CustomerId;
""";

    public const string MinSql = """
SELECT MIN(o.GrandTotal) FROM dbo.Orders o WHERE o.CustomerId = @CustomerId;
""";

    public const string MaxSql = """
SELECT MAX(o.GrandTotal) FROM dbo.Orders o WHERE o.CustomerId = @CustomerId;
""";

    public const string GroupBySql = """
SELECT o.Status, COUNT(1) AS TotalOrders, SUM(o.GrandTotal) AS TotalAmount
FROM dbo.Orders o
GROUP BY o.Status
HAVING COUNT(1) > @MinCount
ORDER BY TotalAmount DESC;
""";

    public const string JoinSql = """
SELECT TOP (@Take)
    o.Id,
    o.OrderNo,
    o.CustomerId,
    c.FullName AS CustomerName,
    o.GrandTotal,
    o.Status,
    o.OrderDate
FROM dbo.Orders o
INNER JOIN dbo.Customers c ON c.Id = o.CustomerId
WHERE o.GrandTotal >= @MinTotal
ORDER BY o.Id DESC;
""";

    public const string InsertOrderSql = """
INSERT INTO dbo.Orders(OrderNo, CustomerId, OrderDate, Status, SubTotal, Tax, GrandTotal, CreatedAt)
VALUES (@OrderNo, @CustomerId, @OrderDate, @Status, @SubTotal, @Tax, @GrandTotal, SYSUTCDATETIME());
SELECT CAST(SCOPE_IDENTITY() AS INT);
""";

    public const string UpdateOrderStatusSql = """
UPDATE dbo.Orders SET Status = @Status WHERE Id = @Id;
""";

    public const string DeleteOrderSql = """
DELETE FROM dbo.Payments WHERE OrderId = @Id;
DELETE FROM dbo.OrderItems WHERE OrderId = @Id;
DELETE FROM dbo.Orders WHERE Id = @Id;
""";
}
