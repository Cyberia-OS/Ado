CREATE DATABASE SpainFootballDB;
GO

USE SpainFootballDB;
GO

CREATE TABLE Teams (
    TeamID INT IDENTITY(1,1) PRIMARY KEY,
    TeamName NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Players (
    PlayerID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    PlayerNumber INT NOT NULL,
    Position NVARCHAR(50) NOT NULL,
    TeamID INT FOREIGN KEY REFERENCES Teams(TeamID) ON DELETE CASCADE
);

CREATE TABLE Matches (
    MatchID INT IDENTITY(1,1) PRIMARY KEY,
    Team1ID INT FOREIGN KEY REFERENCES Teams(TeamID),
    Team2ID INT FOREIGN KEY REFERENCES Teams(TeamID),
    Team1Goals INT NOT NULL DEFAULT 0,
    Team2Goals INT NOT NULL DEFAULT 0,
    MatchDate DATE NOT NULL,
    CONSTRAINT CK_DifferentTeams CHECK (Team1ID <> Team2ID)
);

CREATE TABLE MatchGoals (
    GoalID INT IDENTITY(1,1) PRIMARY KEY,
    MatchID INT FOREIGN KEY REFERENCES Matches(MatchID) ON DELETE CASCADE,
    PlayerID INT FOREIGN KEY REFERENCES Players(PlayerID),
    MinuteOfGoal INT NULL
);
GO

CREATE OR ALTER VIEW View_TeamGoalDifference AS
WITH TeamStats AS (
    SELECT Team1ID AS TeamID, Team1Goals AS Scored, Team2Goals AS Conceded FROM Matches
    UNION ALL
    SELECT Team2ID AS TeamID, Team2Goals AS Scored, Team1Goals AS Conceded FROM Matches
)
SELECT 
    t.TeamName,
    ISNULL(SUM(ts.Scored), 0) AS TotalScored,
    ISNULL(SUM(ts.Conceded), 0) AS TotalConceded,
    (ISNULL(SUM(ts.Scored), 0) - ISNULL(SUM(ts.Conceded), 0)) AS GoalDifference
FROM Teams t
LEFT JOIN TeamStats ts ON t.TeamID = ts.TeamID
GROUP BY t.TeamID, t.TeamName;
GO

CREATE OR ALTER VIEW View_FullMatchDetails AS
SELECT 
    m.MatchID,
    t1.TeamName AS HomeTeam,
    t2.TeamName AS AwayTeam,
    m.Team1Goals AS HomeGoals,
    m.Team2Goals AS AwayGoals,
    m.MatchDate,
    ISNULL(STRING_AGG(p.FullName, ', ') WITHIN GROUP (ORDER BY mg.MinuteOfGoal), N'Нет голов') AS Scorers
FROM Matches m
JOIN Teams t1 ON m.Team1ID = t1.TeamID
JOIN Teams t2 ON m.Team2ID = t2.TeamID
LEFT JOIN MatchGoals mg ON m.MatchID = mg.MatchID
LEFT JOIN Players p ON mg.PlayerID = p.PlayerID
GROUP BY m.MatchID, t1.TeamName, t2.TeamName, m.Team1Goals, m.Team2Goals, m.MatchDate;
GO

CREATE OR ALTER FUNCTION GetScorersByDate (@TargetDate DATE)
RETURNS TABLE
AS
RETURN (
    SELECT DISTINCT p.FullName, t.TeamName, p.Position
    FROM MatchGoals mg
    JOIN Matches m ON mg.MatchID = m.MatchID
    JOIN Players p ON mg.PlayerID = p.PlayerID
    JOIN Teams t ON p.TeamID = t.TeamID
    WHERE m.MatchDate = @TargetDate
);
GO

CREATE OR ALTER PROCEDURE AddMatchWithCheck
    @Team1Name NVARCHAR(100),
    @Team2Name NVARCHAR(100),
    @Goals1 INT,
    @Goals2 INT,
    @MatchDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @T1ID INT = (SELECT TeamID FROM Teams WHERE TeamName = @Team1Name);
    DECLARE @T2ID INT = (SELECT TeamID FROM Teams WHERE TeamName = @Team2Name);

    IF @T1ID IS NULL OR @T2ID IS NULL
    BEGIN
        ;THROW 50001, 'Одна или обе команды не найдены в справочнике Teams!', 1;
    END

    IF EXISTS (SELECT 1 FROM Matches WHERE Team1ID = @T1ID AND Team2ID = @T2ID AND MatchDate = @MatchDate)
    BEGIN
        PRINT 'Данный поединок уже зарегистрирован в системе.';
        RETURN;
    END

    INSERT INTO Matches (Team1ID, Team2ID, Team1Goals, Team2Goals, MatchDate)
    VALUES (@T1ID, @T2ID, @Goals1, @Goals2, @MatchDate);
END;
GO

CREATE OR ALTER PROCEDURE UpdateMatchDetails
    @MatchID INT,
    @NewGoals1 INT,
    @NewGoals2 INT,
    @NewDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Matches
    SET Team1Goals = @NewGoals1,
        Team2Goals = @NewGoals2,
        MatchDate = @NewDate
    WHERE MatchID = @MatchID;
END;
GO

INSERT INTO Teams (TeamName) VALUES (N'Реал Мадрид'), (N'Барселона'), (N'Атлетико Мадрид');

INSERT INTO Players (FullName, Country, PlayerNumber, Position, TeamID) VALUES 
(N'Винисиус Жуниор', N'Бразилия', 7, N'Нападающий', 1),
(N'Роберт Левандовский', N'Польша', 9, N'Нападающий', 2),
(N'Антуан Гризманн', N'Франция', 7, N'Полузащитник', 3);

EXEC AddMatchWithCheck N'Реал Мадрид', N'Барселона', 2, 1, '2026-10-29';

INSERT INTO MatchGoals (MatchID, PlayerID, MinuteOfGoal) VALUES (1, 1, 42);
GO
