using System;
using System.Linq;
using EMS.Model;

namespace EMS.Presentation.Menus
{
    public class GradeMenu
    {
        private readonly IDataStore _dataStore;
        private readonly GradeService _gradeService;

        public GradeMenu(
            IDataStore dataStore,
            GradeService gradeService)
        {
            _dataStore = dataStore;
            _gradeService = gradeService;
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();

                Console.WriteLine("-------- GRADE MANAGEMENT --------\n");

                Console.WriteLine("1. View all grades");
                Console.WriteLine("2. Add grade");
                Console.WriteLine("3. Update grade");
                Console.WriteLine("4. Delete grade");
                Console.WriteLine("5. View student's grades");

                Console.WriteLine("\n0. Back");

                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ViewGrades();
                            break;

                        case "2":
                            AddGrade();
                            break;

                        case "3":
                            UpdateGrade();
                            break;

                        case "4":
                            DeleteGrade();
                            break;

                        case "5":
                            ViewStudentGrades();
                            break;

                        case "0":
                            isRunning = false;
                            break;

                        default:
                            Console.WriteLine("\nInvalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                }

                if (isRunning)
                    Console.ReadKey();
            }
        }

        private void ViewGrades()
        {
            Console.WriteLine("\n-------- GRADES --------");

            if (!_dataStore.Grades.Any())
            {
                Console.WriteLine("No grades found.");
                return;
            }

            foreach (Grade grade in _dataStore.Grades)
            {
                Console.WriteLine(
                    $"ID: {grade.Id} | Student: {grade.Student.FullName} | " +
                    $"Teacher: {grade.Teacher.FullName} | Subject: {grade.Subject.Name} | " +
                    $"Grade: {grade.NumericValue} ({grade.LetterValue}) | Date: {grade.Date:d}");
            }
        }

        private void AddGrade()
        {
            Console.WriteLine("\n-------- ADD GRADE --------");

            Student? student = EntitySelector.SelectStudent(_dataStore);

            if (student == null)
                return;

            Teacher? teacher = EntitySelector.SelectTeacher(_dataStore);

            if (teacher == null)
                return;

            Subject? subject = EntitySelector.SelectSubject(_dataStore);

            if (subject == null)
                return;

            Console.Write("Numeric grade (0-100): ");
            int numericValue = int.Parse(Console.ReadLine() ?? "");

            _gradeService.AddGrade(
                student,
                teacher,
                subject,
                numericValue,
                DateTime.Now);

            Console.WriteLine("\nGrade added successfully.");
        }

        private void UpdateGrade()
        {
            Grade? grade = EntitySelector.SelectGrade(_dataStore);

            if (grade == null)
                return;

            Console.Write("\nNew numeric grade (0-100): ");
            int newValue = int.Parse(Console.ReadLine() ?? "");

            _gradeService.UpdateGrade(grade, newValue);

            Console.WriteLine("\nGrade updated successfully.");
        }

        private void DeleteGrade()
        {
            Grade? grade = EntitySelector.SelectGrade(_dataStore);

            if (grade == null)
                return;

            bool result = _gradeService.DeleteGrade(grade);

            Console.WriteLine(result
                ? "\nGrade deleted successfully."
                : "\nGrade could not be deleted.");
        }

        private void ViewStudentGrades()
        {
            Student? student = EntitySelector.SelectStudent(_dataStore);

            if (student == null)
                return;

            var grades = _dataStore.Grades
                .Where(g => g.Student.Id == student.Id)
                .ToList();

            Console.WriteLine($"\nGrades of {student.FullName}:");

            if (!grades.Any())
            {
                Console.WriteLine("No grades found.");
                return;
            }

            foreach (Grade grade in grades)
            {
                Console.WriteLine(
                    $"{grade.Subject.Name} | " +
                    $"{grade.NumericValue} ({grade.LetterValue}) | " +
                    $"{grade.Date:d}");
            }
        }
    }
}