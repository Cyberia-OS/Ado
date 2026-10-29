CREATE OR ALTER FUNCTION GetCategoriesByPopularity (@TopN INT, @IsPopular BIT)
RETURNS TABLE
AS
RETURN (
    SELECT TOP (@TopN) c.id AS category_id, c.name AS category_name, COUNT(s.subscriber_id) AS subscribers_count
    FROM Categories c
    LEFT JOIN Subscriptions s ON c.id = s.category_id
    GROUP BY c.id, c.name
    ORDER BY 
        CASE WHEN @IsPopular = 1 THEN COUNT(s.subscriber_id) END DESC,
        CASE WHEN @IsPopular = 0 THEN COUNT(s.subscriber_id) END ASC
);
GO

-- ЗАДАНИЕ 3: Разделы по кол-ву акций
CREATE OR ALTER FUNCTION GetCategoriesByPromoCount (@TopN INT, @IsHighest BIT)
RETURNS TABLE
AS
RETURN (
    SELECT TOP (@TopN) c.id AS category_id, c.name AS category_name, COUNT(p.id) AS promo_goods_count
    FROM Categories c
    LEFT JOIN PromotionalGoods p ON c.id = p.category_id
    GROUP BY c.id, c.name
    ORDER BY 
        CASE WHEN @IsHighest = 1 THEN COUNT(p.id) END DESC,
        CASE WHEN @IsHighest = 0 THEN COUNT(p.id) END ASC
);
GO

-- ЗАДАНИЕ 4: Сроки акций и Архивация

-- Продукты, где до конца акции 3 дня
CREATE OR ALTER VIEW View_PromosEndingIn3Days AS
SELECT id, name, category_id, end_date FROM PromotionalGoods
WHERE CAST(end_date AS DATE) = CAST(DATEADD(day, 3, GETDATE()) AS DATE);
GO

-- Истекшие акции
CREATE OR ALTER VIEW View_ExpiredPromos AS
SELECT id, name, category_id, end_date FROM PromotionalGoods
WHERE end_date < CAST(GETDATE() AS DATE);
GO

-- Хранимая процедура архивации
CREATE OR ALTER PROCEDURE ArchiveExpiredPromotions
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Перенос в архив
        INSERT INTO PromoArchive (id, name, category_id, end_date)
        SELECT id, name, category_id, end_date 
        FROM PromotionalGoods
        WHERE end_date < CAST(GETDATE() AS DATE);

        -- Удаление оригиналов
        DELETE FROM PromotionalGoods
        WHERE end_date < CAST(GETDATE() AS DATE);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Хелпер-функция расчета возраста для T-SQL
CREATE OR ALTER FUNCTION CalculateAge (@BirthDate DATE)
RETURNS INT
AS
BEGIN
    RETURN FLOOR(DATEDIFF(DAY, @BirthDate, GETDATE()) / 365.25);
END;
GO

-- Средний возраст по разделам
CREATE OR ALTER VIEW View_AvgAgeByCategory AS
SELECT c.name AS group_name, AVG(CAST(dbo.CalculateAge(s.birth_date) AS FLOAT)) AS avg_age
FROM Categories c
JOIN Subscriptions sub ON c.id = sub.category_id
JOIN Subscribers s ON sub.subscriber_id = s.id
GROUP BY c.id, c.name;
GO

-- Средний возраст по городам
CREATE OR ALTER VIEW View_AvgAgeByCity AS
SELECT city AS group_name, AVG(CAST(dbo.CalculateAge(birth_date) AS FLOAT)) AS avg_age
FROM Subscribers
GROUP BY city;
GO

-- Средний возраст по странам
CREATE OR ALTER VIEW View_AvgAgeByCountry AS
SELECT country AS group_name, AVG(CAST(dbo.CalculateAge(birth_date) AS FLOAT)) AS avg_age
FROM Subscribers
GROUP BY country;
GO

-- ЗАДАНИЕ 5: Статистика по полу

-- Топ-N разделов для каждого пола
CREATE OR ALTER FUNCTION GetTopCategoriesByGender (@TopN INT)
RETURNS TABLE
AS
RETURN (
    WITH GenderCategoryPopularity AS (
        SELECT 
            s.gender, 
            c.name AS category_name,
            ROW_NUMBER() OVER(PARTITION BY s.gender ORDER BY COUNT(sub.category_id) DESC) as rank_position
        FROM Subscribers s
        JOIN Subscriptions sub ON s.id = sub.subscriber_id
        JOIN Categories c ON sub.category_id = c.id
        GROUP BY s.gender, c.id, c.name
    )
    SELECT gender, category_name, CAST(rank_position AS INT) as rank_position
    FROM GenderCategoryPopularity
    WHERE rank_position <= @TopN
);
GO

-- Кол-во покупателей каждого пола
CREATE OR ALTER VIEW View_SubscriberCountByGender AS
SELECT gender AS group_name, COUNT(*) AS total_count
FROM Subscribers
GROUP BY gender;
GO

-- Кол-во покупателей по полам и странам
CREATE OR ALTER VIEW View_SubscriberCountByGenderAndCountry AS
SELECT country, gender, COUNT(*) AS total_count
FROM Subscribers
GROUP BY country, gender;
GO
