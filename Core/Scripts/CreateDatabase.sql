CREATE TABLE IF NOT EXISTS CoinMarketCup
(
    ParsedDate TEXT NOT NULL,
    Rank INTEGER NOT NULL,
    Name TEXT NOT NULL,
    Symbol TEXT NOT NULL,
    MarketCap REAL NOT NULL,
    Price REAL NOT NULL,
    CirculatingSupply INTEGER NULL,
    Volume24h INTEGER NULL,
    Percent24h REAL NOT NULL
);

CREATE INDEX IF NOT EXISTS IX_CoinMarketCup_ParsedDate ON CoinMarketCup(ParsedDate);
CREATE INDEX IF NOT EXISTS IX_CoinMarketCup_Symbol_Name ON CoinMarketCup (Symbol, Name);
CREATE INDEX IF NOT EXISTS IX_CoinMarketCup_Rank ON CoinMarketCup (Rank);
CREATE INDEX IF NOT EXISTS IX_CoinMarketCup_MarketCap ON CoinMarketCup (MarketCap);
CREATE INDEX IF NOT EXISTS IX_CoinMarketCup_Price ON CoinMarketCup (Price);
CREATE INDEX IF NOT EXISTS IX_CoinMarketCup_Volume24h ON CoinMarketCup (Volume24h);
CREATE INDEX IF NOT EXISTS IX_CoinMarketCup_Percent24h ON CoinMarketCup (Percent24h);
