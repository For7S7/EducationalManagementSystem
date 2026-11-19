using EMS.Model;
using EMS.Model.Reports;
using EMS.Model.Reports.ReportModels;
using System;
using System.Linq;
using System.Globalization;

namespace EMS.Presentation.Menus
{
    public class StudentMenu
    {
        private readonly IDataStore _dataStore;
        private readonly StudentService _studentService;
        private readonly StudentReportService _studentReportService;
        private readonly StudentFilterService _studentFilterService;

        public StudentMenu(
            IDataStore dataStore,
            StudentService studentService,
            StudentReportService studentReportService,
            StudentFilterService studentFilterService)
        {
            _dataStore = dataStore;
            _studentService = studentService;
            _studentReportService = studentReportService;
            _studentFilterService = studentFilterService
                ?? throw new ArgumentNullException(nameof(studentFilterService));
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
                Console.WriteLine("7. View student transcript");
                Console.WriteLine("8. View student's academic record");
                Console.WriteLine("9. Filter students");

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

                        case "7":
                            ViewStudentTranscript();
                            break;

                        case "8":
                            ViewStudentAcademicRecord();
                            break;

                        case "9":
                            FilterStudents();
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

        private void ViewStudentTranscript()
        {
            Console.WriteLine("\n--------- STUDENT TRANSCRIPT ---------");

            Student? student = EntitySelector.SelectStudent(_dataStore);

            if (student == null)
                return;

            Console.Write("\nEnter academic year start (e.g. 2025): ");

            if (!int.TryParse(Console.ReadLine(), out int startYear))
            {
                Console.WriteLine("Invalid academic year.");
                return;
            }

            AcademicYear academicYear = new AcademicYear(startYear);

            Console.Write("Enter semester number (1-8): ");

            if (!int.TryParse(Console.ReadLine(), out int semester))
            {
                Console.WriteLine("Invalid semester number.");
                return;
            }

            StudentTranscript transcript = _studentReportService.GenerateStudentTranscript(
                student,
                academicYear,
                semester);

            Console.WriteLine("\n========== STUDENT TRANSCRIPT ==========");

            Console.WriteLine($"Student: {transcript.StudentName}");
            Console.WriteLine($"Group: {transcript.Group}");
            Console.WriteLine($"Admission year: {transcript.AdmissionYear}");

            Console.WriteLine($"\nAcademic year: {transcript.AcademicYear}");
            Console.WriteLine($"Semester: {transcript.Semester}");

            Console.WriteLine("\n-----------------------------------------------");
            Console.WriteLine($"{"Subject",-22} {"Grades",-18} {"Average",-10}");
            Console.WriteLine("-----------------------------------------------");

            foreach (SubjectTranscriptEntry entry in transcript.Subjects)
            {
                string grades = entry.Grades.Count > 0
                    ? string.Join(", ", entry.Grades)
                    : "No grades";

                string average = entry.Average.HasValue
                    ? entry.Average.Value.ToString("F2")
                    : "-";

                Console.WriteLine(
                    $"{entry.SubjectName,-22} {grades,-18} {average,-10}");
            }

            Console.WriteLine("-----------------------------------------------");

            string semesterAverage = transcript.SemesterAverage.HasValue
                ? transcript.SemesterAverage.Value.ToString("F2")
                : "No grades";

            Console.WriteLine($"Semester average: {semesterAverage}");
        }

        private void ViewStudentAcademicRecord()
        {
            Console.WriteLine("\n-------- STUDENT ACADEMIC RECORD --------");

            var students = _dataStore.Students.ToList();

            if (students.Count == 0)
            {
                Console.WriteLine("\nNo students available.");
                return;
            }

            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i].FullName} ({students[i].Group})");
            }

            Console.Write("\nSelect a student: ");

            if (!int.TryParse(Console.ReadLine(), out int index) ||
                index < 1 || index > students.Count)
            {
                Console.WriteLine("\nInvalid student selection.");
                return;
            }

            Student student = students[index - 1];

            StudentAcademicRecord record =
                _studentReportService.GenerateStudentAcademicRecord(student);

            Console.WriteLine("\n========================================");
            Console.WriteLine("       STUDENT ACADEMIC RECORD");
            Console.WriteLine("========================================");

            Console.WriteLine($"Student: {record.StudentName}");
            Console.WriteLine($"Group: {record.Group}");
            Console.WriteLine($"Admission year: {record.AdmissionYear}");

