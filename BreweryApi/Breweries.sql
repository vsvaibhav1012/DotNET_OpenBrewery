-- ================================================
-- Brewery API Database Schema
-- SQLite Compatible Schema for Production Upgrade
-- ================================================

CREATE TABLE IF NOT EXISTS Breweries (
    Id TEXT PRIMARY KEY NOT NULL,
    Name TEXT NOT NULL,
    BreweryType TEXT NOT NULL,
    Address1 TEXT,
    Address2 TEXT,
    Address3 TEXT,
    City TEXT NOT NULL,
    StateProvince TEXT,
    PostalCode TEXT,
    Country TEXT,
    Phone TEXT,
    WebsiteUrl TEXT,
    Latitude REAL,
    Longitude REAL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);


CREATE INDEX IF NOT EXISTS idx_breweries_name ON Breweries(Name);
CREATE INDEX IF NOT EXISTS idx_breweries_city ON Breweries(City);
CREATE INDEX IF NOT EXISTS idx_breweries_state ON Breweries(StateProvince);
CREATE INDEX IF NOT EXISTS idx_breweries_type ON Breweries(BreweryType);
CREATE INDEX IF NOT EXISTS idx_breweries_location ON Breweries(Latitude, Longitude);
CREATE INDEX IF NOT EXISTS idx_breweries_name_search ON Breweries(Name COLLATE NOCASE);


CREATE VIRTUAL TABLE IF NOT EXISTS BreweriesSearch USING fts5(
    Id UNINDEXED,
    Name,
    City,
    StateProvince,
    content='Breweries',
    content_rowid='rowid'
);


CREATE TRIGGER IF NOT EXISTS breweries_ai AFTER INSERT ON Breweries BEGIN
    INSERT INTO BreweriesSearch(Id, Name, City, StateProvince) 
    VALUES (new.Id, new.Name, new.City, new.StateProvince);
END;

CREATE TRIGGER IF NOT EXISTS breweries_ad AFTER DELETE ON Breweries BEGIN
    INSERT INTO BreweriesSearch(BreweriesSearch, Id, Name, City, StateProvince) 
    VALUES('delete', old.Id, old.Name, old.City, old.StateProvince);
END;

CREATE TRIGGER IF NOT EXISTS breweries_au AFTER UPDATE ON Breweries BEGIN
    INSERT INTO BreweriesSearch(BreweriesSearch, Id, Name, City, StateProvince) 
    VALUES('delete', old.Id, old.Name, old.City, old.StateProvince);
    INSERT INTO BreweriesSearch(Id, Name, City, StateProvince) 
    VALUES (new.Id, new.Name, new.City, new.StateProvince);
END;


INSERT OR IGNORE INTO Breweries (Id, Name, BreweryType, City, StateProvince, Country, Phone, Latitude, Longitude) VALUES
('sample-1', 'Stone Brewing', 'regional', 'San Diego', 'California', 'United States', '+1-760-294-7866', 33.1286, -117.1837),
('sample-2', 'Ballast Point Brewing', 'regional', 'San Diego', 'California', 'United States', '+1-619-255-7213', 32.7480, -117.1931),
('sample-3', 'Russian River Brewing', 'regional', 'Santa Rosa', 'California', 'United States', '+1-707-545-2337', 38.4404, -122.7144);


CREATE VIEW IF NOT EXISTS BreweryWithDistance AS
SELECT 
    b.*,
    CASE 
        WHEN b.Latitude IS NOT NULL AND b.Longitude IS NOT NULL 
        THEN 0.0 
        ELSE NULL 
    END as Distance
FROM Breweries b;
