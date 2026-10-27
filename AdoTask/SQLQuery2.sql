USE SpainChampionship;
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

    IF NOT EXISTS (SELECT 1 FROM Teams)
    BEGIN
        INSERT INTO Teams (Name) VALUES ('Real Madrid'), ('Barcelona'), ('Atletico Madrid'), ('Sevilla');
        
        INSERT INTO Players (Name, TeamId) VALUES 
        ('Vinicius Jr', 1), ('Kylian Mbappe', 1),
        ('Robert Lewandowski', 2), ('Lamine Yamal', 2),
        ('Antoine Griezmann', 3), ('Alexander Sorloth', 3),
        ('Lucas Ocampos', 4), ('Isaac Romero', 4);
    END

    WHILE @Counter < @NumberOfMatches
    BEGIN
        SELECT TOP 1 @HomeTeamId = Id FROM Teams ORDER BY NEWID();
        SELECT TOP 1 @AwayTeamId = Id FROM Teams WHERE Id <> @HomeTeamId ORDER BY NEWID();

        SET @HomeGoals = ABS(CHECKSUM(NEWID())) % 5;
        SET @AwayGoals = ABS(CHECKSUM(NEWID())) % 5;

        INSERT INTO Matches (HomeTeamId, AwayTeamId, HomeGoals, AwayGoals, MatchDate)
        VALUES (@HomeTeamId, @AwayTeamId, @HomeGoals, @AwayGoals, DATEADD(day, -@Counter, GETDATE()));
        
        SET @MatchId = SCOPE_IDENTITY();

        DECLARE @G INT = 0;
        WHILE @G < @HomeGoals
        BEGIN
            DECLARE @HomePlayerId INT = (SELECT TOP 1 Id FROM Players WHERE TeamId = @HomeTeamId ORDER BY NEWID());
            IF @HomePlayerId IS NOT NULL
                INSERT INTO Goals (MatchId, PlayerId, Minute) VALUES (@MatchId, @HomePlayerId, ABS(CHECKSUM(NEWID())) % 90 + 1);
            SET @G = @G + 1;
        END

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

CREATE OR ALTER VIEW v_AllScorers AS
SELECT p.Id, p.Name AS PlayerName, t.Name AS TeamName, COUNT(g.Id) AS GoalsCount
FROM Players p
JOIN Teams t ON p.TeamId = t.Id
LEFT JOIN Goals g ON p.Id = g.PlayerId
GROUP BY p.Id, p.Name, t.Name;
GO

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