            foreach (var semester in record.Semesters)
            {
                Console.WriteLine("\n----------------------------------------");
                Console.WriteLine($"Academic year: {semester.AcademicYear}");
                Console.WriteLine($"Semester: {semester.Semester}");
                Console.WriteLine("----------------------------------------");

                foreach (var subject in semester.Subjects)
                {
                    Console.WriteLine($"\nSubject: {subject.SubjectName}");

                    string grades = subject.Grades.Count > 0
                        ? string.Join(", ", subject.Grades)
                        : "No grades";

                    Console.WriteLine($"Grades: {grades}");
                    Console.WriteLine(
                        $"Average: {subject.Average?.ToString("F2") ?? "No grades"}");
                }

                Console.WriteLine(
                    $"\nSemester average: {semester.SemesterAverage?.ToString("F2") ?? "No grades"}");
            }

            Console.WriteLine("\n========================================");
            Console.WriteLine(
                $"Overall average: {record.OverallAverage?.ToString("F2") ?? "No grades"}");
            Console.WriteLine("========================================");
        }

        private void FilterStudents()
        {
            Console.WriteLine("\n-------- FILTER STUDENTS --------");

            if (!_dataStore.Students.Any())
            {
                Console.WriteLine("No students found.");
                return;
            }

            string? group = SelectOptionalGroup();
            int? semester = SelectOptionalSemester();

            double? minAverage = ReadOptionalAverage(
                "Minimum average (0-100, Enter to skip): ");

            double? maxAverage = ReadOptionalAverage(
                "Maximum average (0-100, Enter to skip): ");

            if (minAverage.HasValue && maxAverage.HasValue &&
                minAverage.Value > maxAverage.Value)
            {
                Console.WriteLine("Minimum average cannot be greater than maximum average.");
                return;
            }

            var results = _studentFilterService.FilterStudents(
                group,
                semester,
                minAverage,
                maxAverage);

            Console.WriteLine("\n-------- FILTER RESULTS --------");
            Console.WriteLine($"Group: {group ?? "All groups"}");
            Console.WriteLine($"Semester: {(semester.HasValue ? semester.Value.ToString() : "All semesters")}");
            Console.WriteLine($"Minimum average: {minAverage?.ToString("F2") ?? "Not set"}");
            Console.WriteLine($"Maximum average: {maxAverage?.ToString("F2") ?? "Not set"}");

            if (semester.HasValue)
            {
                Console.WriteLine(
                    "Average is calculated from grades in the selected semester across all academic years.");
            }
            else
            {
                Console.WriteLine(
                    "Average is calculated from all recorded grades across all semesters and academic years.");
            }

            if (!results.Any())
            {
                Console.WriteLine("\nNo students match the selected filters.");
                return;
            }

            Console.WriteLine("\nStudent | Group | Admission year | Grades | Average");
            Console.WriteLine("---------------------------------------------------");

            foreach (StudentFilterResult result in results)
            {
                Console.WriteLine(
                    $"{result.Student.FullName} | " +
                    $"{result.Student.Group} | " +
                    $"{result.Student.AdmissionYear} | " +
                    $"{result.GradeCount} | " +
                    $"{result.AverageGrade?.ToString("F2") ?? "N/A"}");
            }

            Console.WriteLine($"\nStudents found: {results.Count}");
        }

        private string? SelectOptionalGroup()
        {
            var groups = _dataStore.Students
                .Select(student => student.Group)
                .Where(group => !string.IsNullOrWhiteSpace(group))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(group => group, StringComparer.OrdinalIgnoreCase)
                .ToList();

            Console.WriteLine("\nSelect a group:");
            Console.WriteLine("0. All groups");

            for (int i = 0; i < groups.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {groups[i]}");
            }

            int choice = ReadIntegerInRange(
                "\nSelect an option: ",
                0,
                groups.Count);

            return choice == 0 ? null : groups[choice - 1];
        }

        private int? SelectOptionalSemester()
        {
            Console.WriteLine("\nSelect a semester:");
            Console.WriteLine("0. All semesters");

            for (int semester = 1; semester <= 8; semester++)
            {
                Console.WriteLine($"{semester}. Semester {semester}");
            }

            int choice = ReadIntegerInRange(
                "\nSelect an option: ",
                0,
                8);

            return choice == 0 ? null : choice;
        }

        private double? ReadOptionalAverage(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                    return null;

                bool parsed = double.TryParse(
                    input,
                    NumberStyles.Float,
                    CultureInfo.CurrentCulture,
                    out double average);

                if (!parsed)
                {
                    parsed = double.TryParse(
                        input,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out average);
                }

                if (parsed && average >= 0 && average <= 100)
                    return average;

                Console.WriteLine("Enter a number between 0 and 100, or leave the field empty.");
            }
        }

        private int ReadIntegerInRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);

                if (int.TryParse(Console.ReadLine(), out int value) &&
                    value >= min && value <= max)
                {
                    return value;
                }

                Console.WriteLine($"Enter a number between {min} and {max}.");
            }
        }
    }
}