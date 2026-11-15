using EMS.Model;
using EMS.Model.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Presentation.Menus
{
    public class TeacherReportMenu
    {
        private readonly IDataStore _dataStore;
        private readonly TeacherReportService _reportService;

        public TeacherReportMenu(
            IDataStore dataStore,
            TeacherReportService reportService)
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
                Console.WriteLine("         TEACHER REPORTS");
                Console.WriteLine("----------------------------------\n");

                Console.WriteLine("1. Subject performance report");
                Console.WriteLine("2. Semester grading statistics");

                Console.WriteLine("\n0. Back");

                Console.Write("\nSelect an option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        ShowTeacherSubjectReport();
                        break;

                    case "2":
                        ShowTeacherSemesterStatistics();
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

        private void ShowTeacherSubjectReport()
        {
            Console.Clear();

            Console.WriteLine("=== TEACHER SUBJECT PERFORMANCE ===\n");

            Teacher teacher = SelectTeacher();
            AcademicYear academicYear = SelectAcademicYear();
            int semester = SelectSemester();

            var subjects = teacher.Subjects.Where(subject => subject.Semester == semester).OrderBy(subject => subject.Name).ToList();

            if (!subjects.Any())
            {
                Console.WriteLine("\nNo subjects found for this teacher in the selected semester.");
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

            Subject selectedSubject = subjects[subjectChoice - 1];

            var report = _reportService.GenerateTeacherSubjectReport(
                teacher,
                selectedSubject,
                academicYear,
                semester);

            Console.WriteLine("\n=== REPORT ===");
            Console.WriteLine($"Teacher: {report.TeacherName}");
            Console.WriteLine($"Subject: {report.SubjectName}");
            Console.WriteLine($"Academic year: {report.AcademicYear}");
            Console.WriteLine($"Semester: {report.Semester}");
            Console.WriteLine($"Grades issued: {report.GradeCount}");

            Console.WriteLine(
                $"Average grade: {report.AverageGrade?.ToString("F2") ?? "N/A"}");

            Console.ReadKey();
        }

        private Teacher SelectTeacher()
        {
            var teachers = _dataStore.Teachers.OrderBy(teacher => teacher.FullName).ToList();

            if (!teachers.Any())
                throw new InvalidOperationException("No teachers found.");

            Console.WriteLine("Available teachers:");

            for (int i = 0; i < teachers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {teachers[i].FullName}");
            }

            int choice = ReadInt(
                "\nSelect a teacher: ",
                1,
                teachers.Count);

            return teachers[choice - 1];
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

        private void ShowTeacherSemesterStatistics()
        {
            Console.Clear();

            Console.WriteLine("=== TEACHER SEMESTER STATISTICS ===\n");

            Teacher teacher = SelectTeacher();
            AcademicYear academicYear = SelectAcademicYear();
            int semester = SelectSemester();

            var report = _reportService.GenerateTeacherSemesterStatistics(
                teacher,
                academicYear,
                semester);

            Console.WriteLine("\n=== REPORT ===");
            Console.WriteLine($"Teacher: {report.TeacherName}");
            Console.WriteLine($"Academic year: {report.AcademicYear}");
            Console.WriteLine($"Semester: {report.Semester}");
            Console.WriteLine($"Total grades issued: {report.GradeCount}");

            Console.ReadKey();
        }
    }
}
