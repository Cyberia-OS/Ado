using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public class Student
{
    public string Name { get; set; }
    public string Group { get; set; }
    public int Age { get; set; }
    public double GPA { get; set; } 
    public int PassedCredits { get; set; } 

    public override string ToString()
    {
        return $"{Name,-10} | {Group,-6} | {Age,2} | {GPA,4:F1} | {PassedCredits,2}";
    }
}

class Program
{

    static void Task2_MethodSyntax()
    {
        var students = GetStudents();

        Console.WriteLine("--- Задание 2: Стипендии (синтаксис методов) ---");

        var result = students
            .Where(s => s.GPA > 3.5)                 
            .OrderByDescending(s => s.GPA)
            .Select(s => new
            {
                FullName = s.Name,
                Scholarship = s.GPA >= 4.7 ? "Ректорская"
                            : s.GPA >= 4.5 ? "Повышенная"
                            : s.GPA >= 4.0 ? "Стандартная"
                            : "Нет"
            });

        foreach (var item in result)
            Console.WriteLine($"{item.FullName}: {item.Scholarship}");
    }

    static void Task1_QuerySyntax()
    {
        var students = GetStudents();

        Console.WriteLine("--- Задание 1: Студенты ИТ-41 с баллом > 3.5 ---");
        Console.WriteLine("Имя | Группа | Возраст | Балл | Зачеты");

        var query = from s in students
                    where s.Group == "ИТ-41" && s.GPA > 3.5
                    orderby s.Age
                    select s;

        foreach (var s in query)
            Console.WriteLine(s); // ToString() уже переопределён
    }

    static void Task3_Aggregation()
    {
        var students = GetStudents();

        Console.WriteLine("--- Задание 3: Статистика ---");

        // Часть А: группировка
        var groups = students
            .GroupBy(s => s.Group)
            .OrderBy(g => g.Key)
            .Select(g => new { Group = g.Key, AvgGPA = g.Average(s => s.GPA) });

        foreach (var g in groups)
            Console.WriteLine($"Группа {g.Group}: Средний балл {g.AvgGPA:F2}");

        // Часть Б: поиск самого старшего с 45 зачётами
        var oldest = students
            .Where(s => s.PassedCredits == 45)
            .OrderByDescending(s => s.Age)
            .FirstOrDefault();

        Console.WriteLine();
        if (oldest != null)
            Console.WriteLine($"Студент с 45 зачетами: {oldest.Name} ({oldest.Age} год)");
        else
            Console.WriteLine("Студентов с 45 зачетами не найдено");
    }

    static void Task4_Xml()
    {
        // Часть А: создание документа
        XDocument doc = new XDocument(
            new XElement("university",
                new XElement("course",
                    new XAttribute("name", "Программирование"),
                    new XElement("student", new XAttribute("name", "Кирилл"), new XAttribute("gpa", "3.9")),
                    new XElement("student", new XAttribute("name", "Лариса"), new XAttribute("gpa", "4.7")),
                    new XElement("student", new XAttribute("name", "Михаил"), new XAttribute("gpa", "3.2")),
                    new XElement("student", new XAttribute("name", "Наталья"), new XAttribute("gpa", "4.1"))
                )
            )
        );
        doc.Save("uni.xml");

        // Часть Б: чтение и фильтрация
        Console.WriteLine("--- Задание 4: Отличники из XML ---");

        XDocument loaded = XDocument.Load("uni.xml");

        var names = loaded
            .Descendants("student")
            .Where(s => (double)s.Attribute("gpa") > 4.0)
            .Select(s => s.Attribute("name").Value);

        foreach (var name in names)
            Console.WriteLine(name);
    }

    // Источник данных для Заданий 1, 2 и 3
    private static List<Student> GetStudents()
    {
        return new List<Student>
        {
            new Student { Name = "Алексей", Group = "ИТ-41", Age = 20, GPA = 4.2, PassedCredits = 40 },
            new Student { Name = "Борис", Group = "ИТ-41", Age = 19, GPA = 3.1, PassedCredits = 35 },
            new Student { Name = "Виктория", Group = "ПИ-42", Age = 21, GPA = 4.8, PassedCredits = 45 },
            new Student { Name = "Григорий", Group = "ИБ-43", Age = 20, GPA = 2.9, PassedCredits = 20 },
            new Student { Name = "Дарья", Group = "ИТ-41", Age = 22, GPA = 4.5, PassedCredits = 42 },
            new Student { Name = "Елена", Group = "ПИ-42", Age = 20, GPA = 3.8, PassedCredits = 38 },
            new Student { Name = "Жанна", Group = "ИБ-43", Age = 23, GPA = 4.9, PassedCredits = 48 },
            new Student { Name = "Игорь", Group = "ИТ-41", Age = 19, GPA = 2.5, PassedCredits = 15 }
        };
    }

    static void Main(string[] args)
    {
        Task1_QuerySyntax();
        Console.WriteLine();
        Task2_MethodSyntax();
        Console.WriteLine();
        Task3_Aggregation();
        Console.WriteLine();
        Task4_Xml();

        Console.WriteLine("\nНажмите Enter для выхода");
        Console.ReadLine();
    }
}