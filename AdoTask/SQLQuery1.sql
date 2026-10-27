USE SpainChampionship;
GO

-- 2. Таблица команд
CREATE TABLE Teams (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- 3. Таблица игроков
CREATE TABLE Players (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    TeamId INT FOREIGN KEY REFERENCES Teams(Id) ON DELETE CASCADE
);

-- 4. Таблица матчей
CREATE TABLE Matches (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    HomeTeamId INT FOREIGN KEY REFERENCES Teams(Id),
    AwayTeamId INT FOREIGN KEY REFERENCES Teams(Id),
    HomeGoals INT NOT NULL,
    AwayGoals INT NOT NULL,
    MatchDate DATETIME NOT NULL
);

-- 5. Таблица забитых голов (для точной статистики бомбардиров)
CREATE TABLE Goals (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MatchId INT FOREIGN KEY REFERENCES Matches(Id) ON DELETE CASCADE,
    PlayerId INT FOREIGN KEY REFERENCES Players(Id),
    Minute INT NOT NULL
);
GO

-- Добавим 4 известные команды Испании
INSERT INTO Teams (Name) VALUES ('Real Madrid'), ('Barcelona'), ('Atletico Madrid'), ('Sevilla');

-- Добавим по паре игроков для демонстрации
INSERT INTO Players (Name, TeamId) VALUES 
('Vinicius Jr', 1), ('Kylian Mbappe', 1),
('Robert Lewandowski', 2), ('Lamine Yamal', 2),
('Antoine Griezmann', 3), ('Alexander Sorloth', 3),
('Lucas Ocampos', 4), ('Isaac Romero', 4);
GO

CREATE OR ALTER PROCEDURE sp_GenerateRandomMatches
    @NumberOfMatches INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Counter INT = 0;
    DECLARE @HomeTeamId INT, @AwayTeamId INT;
    DECLARE @HomeGoals INT, @AwayGoals INT;
    DECLARE @MatchId INT;

    WHILE @Counter < @NumberOfMatches
    BEGIN
        -- Выбираем случайные команды
        SELECT TOP 1 @HomeTeamId = Id FROM Teams ORDER BY NEWID();
        SELECT TOP 1 @AwayTeamId = Id FROM Teams WHERE Id <> @HomeTeamId ORDER BY NEWID();

        -- Случайный счет от 0 до 4
        SET @HomeGoals = ABS(CHECKSUM(NEWID())) % 5;
        SET @AwayGoals = ABS(CHECKSUM(NEWID())) % 5;

        -- Вставляем матч
        INSERT INTO Matches (HomeTeamId, AwayTeamId, HomeGoals, AwayGoals, MatchDate)
        VALUES (@HomeTeamId, @AwayTeamId, @HomeGoals, @AwayGoals, DATEADD(day, -@Counter, GETDATE()));
        
        SET @MatchId = SCOPE_IDENTITY();

        -- Генерируем голы для домашних игроков
        DECLARE @G INT = 0;
        WHILE @G < @HomeGoals
        BEGIN
            DECLARE @HomePlayerId INT = (SELECT TOP 1 Id FROM Players WHERE TeamId = @HomeTeamId ORDER BY NEWID());
            IF @HomePlayerId IS NOT NULL
                INSERT INTO Goals (MatchId, PlayerId, Minute) VALUES (@MatchId, @HomePlayerId, ABS(CHECKSUM(NEWID())) % 90 + 1);
            SET @G = @G + 1;
        END

        -- Генерируем голы для гостевых игроков
        SET @G = 0;
        WHILE @G < @AwayGoals
        BEGIN
            DECLARE @AwayPlayerId INT = (SELECT TOP 1 Id FROM Players WHERE TeamId = @AwayTeamId ORDER BY NEWID());
            IF @AwayPlayerId IS NOT NULL
                INSERT INTO Goals (MatchId, PlayerId, Minute) VALUES (@MatchId, @AwayPlayerId, ABS(CHECKSUM(NEWID())) % 90 + 1);
            SET @G = @G + 1;
        END

        SET @Counter = @Counter + 1;
    END
END;
GO

-- ЗАДАНIE 2: Общий список бомбардиров
CREATE OR ALTER VIEW v_AllScorers AS
SELECT p.Id, p.Name AS PlayerName, t.Name AS TeamName, COUNT(g.Id) AS GoalsCount
FROM Players p
JOIN Teams t ON p.TeamId = t.Id
LEFT JOIN Goals g ON p.Id = g.PlayerId
GROUP BY p.Id, p.Name, t.Name;
GO

-- ЗАДАНIE 2: Процедура для бомбардиров конкретной команды
CREATE OR ALTER PROCEDURE sp_GetTopScorersByTeam
    @TeamId INT,
    @TopCount INT = 3
AS
BEGIN
    SELECT TOP (@TopCount) Id, PlayerName, TeamName, GoalsCount
    FROM v_AllScorers
    WHERE Id IN (SELECT Id FROM Players WHERE TeamId = @TeamId)
    ORDER BY GoalsCount DESC;
END;
GO

-- ЗАДАНIE 3: Забитые и пропущенные голы команд
CREATE OR ALTER VIEW v_TeamGoalsStats AS
SELECT 
    t.Id, 
    t.Name AS TeamName,
    ISNULL(SUM(CASE WHEN m.HomeTeamId = t.Id THEN m.HomeGoals ELSE 0 END), 0) +
    ISNULL(SUM(CASE WHEN m.AwayTeamId = t.Id THEN m.AwayGoals ELSE 0 END), 0) AS GoalsScored,
    ISNULL(SUM(CASE WHEN m.HomeTeamId = t.Id THEN m.AwayGoals ELSE 0 END), 0) +
    ISNULL(SUM(CASE WHEN m.AwayTeamId = t.Id THEN m.HomeGoals ELSE 0 END), 0) AS GoalsConceded
FROM Teams t
LEFT JOIN Matches m ON t.Id = m.HomeTeamId OR t.Id = m.AwayTeamId
GROUP BY t.Id, t.Name;
GO

-- ЗАДАНIE 4: Таблица очков чемпионата
CREATE OR ALTER VIEW v_LeagueTable AS
SELECT 
    t.Id,
    t.Name AS TeamName,
    SUM(CASE 
        WHEN m.HomeTeamId = t.Id AND m.HomeGoals > m.AwayGoals THEN 3
        WHEN m.AwayTeamId = t.Id AND m.AwayGoals > m.HomeGoals THEN 3
        WHEN (m.HomeTeamId = t.Id OR m.AwayTeamId = t.Id) AND m.HomeGoals = m.AwayGoals THEN 1
        ELSE 0 
    END) AS Points
FROM Teams t
LEFT JOIN Matches m ON t.Id = m.HomeTeamId OR t.Id = m.AwayTeamId
GROUP BY t.Id, t.Name;
GO
    