using System;
using System.Linq;
using EMS.Model;

namespace EMS.Presentation.Menus
{
    public class StudentMenu
    {
        private readonly IDataStore _dataStore;
        private readonly StudentService _studentService;

        public StudentMenu(
            IDataStore dataStore,
            StudentService studentService)
        {
            _dataStore = dataStore;
            _studentService = studentService;
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();

                Console.WriteLine("-------- STUDENT MANAGEMENT --------\n");

                Console.WriteLine("1. View all students");
                Console.WriteLine("2. Add student");
                Console.WriteLine("3. Delete student");
                Console.WriteLine("4. Assign subject");
                Console.WriteLine("5. Remove subject");
                Console.WriteLine("6. View student's subjects");

                Console.WriteLine("\n0. Back");

                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ViewStudents();
                            break;

                        case "2":
                            AddStudent();
                            break;

                        case "3":
                            DeleteStudent();
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

        private void ViewStudents()
        {
            Console.WriteLine("\n--------- STUDENTS ---------");

            if (!_dataStore.Students.Any())
            {
                Console.WriteLine("No students found.");
                return;
            }

            foreach (Student student in _dataStore.Students)
            {
                Console.WriteLine(
                    $"ID: {student.Id} | Name: {student.FullName} | " +
                    $"Group: {student.Group} | Admission Year: {student.AdmissionYear}");
            }
        }

        private void AddStudent()
        {
            Console.WriteLine("\n--------- ADD STUDENT ---------");

            Console.Write("Full name: ");
            string fullName = Console.ReadLine() ?? "";

            Console.Write("Group: ");
            string group = Console.ReadLine() ?? "";

            Console.Write("Admission year: ");
            int admissionYear = int.Parse(Console.ReadLine() ?? "");

            Student student = new Student
            {
                FullName = fullName,
                Group = group,
                AdmissionYear = admissionYear
            };

            _studentService.AddStudent(student);

            Console.WriteLine("\nStudent added successfully.");
        }

        private void DeleteStudent()
        {
            Student? student = EntitySelector.SelectStudent(_dataStore);

            if (student == null)
                return;

            bool result = _studentService.DeleteStudent(student);

            Console.WriteLine(result
                ? "\nStudent deleted successfully."
                : "\nStudent could not be deleted.");
        }

        private void AssignSubject()
        {
            Student? student = EntitySelector.SelectStudent(_dataStore);

            if (student == null)
                return;

            Subject? subject = EntitySelector.SelectSubject(_dataStore);

            if (subject == null)
                return;

            _studentService.AssignSubject(student, subject);

            Console.WriteLine("\nSubject assigned successfully.");
        }

        private void RemoveSubject()
        {
            Student? student = EntitySelector.SelectStudent(_dataStore);

            if (student == null)
                return;

            Subject? subject = EntitySelector.SelectSubject(_dataStore);

            if (subject == null)
                return;

            bool result = _studentService.RemoveSubject(student, subject);

            Console.WriteLine(result
                ? "\nSubject removed successfully."
                : "\nSubject was not assigned to this student.");
        }

        private void ViewSubjects()
        {
            Student? student = EntitySelector.SelectStudent(_dataStore);

            if (student == null)
                return;

            var subjects = _studentService.GetSubjects(student);

            Console.WriteLine($"\nSubjects of {student.FullName}:");

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