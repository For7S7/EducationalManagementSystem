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
                Console.WriteLine("3. Update teacher");
                Console.WriteLine("4. Delete teacher");
                Console.WriteLine("5. Assign subject");
                Console.WriteLine("6. Remove subject");
                Console.WriteLine("7. View teacher's subjects");

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
                            UpdateTeacher();
                            break;
                        case "4":
                            DeleteTeacher();
                            break;
                        case "5":
                            AssignSubject();
                            break;
                        case "6":
                            RemoveSubject();
                            break;
                        case "7":
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
                    $"Department: {teacher.Department} | " +
                    $"Position: {GetPositionName(teacher.Position)}");
            }
        }

        private void AddTeacher()
        {
            Console.WriteLine("\n-------- ADD TEACHER --------");

            Console.Write("Full name: ");
            string fullName = Console.ReadLine() ?? "";

            Console.Write("Department (leave empty if not assigned): ");
            string department = Console.ReadLine() ?? "";

            TeacherPosition position = SelectPosition();

            Teacher teacher = new Teacher
            {
                FullName = fullName,
                Department = department,
                Position = position
            };

            _teacherService.AddTeacher(teacher);

            Console.WriteLine("\nTeacher added successfully.");
        }

        private void UpdateTeacher()
        {
            Console.WriteLine("\n-------- UPDATE TEACHER --------");

            Teacher? teacher = EntitySelector.SelectTeacher(_dataStore);

            if (teacher == null)
                return;

            Console.WriteLine("Leave a field empty to keep its current value.");

            Console.Write($"Full name [{teacher.FullName}]: ");
            string? fullNameInput = Console.ReadLine();
            string fullName = string.IsNullOrWhiteSpace(fullNameInput)
                ? teacher.FullName
                : fullNameInput.Trim();

            Console.Write($"Department [{teacher.Department}] (enter = keep, '-' = remove): ");
            string? departmentInput = Console.ReadLine();
            string department = string.IsNullOrWhiteSpace(departmentInput)
                ? teacher.Department
                : departmentInput.Trim() == "-" ? string.Empty : departmentInput.Trim();

            TeacherPosition position = SelectPosition(teacher.Position);

            Teacher updatedTeacher = new Teacher
            {
                Id = teacher.Id,
                FullName = fullName,
                Department = department,
                Position = position
            };

            bool result = _teacherService.UpdateTeacher(updatedTeacher);

            Console.WriteLine(result
                ? "\nTeacher updated successfully."
                : "\nTeacher could not be updated.");
        }

        private TeacherPosition SelectPosition(TeacherPosition? currentPosition = null)
        {
            while (true)
            {
                Console.WriteLine("\nSelect teacher position:");
                Console.WriteLine("1. Assistant");
                Console.WriteLine("2. Associate professor");
                Console.WriteLine("3. Professor");

                if (currentPosition.HasValue)
                    Console.WriteLine($"Current: {GetPositionName(currentPosition)} (press Enter to keep)");

                Console.Write("Select an option: ");
                string? choice = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(choice) && currentPosition.HasValue)
                    return currentPosition.Value;

                switch (choice)
                {
                    case "1":
                        return TeacherPosition.Assistant;
                    case "2":
                        return TeacherPosition.AssociateProfessor;
                    case "3":
                        return TeacherPosition.Professor;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private static string GetPositionName(TeacherPosition? position)
        {
            return position switch
            {
                TeacherPosition.Assistant => "Assistant",
                TeacherPosition.AssociateProfessor => "Associate professor",
                TeacherPosition.Professor => "Professor",
                _ => "Not specified"
            };
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
