using EMS.Model;
using EMS.Model.Reports;
using EMS.Model.Reports.ReportModels;
using System;
using System.Linq;

namespace EMS.Presentation.Menus
{
    public class StudentMenu
    {
        private readonly IDataStore _dataStore;
        private readonly StudentService _studentService;
        private readonly StudentReportService _studentReportService;

        public StudentMenu(
            IDataStore dataStore,
            StudentService studentService,
            StudentReportService studentReportService)
        {
            _dataStore = dataStore;
            _studentService = studentService;
            _studentReportService = studentReportService;
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
    }
}