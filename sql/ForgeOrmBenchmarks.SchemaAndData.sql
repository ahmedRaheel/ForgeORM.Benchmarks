IF DB_ID('ForgeOrmBenchmarks') IS NULL
BEGIN
    CREATE DATABASE ForgeOrmBenchmarks;
END
GO

USE ForgeOrmBenchmarks;
GO

IF OBJECT_ID('dbo.OrderItems', 'U') IS NOT NULL DROP TABLE dbo.OrderItems;
IF OBJECT_ID('dbo.Payments', 'U') IS NOT NULL DROP TABLE dbo.Payments;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
GO

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
GO

CREATE INDEX IX_Products_CategoryId ON dbo.Products(CategoryId);
CREATE INDEX IX_Orders_CustomerId ON dbo.Orders(CustomerId);
CREATE INDEX IX_Orders_Status ON dbo.Orders(Status);
CREATE INDEX IX_Orders_Total ON dbo.Orders(GrandTotal);
CREATE INDEX IX_OrderItems_OrderId ON dbo.OrderItems(OrderId);
CREATE INDEX IX_OrderItems_ProductId ON dbo.OrderItems(ProductId);
CREATE INDEX IX_Payments_OrderId ON dbo.Payments(OrderId);
GO

INSERT INTO dbo.Categories (Name, Description)
VALUES
('Electronics', 'Electronic products'),
('Books', 'Books and learning material'),
('Fashion', 'Clothing and accessories'),
('Home', 'Home and kitchen products'),
('Sports', 'Sports products');
GO

DECLARE @p INT = 1;
WHILE @p <= 100
BEGIN
    INSERT INTO dbo.Products(CategoryId, Sku, Name, UnitPrice, StockQuantity)
    VALUES (((@p - 1) % 5) + 1, CONCAT('SKU-', FORMAT(@p, '00000')), CONCAT('Product ', FORMAT(@p, '00000')), 50 + @p, 1000 - @p);
    SET @p += 1;
END;
GO

DECLARE @customerIndex INT = 1;
WHILE @customerIndex <= 1000
BEGIN
    INSERT INTO dbo.Customers(FullName, Email, Phone, City, IsActive, CreatedAt)
    VALUES
    (
        CONCAT('Customer ', FORMAT(@customerIndex, '00000')),
        CONCAT('customer', FORMAT(@customerIndex, '00000'), '@example.com'),
        CONCAT('0300', FORMAT(@customerIndex, '0000000')),
        CASE WHEN @customerIndex % 2 = 0 THEN 'Karachi' ELSE 'Lahore' END,
        1,
        SYSUTCDATETIME()
    );

    DECLARE @customerId INT = SCOPE_IDENTITY();
    DECLARE @orderIndex INT = 1;

    WHILE @orderIndex <= 20
    BEGIN
        DECLARE @status NVARCHAR(50) =
            CASE
                WHEN @orderIndex % 4 = 0 THEN 'Paid'
                WHEN @orderIndex % 4 = 1 THEN 'Processing'
                WHEN @orderIndex % 4 = 2 THEN 'Pending'
                ELSE 'Cancelled'
            END;
        DECLARE @subTotal DECIMAL(18,2) = 100 + @orderIndex;
        DECLARE @tax DECIMAL(18,2) = @subTotal * 0.15;
        DECLARE @grandTotal DECIMAL(18,2) = @subTotal + @tax;

        INSERT INTO dbo.Orders(OrderNo, CustomerId, OrderDate, Status, SubTotal, Tax, GrandTotal, CreatedAt)
        VALUES
        (
            CONCAT('ORD-', FORMAT(@customerIndex, '00000'), '-', FORMAT(@orderIndex, '00000')),
            @customerId,
            DATEADD(DAY, -@orderIndex, SYSUTCDATETIME()),
            @status,
            @subTotal,
            @tax,
            @grandTotal,
            SYSUTCDATETIME()
        );

        DECLARE @orderId INT = SCOPE_IDENTITY();
        DECLARE @itemIndex INT = 1;

        WHILE @itemIndex <= 3
        BEGIN
            DECLARE @productId INT = ((@orderIndex + @itemIndex - 1) % 100) + 1;
            DECLARE @quantity INT = @itemIndex + 1;
            DECLARE @unitPrice DECIMAL(18,2) = 50 + @productId;

            INSERT INTO dbo.OrderItems(OrderId, ProductId, Quantity, UnitPrice, LineTotal)
            VALUES (@orderId, @productId, @quantity, @unitPrice, @quantity * @unitPrice);

            SET @itemIndex += 1;
        END;

        IF @status = 'Paid'
        BEGIN
            INSERT INTO dbo.Payments(OrderId, PaymentDate, Amount, Method, Status)
            VALUES (@orderId, SYSUTCDATETIME(), @grandTotal, CASE WHEN @orderIndex % 2 = 0 THEN 'Card' ELSE 'Cash' END, 'Completed');
        END;

        SET @orderIndex += 1;
    END;

    SET @customerIndex += 1;
END;
GO
