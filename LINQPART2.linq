<Query Kind="Program" />

void Main()
{
"исходное состояние".Dump();
Database.Countries.Dump("Страны");
Database.Capitals.Dump("Столицы");
Database.Cities.Dump("Города");

//задание 1
"задание 1: добавленеи".Dump();
var japan = new Country { Name = "Япония", PartOfWorld = "Азия", Area = 377_975, Population = 125_000_000 };
Repository.AddCountry(japan).Dump("AddCountry(Япония)");
Repository.AddCapital(new Capital { Name = "Токио", CountryName = "Япония", Population = 13_900_000 }).Dump("AddCapital(Токио)");
Repository.AddCity(new City { Name = "Токио", CountryName = "Япония", Population = 13_900_000 }).Dump("AddCity(Токио)");
Repository.AddCity(new City { Name = "Осака", CountryName = "Япония", Population = 2_700_000 }).Dump("AddCity(Осака)");
Repository.AddCity(new City { Name = "Нагоя", CountryName = "Япония", Population = 2_300_000 }).Dump("AddCity(Нагоя)");
Repository.AddCountry(new Country { Name = "Япония" }).Dump("AddCountry(Япония) — дубликат, должно быть false");
Repository.AddCity(new City { Name = "Токио", CountryName = "Несуществующая" }).Dump("AddCity с несуществующей страной — false");
Database.Countries.Dump("Страны после добавления");
Database.Capitals.Dump("Столицы после добавления");
Database.Cities.Dump("Города после добавления");

//задание 2
"Задание 2: обновление".Dump();
Repository.UpdateCountry("Япония", new Country { Name = "Япония", PartOfWorld = "Азия", Area = 377_975, Population = 124_000_000 }).Dump("UpdateCountry(Япония) — полное обновление");
Repository.UpdateCapital("Токио", new Capital { Name = "Токио", CountryName = "Япония", Population = 14_000_000 }).Dump("UpdateCapital(Токио)");
Repository.UpdateCity("Осака", new City { Name = "Осака", CountryName = "Япония", Population = 2_750_000 }).Dump("UpdateCity(Осака)");
Repository.UpdateCountryPopulation("Япония", 125_000_000).Dump("UpdateCountryPopulation(Япония, 125 млн)");
Repository.UpdateCapitalPopulation("Токио", 14_100_000).Dump("UpdateCapitalPopulation(Токио, 14.1 млн)");
Repository.UpdateCityPopulation("Осака", 2_720_000).Dump("UpdateCityPopulation(Осака, 2.72 млн)");
Database.Countries.Dump("Страны после обновления");
Database.Capitals.Dump("Столицы после обновления");
Database.Cities.Dump("Города после обновления");

//задание 3
"задание 3 удаление".Dump();
Repository.ClearCityPopulation("Осака").Dump("ClearCityPopulation(Осака) — удаление данных о населении");
Repository.DeleteCity("Нагоя").Dump("DeleteCity(Нагоя)");
Repository.DeleteCapital("Токио").Dump("DeleteCapital(Токио)");
Repository.DeleteCountry("Япония").Dump("DeleteCountry(Япония) — каскадное удаление");
Repository.DeleteCountry("Япония").Dump("DeleteCountry(Япония) — повторно, должно быть false");
Database.Countries.Dump("Страны после удаления");
Database.Capitals.Dump("Столицы после удаления");
Database.Cities.Dump("Города после удаления");

//задание 4
"задание 4 хранимые процедуры".Dump();
StoredProcedures.Top3CountriesByPopulation().Dump("Топ-3 стран по населению");
StoredProcedures.Top3CapitalsByPopulation().Dump("Топ-3 столиц по населению");
StoredProcedures.CountryWithMaxPopulation().Dump("Страна с максимальным населением");
StoredProcedures.CityWithMaxPopulation().Dump("Город с максимальным населением");
}

public static class Database
{
public static List<Country> Countries = new List<Country>
{
new Country { Name = "Россия",PartOfWorld = "Европа", Area = 17_098_246, Population = 146_000_000 },
new Country { Name = "Германия", PartOfWorld = "Европа", Area = 357_022, Population = 83_000_000 },
new Country { Name = "Китай", PartOfWorld = "Азия", Area = 9_596_961, Population = 1_412_000_000 },
new Country { Name = "Индия", PartOfWorld = "Азия", Area = 3_287_263, Population = 1_400_000_000 },
};

public static List<Capital> Capitals = new List<Capital>
{
new Capital { Name = "Москва", CountryName = "Россия",Population = 12_500_000 },
new Capital { Name = "Берлин", CountryName = "Германия", Population =  3_700_000 },
new Capital { Name = "Пекин", CountryName = "Китай", Population = 21_500_000 },
new Capital { Name = "Нью-Дели", CountryName = "Индия", Population = 32_000_000 },
};

public static List<City> Cities = new List<City>
{
new City { Name = "Москва", CountryName = "Россия", Population = 12_500_000 },
new City { Name = "Санкт-Петербург", CountryName = "Россия", Population =  5_400_000 },
new City { Name = "Новосибирск", CountryName = "Россия", Population =  1_600_000 },
new City { Name = "Берлин",CountryName = "Германия", Population =  3_700_000 },
new City { Name = "Мюнхен", CountryName = "Германия", Population =  1_500_000 },
new City { Name = "Пекин", CountryName = "Китай", Population = 21_500_000 },
new City { Name = "Шанхай", CountryName = "Китай", Population = 24_900_000 },
new City { Name = "Нью-Дели", CountryName = "Индия", Population = 32_000_000 },
new City { Name = "Мумбаи",  CountryName = "Индия",  Population = 20_400_000 },
};
}

