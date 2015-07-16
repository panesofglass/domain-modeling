USE master;
GO

DECLARE @dbname NVARCHAR(128);
SET @dbname = N'Database1';

IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = @dbname OR name = @dbname))
DROP DATABASE Database1;
GO

CREATE DATABASE Database1;
GO

USE Database1;
GO

CREATE TABLE [dbo].[CityLocations]
(
[City] NVARCHAR(250) NOT NULL PRIMARY KEY CLUSTERED,
[Latitude] FLOAT,
[Longitude] FLOAT
);
GO

INSERT INTO [dbo].[CityLocations] (City, Latitude, Longitude)
VALUES	(N'Beaumont, TX', 30.080174, -94.126556),
		(N'College Station, TX', 30.627977, -96.334407),
		(N'Conroe, TX', 30.311877, -95.456051),
		(N'Friendswood, TX', 29.529400, -95.201045),
		(N'Houston, TX', 29.760427, -95.369803),
		(N'Humble, TX', 29.998831, -95.262155),
		(N'Huntsville, TX', 30.723526, -95.550777),
		(N'Katy, TX', 29.785785, -95.824396),
		(N'Pearland, TX', 29.563567, -95.286047),
		(N'Spring, TX', 30.079940, -95.417160),
		(N'The Woodlands, TX', 30.165821, -95.461262),
		(N'San Mateo, CA', 31.700148, -106.275785),
		(N'London, UK', 51.507351, -0.127758),
		(N'Paris, FR', 48.856614, 2.352222),
		(N'Ciudad Mitad del Mundo, Equador', -0.002310, -78.455776),
		(N'Durban, SA', -30.618957, 30.546184),
		(N'Adelaide, AUS', -34.928621, 138.599959),
		(N'Atlantis', NULL, NULL),
		(N'Camelot', NULL, NULL)
GO
