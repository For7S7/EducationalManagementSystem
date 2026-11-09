using System;
using System.Linq;
using EMS.Model;

namespace EMS.Presentation.Menus
{
    public class TeacherMenu
    {
        private readonly IDataStore _dataStore;
        private readonly TeacherService _teacherService;

        public TeacherMenu(
            IDataStore dataStore,
            TeacherService teacherService)
        {
            _dataStore = dataStore;
            _teacherService = teacherService;
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();

                Console.WriteLine("-------- TEACHER MANAGEMENT --------\n");

                Console.WriteLine("1. View all teachers");
                Console.WriteLine("2. Add teacher");
                Console.WriteLine("3. Delete teacher");
                Console.WriteLine("4. Assign subject");
                Console.WriteLine("5. Remove subject");
                Console.WriteLine("6. View teacher's subjects");

                Console.WriteLine("\n0. Back");

                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ViewTeachers();
                            break;

                        case "2":
                            AddTeacher();
                            break;

                        case "3":
                            DeleteTeacher();
                            break;

                        case "4":
                            AssignSubject();
                            break;

                        case "5":
                            RemoveSubject();
                            break;

                        case "6":
                            ViewSubjects();
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

        private void ViewTeachers()
        {
            Console.WriteLine("\n-------- TEACHERS --------");

            if (!_dataStore.Teachers.Any())
            {
                Console.WriteLine("No teachers found.");
                return;
            }

            foreach (Teacher teacher in _dataStore.Teachers)
            {
                Console.WriteLine(
                    $"ID: {teacher.Id} | Name: {teacher.FullName} | " +
                    $"Department: {teacher.Department}");
            }
        }

        private void AddTeacher()
        {
            Console.WriteLine("\n-------- ADD TEACHER --------");

            Console.Write("Full name: ");
            string fullName = Console.ReadLine() ?? "";

            Console.Write("Department: ");
            string department = Console.ReadLine() ?? "";

            Teacher teacher = new Teacher
            {
                FullName = fullName,
                Department = department
            };

            _teacherService.AddTeacher(teacher);

            Console.WriteLine("\nTeacher added successfully.");
        }

        private void DeleteTeacher()
        {
            Teacher? teacher = EntitySelector.SelectTeacher(_dataStore);

            if (teacher == null)
                return;

            bool result = _teacherService.DeleteTeacher(teacher);

            Console.WriteLine(result
                ? "\nTeacher deleted successfully."
                : "\nTeacher could not be deleted.");
        }

        private void AssignSubject()
        {
            Teacher? teacher = EntitySelector.SelectTeacher(_dataStore);

            if (teacher == null)
                return;

            Subject? subject = EntitySelector.SelectSubject(_dataStore);

            if (subject == null)
                return;

            _teacherService.AssignSubject(teacher, subject);

            Console.WriteLine("\nSubject assigned successfully.");
        }

        private void RemoveSubject()
        {
            Teacher? teacher = EntitySelector.SelectTeacher(_dataStore);

            if (teacher == null)
                return;

            Subject? subject = EntitySelector.SelectSubject(_dataStore);

            if (subject == null)
                return;

            bool result = _teacherService.RemoveSubject(teacher, subject);

            Console.WriteLine(result
                ? "\nSubject removed successfully."
                : "\nSubject was not assigned to this teacher.");
        }

        private void ViewSubjects()
        {
            Teacher? teacher = EntitySelector.SelectTeacher(_dataStore);

            if (teacher == null)
                return;

            var subjects = _teacherService.GetSubjects(teacher);

            Console.WriteLine($"\nSubjects of {teacher.FullName}:");

            if (!subjects.Any())
            {
                Console.WriteLine("No subjects assigned.");
                return;
            }

            foreach (Subject subject in subjects)
            {
                Console.WriteLine(
                    $"{subject.Name} | Semester: {subject.Semester} | Credits: {subject.Credits}");
            }
        }
    }
}