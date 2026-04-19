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
        [ParsedDate] DATETIME NOT NULL,
        [Rank] INT NOT NULL,
        [Name] NVARCHAR(200)  NOT NULL,
        [Symbol] NVARCHAR(50) NOT NULL,
        [MarketCap] DECIMAL(18, 2) NOT NULL,
        [Price] DECIMAL(18, 8) NOT NULL,
        [CirculatingSupply] BIGINT NULL,
        [Volume24h] BIGINT NULL,
        [Percent24h] DECIMAL(10, 4) NOT NULL
    );

    CREATE INDEX IX_CoinMarketCup_ParsedDate ON dbo.CoinMarketCup(ParsedDate);
    CREATE INDEX IX_CoinMarketCup_Symbol_Name ON dbo.CoinMarketCup (Symbol, [Name]);
    CREATE INDEX IX_CoinMarketCup_Rank ON dbo.CoinMarketCup ([Rank]);

    CREATE INDEX IX_CoinMarketCup_MarketCap ON dbo.CoinMarketCup (MarketCap);
    CREATE INDEX IX_CoinMarketCup_Price ON dbo.CoinMarketCup (Price);
    CREATE INDEX IX_CoinMarketCup_Volume24h ON dbo.CoinMarketCup (Volume24h);
    CREATE INDEX IX_CoinMarketCup_Percent24h ON dbo.CoinMarketCup (Percent24h);
END
GO