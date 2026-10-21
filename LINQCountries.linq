<Query Kind="Program" />

void Main()
{
// задание 1: однотабличная бд "страны"
var countries = new List<Country>
{
new Country { Name = "Россия",  Capital = "Москва",  Population = 146_000_000, Area = 17_098_246, PartOfWorld = "Европа" },
new Country { Name = "Германия", Capital = "Берлин", Population = 83_000_000, Area =  357_022, PartOfWorld = "Европа" },
new Country { Name = "Франция", Capital = "Париж",  Population =  67_000_000, Area =  551_695, PartOfWorld = "Европа" },
new Country { Name = "Италия", Capital = "Рим", Population = 59_000_000, Area = 301_340, PartOfWorld = "Европа" },
new Country { Name = "Испания",  Capital = "Мадрид", Population = 47_000_000, Area =  505_990, PartOfWorld = "Европа" },
new Country { Name = "Польша", Capital = "Варшава", Population = 38_000_000, Area =  312_696, PartOfWorld = "Европа" },
new Country { Name = "Китай",  Capital = "Пекин", Population = 1_412_000_000, Area =  9_596_961, PartOfWorld = "Азия" },
new Country { Name = "Индия", Capital = "Нью-Дели", Population = 1_400_000_000, Area = 3_287_263, PartOfWorld = "Азия" },
new Country { Name = "Япония",  Capital = "Токио",  Population = 125_000_000, Area = 377_975, PartOfWorld = "Азия" },
new Country { Name = "Австралия",Capital = "Канберра", Population = 26_000_000, Area =  7_692_024, PartOfWorld = "Австралия и Океания" },
new Country { Name = "Египет",  Capital = "Каир",  Population = 104_000_000, Area =  1_002_450, PartOfWorld = "Африка" },
new Country { Name = "Нигерия",  Capital = "Абуджа", Population =  206_000_000, Area = 923_768, PartOfWorld = "Африка" },
new Country { Name = "ЮАР", Capital = "Претория",  Population = 60_000_000, Area = 1_221_037, PartOfWorld = "Африка" },
new Country { Name = "США", Capital = "Вашингтон",  Population = 331_000_000, Area = 9_833_517, PartOfWorld = "Северная Америка" },
new Country { Name = "Канада",  Capital = "Оттава",  Population = 38_000_000, Area = 9_984_670, PartOfWorld = "Северная Америка" },
new Country { Name = "Бразилия", Capital = "Бразилиа", Population = 214_000_000, Area =  8_515_767, PartOfWorld = "Южная Америка" },
new Country { Name = "Аргентина", Capital = "Буэнос-Айрес", Population = 45_000_000, Area = 2_780_400, PartOfWorld = "Южная Америка" },
};

// задание 2.1: вся информация о странах
"2.1. вся информация о странах".Dump();
(from c in countries select c).Dump();

// задание 2.2: названия стран
"2.2. названия стран".Dump();
(from c in countries select c.Name).Dump();

// задание 2.3: названия столиц
"2.3. названия столиц".Dump();
(from c in countries select c.Capital).Dump();

// задание 2.4: европейские страны
"2.4. европейские страны".Dump();
(from c in countries
 where c.PartOfWorld == "Европа"
 select c.Name).Dump();

// задание 2.5: страны с площадью больше числа
double minArea = 500_000;
"2.5. страны с площадью > 500 000".Dump();
(from c in countries
 where c.Area > minArea
 select new { c.Name, c.Area }).Dump();

// задание 3.1: в названии есть 'a' и 'u'
"3.1. страны с буквами 'a' и 'u' в названии".Dump();
(from c in countries
 where c.Name.ToLower().Contains('a') && c.Name.ToLower().Contains('u')
 select c.Name).Dump();

// задание 3.2: название начинается с 'a'
"3.2. страны на 'a'".Dump();
(from c in countries
 where c.Name.ToLower().StartsWith("a")
 select c.Name).Dump();

// задание 3.3: площадь в диапазоне
double fromArea = 300_000, toArea = 1_000_000;
"3.3. площадь в диапазоне 300 000 – 1 000 000".Dump();
(from c in countries
 where c.Area >= fromArea && c.Area <= toArea
 select new { c.Name, c.Area }).Dump();

// задание 3.4: население больше числа
long minPop = 100_000_000;
"3.4. население > 100 000 000".Dump();
(from c in countries
 where c.Population > minPop
 select new { c.Name, c.Population }).Dump();

// задание 4.1: топ-5 по площади
"4.1. топ-5 стран по площади".Dump();
(from c in countries
 orderby c.Area descending
 select new { c.Name, c.Area }).Take(5).Dump();

// задание 4.2: топ-5 по населению
"4.2. топ-5 стран по населению".Dump();
(from c in countries
 orderby c.Population descending
 select new { c.Name, c.Population }).Take(5).Dump();

// задание 4.3: страна с самой большой площадью
"4.3. страна с самой большой площадью".Dump();
(from c in countries
 orderby c.Area descending
 select c).First().Dump();

// задание 4.4: страна с самым большим населением
"4.4. страна с самым большим населением".Dump();
(from c in countries
 orderby c.Population descending
 select c).First().Dump();

// задание 4.5: страна с самой маленькой площадью в африке
"4.5. страна с самой маленькой площадью в африке".Dump();
(from c in countries
 where c.PartOfWorld == "Африка"
 orderby c.Area
 select c).First().Dump();

// задание 4.6: средняя площадь стран в азии
"4.6. средняя площадь стран в азии".Dump();
(from c in countries
 where c.PartOfWorld == "Азия"
 select c.Area).Average().Dump();
}

// задание 1: модель
public class Country
{
public string Name { get; set; }
public string Capital { get; set; }
public long Population { get; set; }
public double Area { get; set; }
public string PartOfWorld { get; set; }
}