using System;
using System.Linq;
using EMS.Model;
using EMS.Model.Reports;
using EMS.Model.Reports.ReportModels;

namespace EMS.Presentation.Menus
{
    public class DepartmentMenu
    {
        private readonly IDataStore _dataStore;
        private readonly DepartmentService _departmentService;
        private readonly DepartmentReportService _departmentReportService;

        public DepartmentMenu(
            IDataStore dataStore,
            DepartmentService departmentService,
            DepartmentReportService departmentReportService)
        {
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
            _departmentReportService = departmentReportService
                ?? throw new ArgumentNullException(nameof(departmentReportService));
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();

                Console.WriteLine("-------- DEPARTMENT MANAGEMENT --------\n");
                Console.WriteLine("1. View all departments");
                Console.WriteLine("2. Add department");
                Console.WriteLine("3. Rename department");
                Console.WriteLine("4. Delete department");
                Console.WriteLine("5. Assign teacher to department");
                Console.WriteLine("6. Remove teacher from department");
                Console.WriteLine("7. Assign/change department head");
                Console.WriteLine("8. Remove department head");
                Console.WriteLine("9. Department statistics");
                Console.WriteLine("\n0. Back");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ViewDepartments();
                            break;
                        case "2":
                            AddDepartment();
                            break;
                        case "3":
                            RenameDepartment();
                            break;
                        case "4":
                            DeleteDepartment();
                            break;
                        case "5":
                            AssignTeacher();
                            break;
                        case "6":
                            RemoveTeacher();
                            break;
                        case "7":
                            AssignHead();
                            break;
                        case "8":
                            RemoveHead();
                            break;
                        case "9":
                            ShowDepartmentStatistics();
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

        private void ViewDepartments()
        {
            Console.WriteLine("\n-------- DEPARTMENTS --------");

            if (!_dataStore.Departments.Any())
            {
                Console.WriteLine("No departments found.");
                return;
            }

            foreach (Department department in _dataStore.Departments)
            {
                Console.WriteLine($"\nDepartment: {department.Name}");
                Console.WriteLine($"ID: {department.Id}");
                Console.WriteLine($"Head: {department.Head?.FullName ?? "Not assigned"}");
                Console.WriteLine($"Teachers ({department.Teachers.Count}):");

                if (!department.Teachers.Any())
                {
                    Console.WriteLine("  No teachers assigned.");
                    continue;
                }

                foreach (Teacher teacher in department.Teachers)
                {
                    string headMark = department.Head?.Id == teacher.Id ? " (Head)" : "";
                    Console.WriteLine(
                        $"  - {teacher.FullName} | {GetPositionName(teacher.Position)}{headMark}");
                }
            }
        }

        private void AddDepartment()
        {
            Console.WriteLine("\n-------- ADD DEPARTMENT --------");
            Console.Write("Department name: ");
            string name = Console.ReadLine() ?? string.Empty;

            _departmentService.AddDepartment(name);
            Console.WriteLine("\nDepartment added successfully.");
        }

        private void RenameDepartment()
        {
            Department? department = EntitySelector.SelectDepartment(_dataStore);

            if (department == null)
                return;

            Console.Write($"New name [{department.Name}]: ");
            string newName = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(newName))
            {
                Console.WriteLine("Department name was not changed.");
                return;
            }

            _departmentService.UpdateDepartment(department, newName);
            Console.WriteLine("\nDepartment renamed successfully.");
        }

        private void DeleteDepartment()
        {
            Department? department = EntitySelector.SelectDepartment(_dataStore);

            if (department == null)
                return;

            Console.Write($"Delete department '{department.Name}'? (y/n): ");

            if (!string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Deletion cancelled.");
                return;
            }

            bool result = _departmentService.DeleteDepartment(department);
            Console.WriteLine(result
                ? "\nDepartment deleted. Teachers remain in the system without department assignment."
                : "\nDepartment could not be deleted.");
        }

        private void AssignTeacher()
        {
            Department? department = EntitySelector.SelectDepartment(_dataStore);

            if (department == null)
                return;

            Teacher? teacher = EntitySelector.SelectTeacher(_dataStore);

            if (teacher == null)
                return;

            _departmentService.AssignTeacher(department, teacher);
            Console.WriteLine($"\n{teacher.FullName} assigned to '{department.Name}'.");
        }

        private void RemoveTeacher()
        {
            Department? department = EntitySelector.SelectDepartment(_dataStore);

            if (department == null)
                return;

            Teacher? teacher = SelectDepartmentTeacher(department);

            if (teacher == null)
                return;

            bool result = _departmentService.RemoveTeacher(department, teacher);
            Console.WriteLine(result
                ? "\nTeacher removed from department."
                : "\nTeacher was not assigned to this department.");
        }

        private void AssignHead()
        {
            Department? department = EntitySelector.SelectDepartment(_dataStore);

            if (department == null)
                return;

            Teacher? teacher = SelectDepartmentTeacher(department);

            if (teacher == null)
                return;

            _departmentService.SetHead(department, teacher);
            Console.WriteLine($"\n{teacher.FullName} is now the head of '{department.Name}'.");
        }

        private void RemoveHead()
        {
            Department? department = EntitySelector.SelectDepartment(_dataStore);

            if (department == null)
                return;

            bool result = _departmentService.RemoveHead(department);
            Console.WriteLine(result
                ? "\nDepartment head removed."
                : "\nThis department has no assigned head.");
        }

        private Teacher? SelectDepartmentTeacher(Department department)
        {
            var teachers = department.Teachers.ToList();

            if (!teachers.Any())
            {
                Console.WriteLine("\nThis department has no teachers.");
                return null;
            }

            Console.WriteLine($"\nTeachers of '{department.Name}':");

            for (int i = 0; i < teachers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {teachers[i].FullName} ({GetPositionName(teachers[i].Position)})");
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


        private void ShowDepartmentStatistics()
        {
            Console.WriteLine("\n-------- DEPARTMENT STATISTICS --------");

            Department? department = EntitySelector.SelectDepartment(_dataStore);

            if (department == null)
                return;

            DepartmentStatistics report =
                _departmentReportService.GenerateDepartmentStatistics(department);

            Console.WriteLine($"\nDepartment: {report.DepartmentName}");
            Console.WriteLine($"Subjects: {report.SubjectCount}");
            Console.WriteLine($"Students with grades: {report.StudentCount}");
            Console.WriteLine(
                $"Average student grade: {report.AverageGrade?.ToString("F2") ?? "N/A"}");

            Console.WriteLine("\n-------- TOP 5 STUDENTS --------");

            if (!report.TopStudents.Any())
            {
                Console.WriteLine("No student grades found for this department.");
                return;
            }

            for (int i = 0; i < report.TopStudents.Count; i++)
            {
                DepartmentStudentPerformance student = report.TopStudents[i];

                Console.WriteLine(
                    $"{i + 1}. {student.StudentName} | Group: {student.Group} | " +
                    $"Average: {student.AverageGrade:F2} | Grades: {student.GradeCount}");
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
    }
}