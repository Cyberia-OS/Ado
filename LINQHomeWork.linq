<Query Kind="Program" />

void Main()
{
var countries = new List<Country>
{
new Country { Name = "Россия", PartOfWorld = "Европа", Area = 17_098_246, Population = 146_000_000 },
new Country { Name = "Германия", PartOfWorld = "Европа", Area = 357_022, Population =  83_000_000 },
new Country { Name = "Франция", PartOfWorld = "Европа", Area = 551_695, Population =  67_000_000 },
new Country { Name = "Великобритания", PartOfWorld = "Европа", Area = 243_610, Population =  67_000_000 },
new Country { Name = "Италия", PartOfWorld = "Европа", Area = 301_340, Population =  59_000_000 },
new Country { Name = "Испания", PartOfWorld = "Европа", Area = 505_990, Population =  47_000_000 },
new Country { Name = "Польша", PartOfWorld = "Европа", Area = 312_696, Population =  38_000_000 },
};
var capitals = new List<Capital>
{
new Capital { Name = "Москва", CountryName = "Россия",  Population = 12_500_000 },
new Capital { Name = "Берлин", CountryName = "Германия", Population =  3_700_000 },
new Capital { Name = "Париж", CountryName = "Франция", Population =  2_100_000 },
new Capital { Name = "Лондон", CountryName = "Великобритания", Population =  8_900_000 },
new Capital { Name = "Рим", CountryName = "Италия", Population =  2_800_000 },
new Capital { Name = "Мадрид", CountryName = "Испания", Population = 3_200_000 },
new Capital { Name = "Варшава", CountryName = "Польша", Population = 1_800_000 },
};
var cities = new List<City>
{
new City { Name = "Москва", CountryName = "Россия", Population = 12_500_000 },
new City { Name = "Санкт-Петербург", CountryName = "Россия",  Population =  5_400_000 },
new City { Name = "Новосибирск", CountryName = "Россия", Population = 1_600_000 },
new City { Name = "Екатеринбург", CountryName = "Россия", Population = 1_500_000 },
new City { Name = "Казань", CountryName = "Россия", Population =  1_300_000 },
new City { Name = "Берлин",  CountryName = "Германия", Population = 3_700_000 },
new City { Name = "Гамбург", CountryName = "Германия", Population = 1_800_000 },
new City { Name = "Мюнхен", CountryName = "Германия", Population =  1_500_000 },
new City { Name = "Кёльн", CountryName = "Германия", Population = 1_100_000 },
new City { Name = "Париж", CountryName = "Франция",  Population = 2_100_000 },
new City { Name = "Марсель", CountryName = "Франция", Population = 870_000 },
new City { Name = "Лион", CountryName = "Франция",  Population = 520_000 },
new City { Name = "Тулуза", CountryName = "Франция", Population = 500_000 },
new City { Name = "Лондон", CountryName = "Великобритания", Population =  8_900_000 },
new City { Name = "Бирмингем", CountryName = "Великобритания", Population =  1_100_000 },
new City { Name = "Манчестер", CountryName = "Великобритания", Population =    550_000 },
new City { Name = "Глазго", CountryName = "Великобритания", Population =    630_000 },
};

//задание 2
"=== Задание 2 ===".Dump();
"1. Вся информация о странах".Dump();
countries.Dump();
"2. Названия стран".Dump();
countries.Select(c => c.Name).Dump();
"3. Названия столиц".Dump();
capitals.Select(c => c.Name).Dump();
"4. Крупные города Германии".Dump();
cities.Where(c => c.CountryName == "Германия").Select(c => new { c.Name, c.Population }).Dump();
"5. Столицы с населением > 5 млн".Dump();
capitals.Where(c => c.Population > 5_000_000).Select(c => new { c.Name, c.Population }).Dump();
"6. Европейские страны".Dump();
countries.Where(c => c.PartOfWorld == "Европа").Select(c => c.Name).Dump();
"7. Страны с площадью > 500 000 км²".Dump();
double areaThreshold = 500_000;
countries.Where(c => c.Area > areaThreshold).Select(c => new { c.Name, c.Area }).Dump();

//задание 3
"=== Задание 3 ===".Dump();
"1. Столицы, в названии которых есть 'а' и 'р'".Dump();
capitals.Where(c => c.Name.ToLower().Contains('а') && c.Name.ToLower().Contains('р')).Select(c => c.Name).Dump();
"2. Столицы на 'к'".Dump();
capitals.Where(c => c.Name.ToLower().StartsWith("к")).Select(c => c.Name).Dump();
"3. Страны с площадью 300 000 – 1 000 000".Dump();
double minArea = 300_000, maxArea = 1_000_000;
countries.Where(c => c.Area >= minArea && c.Area <= maxArea).Select(c => new { c.Name, c.Area }).Dump();
"4. Страны с населением > 100 млн".Dump();
long minPop = 100_000_000;
countries.Where(c => c.Population > minPop).Select(c => new { c.Name, c.Population }).Dump();

//задание 4
"=== Задание 4 ===".Dump();
"1. Топ-5 стран по площади".Dump();
countries.OrderByDescending(c => c.Area).Take(5).Select(c => new { c.Name, c.Area }).Dump();
"2. Топ-5 столиц по жителям".Dump();
capitals.OrderByDescending(c => c.Population).Take(5).Select(c => new { c.Name, c.Population }).Dump();
"3. Страна с самой большой площадью".Dump();
countries.OrderByDescending(c => c.Area).First().Dump();
"4. Столица с самым большим населением".Dump();
capitals.OrderByDescending(c => c.Population).First().Dump();
"5. Самая маленькая страна в Европе".Dump();
countries.Where(c => c.PartOfWorld == "Европа").OrderBy(c => c.Area).First().Dump();
"6. Средняя площадь стран в Европе".Dump();
countries.Where(c => c.PartOfWorld == "Европа").Average(c => c.Area).Dump();
"7. Топ-3 города России".Dump();
cities.Where(c => c.CountryName == "Россия").OrderByDescending(c => c.Population).Take(3).Select(c => new { c.Name, c.Population }).Dump();
"8. Общее количество стран".Dump();
countries.Count().Dump();
"9. Часть света с максимальным количеством стран".Dump();
countries.GroupBy(c => c.PartOfWorld).OrderByDescending(g => g.Count()).First().Key.Dump();
"10. Количество стран в каждой части света".Dump();
countries.GroupBy(c => c.PartOfWorld).Select(g => new { Часть_света = g.Key, Количество = g.Count() }).Dump();
}

public class Country
{
public string Name { get; set; }
public string PartOfWorld { get; set; }
public double Area { get; set; }
public long Population { get; set; }
}

public class Capital
{
public string Name { get; set; }
public string CountryName { get; set; }
public long Population { get; set; }
}

public class City
{
public string Name { get; set; }
public string CountryName { get; set; }
public long Population { get; set; }
}