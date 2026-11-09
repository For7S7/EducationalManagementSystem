using System;
using System.Linq;
using EMS.Model;

namespace EMS.Presentation.Menus
{
    public static class EntitySelector
    {
        public static Student? SelectStudent(IDataStore dataStore)
        {
            var students = dataStore.Students.ToList();

            if (!students.Any())
            {
                Console.WriteLine("\nNo students available.");
                return null;
            }

            Console.WriteLine("\nSelect a student:");

            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i].FullName}");
            }

            Console.Write("Enter number: ");

            if (!int.TryParse(Console.ReadLine(), out int index) ||
                index < 1 || index > students.Count)
            {
                Console.WriteLine("Invalid selection.");
                return null;
            }

            return students[index - 1];
        }

        public static Teacher? SelectTeacher(IDataStore dataStore)
        {
            var teachers = dataStore.Teachers.ToList();

            if (!teachers.Any())
            {
                Console.WriteLine("\nNo teachers available.");
                return null;
            }

            Console.WriteLine("\nSelect a teacher:");

            for (int i = 0; i < teachers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {teachers[i].FullName}");
            }

            Console.Write("Enter number: ");

            if (!int.TryParse(Console.ReadLine(), out int index) ||
                index < 1 || index > teachers.Count)
            {
                Console.WriteLine("Invalid selection.");
                return null;
            }

            return teachers[index - 1];
        }

        public static Subject? SelectSubject(IDataStore dataStore)
        {
            var subjects = dataStore.Subjects.ToList();

            if (!subjects.Any())
            {
                Console.WriteLine("\nNo subjects available.");
                return null;
            }

            Console.WriteLine("\nSelect a subject:");

            for (int i = 0; i < subjects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {subjects[i].Name}");
            }

            Console.Write("Enter number: ");

            if (!int.TryParse(Console.ReadLine(), out int index) ||
                index < 1 || index > subjects.Count)
            {
                Console.WriteLine("Invalid selection.");
                return null;
            }

            return subjects[index - 1];
        }

        public static Grade? SelectGrade(IDataStore dataStore)
        {
            var grades = dataStore.Grades.ToList();

            if (!grades.Any())
            {
                Console.WriteLine("\nNo grades available.");
                return null;
            }

            Console.WriteLine("\nSelect a grade:");

            for (int i = 0; i < grades.Count; i++)
            {
                Console.WriteLine(
                    $"{i + 1}. {grades[i].Student.FullName} - " +
                    $"{grades[i].Subject.Name} - {grades[i].NumericValue}");
            }

            Console.Write("Enter number: ");

            if (!int.TryParse(Console.ReadLine(), out int index) ||
                index < 1 || index > grades.Count)
            {
                Console.WriteLine("Invalid selection.");
                return null;
            }

            return grades[index - 1];
        }
    }
}
