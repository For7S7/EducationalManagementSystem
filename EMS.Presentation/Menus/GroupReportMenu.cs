using EMS.Model;
using EMS.Model.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Presentation.Menus
{
    public class GroupReportMenu
    {
        private readonly IDataStore _dataStore;
        private readonly GroupReportService _reportService;

        public GroupReportMenu(
            IDataStore dataStore,
            GroupReportService reportService)
        {
            _dataStore = dataStore
                ?? throw new ArgumentNullException(nameof(dataStore));

            _reportService = reportService
                ?? throw new ArgumentNullException(nameof(reportService));
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();

                Console.WriteLine("----------------------------------");
                Console.WriteLine("         GROUP REPORTS");
                Console.WriteLine("----------------------------------\n");

                Console.WriteLine("1. Group semester statistics");
                Console.WriteLine("2. Failed students by subject");
                Console.WriteLine("3. Student ranking by average grade");

                Console.WriteLine("\n0. Back");

                Console.Write("\nSelect an option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        ShowGroupSemesterStatistics();
                        break;

                    case "2":
                        ShowFailedStudentsReport();
                        break;

                    case "3":
                        ShowGroupStudentRanking();
                        break;

                    case "0":
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("\nInvalid option.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowGroupSemesterStatistics()
        {
            Console.Clear();

            Console.WriteLine("=== GROUP SEMESTER STATISTICS ===\n");

            string group = SelectGroup();
            AcademicYear academicYear = SelectAcademicYear();
            int semester = SelectSemester();

            var report = _reportService.GenerateGroupSemesterStatistics(
                group,
                academicYear,
                semester);

            Console.WriteLine("\n=== REPORT ===");
            Console.WriteLine($"Group: {report.Group}");
            Console.WriteLine($"Academic year: {report.AcademicYear}");
            Console.WriteLine($"Semester: {report.Semester}");
            Console.WriteLine($"Students with grades: {report.StudentCount}");

            Console.WriteLine(
                $"Group average: {report.GroupAverage?.ToString("F2") ?? "N/A"}");

            Console.ReadKey();
        }

        private void ShowGroupStudentRanking()
        {
            Console.Clear();

            Console.WriteLine("=== GROUP STUDENT RANKING ===\n");

            string group = SelectGroup();
            var ranking = _reportService.GenerateGroupStudentRanking(group);

            Console.WriteLine("\n=== RANKING ===");
            Console.WriteLine($"Group: {group}");
            Console.WriteLine("Average is calculated using all recorded grades across all semesters and academic years.");

            if (!ranking.Any())
            {
                Console.WriteLine("No students with grades were found in this group.");
            }
            else
            {
                Console.WriteLine("Place | Student | Number of grades | Average grade");
                Console.WriteLine("--------------------------------------------------");

                foreach (var student in ranking)
                {
                    Console.WriteLine(
                        $"{student.Rank} | " +
                        $"{student.StudentName} | " +
                        $"{student.GradeCount} | " +
                        $"{student.AverageGrade:F2}");
                }

                Console.WriteLine($"\nTotal ranked students: {ranking.Count}");
            }

            Console.ReadKey();
        }

        private void ShowFailedStudentsReport()
        {
            Console.Clear();

            Console.WriteLine("=== FAILED STUDENTS REPORT ===\n");

            string group = SelectGroup();
            AcademicYear academicYear = SelectAcademicYear();
            int semester = SelectSemester();

            var subjects = _dataStore.Subjects
                .Where(subject => subject.Semester == semester)
                .OrderBy(subject => subject.Name)
                .ToList();

            if (!subjects.Any())
            {
                Console.WriteLine("\nNo subjects found for this semester.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nAvailable subjects:");

            for (int i = 0; i < subjects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {subjects[i].Name}");
            }

            int subjectChoice = ReadInt(
                "\nSelect a subject: ",
                1,
                subjects.Count);

            var selectedSubject = subjects[subjectChoice - 1];

            var failedStudents = _reportService.GenerateFailedStudentsReport(
                group,
                selectedSubject,
                academicYear,
                semester);

            Console.WriteLine("\n=== REPORT ===");
            Console.WriteLine($"Group: {group}");
            Console.WriteLine($"Academic year: {academicYear}");
            Console.WriteLine($"Semester: {semester}");
            Console.WriteLine($"Subject: {selectedSubject.Name}");

            if (!failedStudents.Any())
            {
                Console.WriteLine("\nNo students with academic debt found.");
            }
            else
            {
                Console.WriteLine("\nStudents with academic debt:");

                foreach (var student in failedStudents)
                {
                    Console.WriteLine(
                        $"{student.StudentName} | " +
                        $"{student.Group} | " +
                        $"Average: {student.AverageGrade:F2}");
                }

                Console.WriteLine(
                    $"\nTotal: {failedStudents.Count}");
            }

            Console.ReadKey();
        }

        private string SelectGroup()
        {
            var groups = _dataStore.Students.Select(student => student.Group).Distinct().OrderBy(group => group).ToList();

            if (!groups.Any())
                throw new InvalidOperationException("No student groups found.");

            Console.WriteLine("Available groups:");

            for (int i = 0; i < groups.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {groups[i]}");
            }

            int choice = ReadInt("\nSelect a group: ", 1, groups.Count);

            return groups[choice - 1];
        }

        private AcademicYear SelectAcademicYear()
        {
            int startYear = ReadInt(
                "\nEnter academic year start (e.g. 2026 for 2026/2027): ",
                1900,
                9998);

            return new AcademicYear(startYear);
        }

        private int SelectSemester()
        {
            return ReadInt("\nEnter semester (1-8): ", 1, 8);
        }

        private int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);

                if (int.TryParse(Console.ReadLine(), out int value) &&
                    value >= min && value <= max)
                {
                    return value;
                }

                Console.WriteLine(
                    $"Enter a number between {min} and {max}.");
            }
        }
    }
}
