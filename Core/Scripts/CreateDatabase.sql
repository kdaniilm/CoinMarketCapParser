IF DB_ID('CoinMarketDb') IS NULL
BEGIN
    CREATE DATABASE CoinMarketDb;
END
GO

USE CoinMarketDb;
GO

IF OBJECT_ID('dbo.CoinMarketCup', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoinMarketCup
    (
        [Rank] INT NOT NULL,
        [Name] NVARCHAR(200) NULL,
        [Symbol] NVARCHAR(50) NULL,
        [MarketCap] DECIMAL(18, 2) NOT NULL,
        [Price] DECIMAL(18, 8) NOT NULL,
        [CirculatingSupply] BIGINT NULL,
        [Volume24h] BIGINT NULL,
        [Percent24h] DECIMAL(10, 4) NOT NULL,

        CONSTRAINT PK_CoinMarketCup PRIMARY KEY (Rank)
    );
END
GO