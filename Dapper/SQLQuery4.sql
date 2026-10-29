USE MailingList;
GO

CREATE OR ALTER VIEW View_SubscribersByCity AS
SELECT city AS GroupName, COUNT(*) AS TotalCount
FROM Subscribers
GROUP BY city;
GO

CREATE OR ALTER VIEW View_SubscribersByCountry AS
SELECT country AS GroupName, COUNT(*) AS TotalCount
FROM Subscribers
GROUP BY country;
GO

CREATE OR ALTER VIEW View_CitiesByCountry AS
SELECT country AS GroupName, COUNT(DISTINCT city) AS TotalCount
FROM Subscribers
GROUP BY country;
GO

CREATE OR ALTER VIEW View_AvgCitiesPerCountry AS
SELECT AVG(CAST(CityCount AS FLOAT)) AS AvgValue
FROM (
    SELECT country, COUNT(DISTINCT city) AS CityCount
    FROM Subscribers
    GROUP BY country
) AS CountryCityCounts;
GO

CREATE OR ALTER FUNCTION GetCategoriesByCountrySubscribers (@Country NVARCHAR(100))
RETURNS TABLE
AS
RETURN (
    SELECT DISTINCT c.name AS CategoryName
    FROM Categories c
    JOIN Subscriptions sub ON c.id = sub.category_id
    JOIN Subscribers s ON sub.subscriber_id = s.id
    WHERE s.country = @Country
);
GO
