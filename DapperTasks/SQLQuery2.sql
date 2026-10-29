-- 1. Таблица разделов товаров (категорий)
CREATE TABLE Sections (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);

-- 2. Таблица покупателей
CREATE TABLE Buyers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(150) NOT NULL,
    BirthDate DATE NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Country NVARCHAR(100) NOT NULL,
    City NVARCHAR(100) NOT NULL
);

-- 3. Таблица интересов покупателей (Связь многие-ко-многим между Buyers и Sections)
CREATE TABLE BuyerInterests (
    BuyerId INT FOREIGN KEY REFERENCES Buyers(Id) ON DELETE CASCADE,
    SectionId INT FOREIGN KEY REFERENCES Sections(Id) ON DELETE CASCADE,
    PRIMARY KEY (BuyerId, SectionId)
);

-- 4. Таблица акционных товаров
CREATE TABLE PromoProducts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(150) NOT NULL,
    SectionId INT FOREIGN KEY REFERENCES Sections(Id) ON DELETE CASCADE,
    Country NVARCHAR(100) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL
);

INSERT INTO Sections (Name) VALUES (N'Мобильные телефоны'), (N'Ноутбуки'), (N'Кухонная техника');

INSERT INTO Buyers (FullName, BirthDate, Gender, Email, Country, City) VALUES 
(N'Иван Иванов', '1990-05-12', N'Мужской', 'ivan@mail.com', N'Россия', N'Москва'),
(N'Анна Сидорова', '1995-09-23', N'Женский', 'anna@mail.com', N'Россия', N'Сочи'),
(N'Джон Смит', '1988-01-15', N'Мужской', 'john@gmail.com', N'США', N'Нью-Йорк');

INSERT INTO BuyerInterests VALUES (1, 1), (1, 2), (2, 3), (3, 2);

INSERT INTO PromoProducts (ProductName, SectionId, Country, StartDate, EndDate) VALUES
(N'iPhone 15 со скидкой 20%', 1, N'Россия', '2026-11-01', '2026-11-30'),
(N'Ноутбук ASUS ROG Pro', 2, N'Россия', '2026-10-15', '2026-11-15'),
(N'Кофемашина DeLonghi', 3, N'США', '2026-11-01', '2026-12-31');
