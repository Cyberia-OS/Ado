using System;
using System.Linq;
using System.Data.Entity;

namespace CarRentalDbFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ЛАБОРАТОРНАЯ РАБОТА: АВТОПАРК ===\n");

            Task1_AddInitialData();
            Task2_GetAllCars();
            Task3_FindCarByModel("Camry");
            Task4_GetCarById(1);
            Task5_GetCarsWithBrandInfo();
            Task6_AddNewCar("X5", 2023, 150.00m, 2);
            Task7_UpdateCarPrice(1, 200.00m);

            Console.WriteLine("\nНажмите Enter для выхода...");
            Console.ReadLine();
        }

        static void Task1_AddInitialData()
        {
            Console.WriteLine("--- Добавление начальных данных ---");
            using (var db = new CarRentalDbEntities())
            {
                if (!db.Brands.Any())
                {
                    var toyota = new Brands { Name = "Toyota", Country = "Япония" };
                    var bmw = new Brands { Name = "BMW", Country = "Германия" };
                    db.Brands.Add(toyota);
                    db.Brands.Add(bmw);
                    db.SaveChanges();

                    var camry = new Cars { Model = "Camry", ReleaseYear = 2020, PricePerDay = 50.00m, BrandId = toyota.Id };
                    var corolla = new Cars { Model = "Corolla", ReleaseYear = 2019, PricePerDay = 40.00m, BrandId = toyota.Id };
                    var x5 = new Cars { Model = "X5", ReleaseYear = 2022, PricePerDay = 100.00m, BrandId = bmw.Id };

                    db.Cars.Add(camry);
                    db.Cars.Add(corolla);
                    db.Cars.Add(x5);
                    db.SaveChanges();

                    Console.WriteLine("Данные успешно добавлены!");
                }
                else
                {
                    Console.WriteLine("База данных уже содержит записи.");
                }
            }
        }

        static void Task2_GetAllCars()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 1: Все автомобили по алфавиту ---");
            using (var db = new CarRentalDbEntities())
            {
                var cars = db.Cars.OrderBy(c => c.Model).ToList();
                foreach (var car in cars)
                    Console.WriteLine($"Модель: {car.Model,-10} | Цена за день: {car.PricePerDay:F2}");
            }
        }

        static void Task3_FindCarByModel(string modelName)
        {
            Console.WriteLine($"\n--- ЗАДАНИЕ 2: Поиск машины '{modelName}' ---");
            using (var db = new CarRentalDbEntities())
            {
                var car = db.Cars.FirstOrDefault(c => c.Model == modelName);
                if (car != null)
                    Console.WriteLine($"Найдено: {car.Model}, год выпуска {car.ReleaseYear}");
                else
                    Console.WriteLine("Машина не найдена");
            }
        }

        static void Task4_GetCarById(int id)
        {
            Console.WriteLine($"\n--- ЗАДАНИЕ 3: Поиск по ID ({id}) ---");
            using (var db = new CarRentalDbEntities())
            {
                var car = db.Cars.Find(id);
                if (car != null)
                    Console.WriteLine($"Модель: {car.Model}");
                else
                    Console.WriteLine("Машина не найдена");
            }
        }

        static void Task5_GetCarsWithBrandInfo()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 4: Автомобили с названием марки ---");
            using (var db = new CarRentalDbEntities())
            {
                var cars = db.Cars.Include(c => c.Brands).ToList();
                foreach (var car in cars)
                    Console.WriteLine($"Модель: {car.Model} | Марка: {car.Brands.Name} | Страна: {car.Brands.Country}");
            }
        }

        static void Task6_AddNewCar(string model, int year, decimal price, int brandId)
        {
            Console.WriteLine($"\n--- ЗАДАНИЕ 5: Добавление машины '{model}' ---");
            using (var db = new CarRentalDbEntities())
            {
                var newCar = new Cars
                {
                    Model = model,
                    ReleaseYear = year,
                    PricePerDay = price,
                    BrandId = brandId
                };
                db.Cars.Add(newCar);
                db.SaveChanges();
                Console.WriteLine($"Машина '{model}' успешно добавлена (Id = {newCar.Id}).");
            }
        }

        static void Task7_UpdateCarPrice(int carId, decimal newPrice)
        {
            Console.WriteLine($"\n--- ЗАДАНИЕ 6: Обновление цены машины (ID: {carId}) ---");
            using (var db = new CarRentalDbEntities())
            {
                var car = db.Cars.Find(carId);
                if (car != null)
                {
                    decimal oldPrice = car.PricePerDay;
                    car.PricePerDay = newPrice;
                    db.SaveChanges();
                    Console.WriteLine($"Цена '{car.Model}' изменена: {oldPrice:F2} → {newPrice:F2}");
                }
                else
                {
                    Console.WriteLine("Машина не найдена");
                }
            }
        }
    }
}