public static class Repository
{
//ДОБАВЛЕНИЕ
public static bool AddCountry(Country c)
{
if (c == null || string.IsNullOrWhiteSpace(c.Name)) return false;
if (Database.Countries.Any(x => x.Name == c.Name)) return false;
Database.Countries.Add(c);
return true;
}

public static bool AddCapital(Capital cap)
{
if (cap == null || string.IsNullOrWhiteSpace(cap.Name)) return false;
if (Database.Capitals.Any(x => x.Name == cap.Name)) return false;
if (!Database.Countries.Any(x => x.Name == cap.CountryName)) return false;
Database.Capitals.Add(cap);
return true;
}

public static bool AddCity(City city)
{
if (city == null || string.IsNullOrWhiteSpace(city.Name)) return false;
if (Database.Cities.Any(x => x.Name == city.Name && x.CountryName == city.CountryName)) return false;
if (!Database.Countries.Any(x => x.Name == city.CountryName)) return false;
Database.Cities.Add(city);
return true;
}

//ОБНОВЛЕНИЕ 
public static bool UpdateCountry(string name, Country updated)
{
var c = Database.Countries.FirstOrDefault(x => x.Name == name);
if (c == null || updated == null) return false;
c.PartOfWorld = updated.PartOfWorld;
c.Area = updated.Area;
c.Population = updated.Population;
return true;
}

public static bool UpdateCapital(string name, Capital updated)
{
var c = Database.Capitals.FirstOrDefault(x => x.Name == name);
if (c == null || updated == null) return false;
if (!Database.Countries.Any(x => x.Name == updated.CountryName)) return false;
c.CountryName = updated.CountryName;
c.Population = updated.Population;
return true;
}

public static bool UpdateCity(string name, City updated)
{
var c = Database.Cities.FirstOrDefault(x => x.Name == name);
if (c == null || updated == null) return false;
if (!Database.Countries.Any(x => x.Name == updated.CountryName)) return false;
c.CountryName = updated.CountryName;
c.Population = updated.Population;
return true;
}

//ОБНОВЛЕНИЕ ТОЛЬКО НАСЕЛЕНИЯ
public static bool UpdateCountryPopulation(string name, long newPopulation)
{
var c = Database.Countries.FirstOrDefault(x => x.Name == name);
if (c == null) return false;
c.Population = newPopulation;
return true;
}

public static bool UpdateCapitalPopulation(string name, long newPopulation)
{
var c = Database.Capitals.FirstOrDefault(x => x.Name == name);
if (c == null) return false;
c.Population = newPopulation;
return true;
}

public static bool UpdateCityPopulation(string name, long newPopulation)
{
var c = Database.Cities.FirstOrDefault(x => x.Name == name);
if (c == null) return false;
c.Population = newPopulation;
return true;
}

//УДАЛЕНИЕ 
public static bool DeleteCountry(string name)
{
var c = Database.Countries.FirstOrDefault(x => x.Name == name);
if (c == null) return false;
Database.Cities.RemoveAll(x => x.CountryName == name);
Database.Capitals.RemoveAll(x => x.CountryName == name);
Database.Countries.Remove(c);
return true;
}

public static bool DeleteCapital(string name)
{
var c = Database.Capitals.FirstOrDefault(x => x.Name == name);
if (c == null) return false;
Database.Capitals.Remove(c);
return true;
}

public static bool DeleteCity(string name)
{
var c = Database.Cities.FirstOrDefault(x => x.Name == name);
if (c == null) return false;
Database.Cities.Remove(c);
return true;
}

//УДАЛЕНИЕ ДАННЫХ О НАСЕЛЕНИИ (обнуление) 
public static bool ClearCountryPopulation(string name)
{
var c = Database.Countries.FirstOrDefault(x => x.Name == name);
if (c == null) return false;
c.Population = 0;
return true;
}

public static bool ClearCapitalPopulation(string name)
{
var c = Database.Capitals.FirstOrDefault(x => x.Name == name);
if (c == null) return false;
c.Population = 0;
return true;
}

public static bool ClearCityPopulation(string name)
{
var c = Database.Cities.FirstOrDefault(x => x.Name == name);
if (c == null) return false;
c.Population = 0;
return true;
}
}

//Хранимые процедуры
public static class StoredProcedures
{
public static List<Country> Top3CountriesByPopulation()
{
return Database.Countries.OrderByDescending(c => c.Population).Take(3).ToList();
}

public static List<Capital> Top3CapitalsByPopulation()
{
return Database.Capitals.OrderByDescending(c => c.Population).Take(3).ToList();
}

public static Country CountryWithMaxPopulation()
{
return Database.Countries.OrderByDescending(c => c.Population).FirstOrDefault();
}

public static City CityWithMaxPopulation()
{
return Database.Cities.OrderByDescending(c => c.Population).FirstOrDefault();
}
}

//Модели 
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