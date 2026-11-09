using System;
using System.Linq;
using EMS.Model;

namespace EMS.Presentation.Menus
{
    public class SubjectMenu
    {
        private readonly IDataStore _dataStore;
        private readonly SubjectService _subjectService;

        public SubjectMenu(
            IDataStore dataStore,
            SubjectService subjectService)
        {
            _dataStore = dataStore;
            _subjectService = subjectService;
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();

                Console.WriteLine("-------- SUBJECT MANAGEMENT --------\n");

                Console.WriteLine("1. View all subjects");
                Console.WriteLine("2. Add subject");
                Console.WriteLine("3. Update subject");
                Console.WriteLine("4. Delete subject");

                Console.WriteLine("\n0. Back");

                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ViewSubjects();
                            break;

                        case "2":
                            AddSubject();
                            break;

                        case "3":
                            UpdateSubject();
                            break;

                        case "4":
                            DeleteSubject();
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

        private void ViewSubjects()
        {
            Console.WriteLine("\n-------- SUBJECTS --------");

            var subjects = _subjectService.GetAllSubjects();

            if (!subjects.Any())
            {
                Console.WriteLine("No subjects found.");
                return;
            }

            foreach (Subject subject in subjects)
            {
                Console.WriteLine(
                    $"ID: {subject.Id} | Name: {subject.Name} | " +
                    $"Semester: {subject.Semester} | Credits: {subject.Credits}");
            }
        }

        private void AddSubject()
        {
            Console.WriteLine("\n-------- ADD SUBJECT --------");

            Console.Write("Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Semester: ");
            int semester = int.Parse(Console.ReadLine() ?? "");

            Console.Write("Credits: ");
            int credits = int.Parse(Console.ReadLine() ?? "");

            Subject subject = new Subject
            {
                Name = name,
                Semester = semester,
                Credits = credits
            };

            _subjectService.AddSubject(subject);

            Console.WriteLine("\nSubject added successfully.");
        }

        private void UpdateSubject()
        {
            Subject? subject = EntitySelector.SelectSubject(_dataStore);

            if (subject == null)
                return;

            Console.Write("\nNew name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("New semester: ");
            int semester = int.Parse(Console.ReadLine() ?? "");

            Console.Write("New credits: ");
            int credits = int.Parse(Console.ReadLine() ?? "");

            _subjectService.UpdateSubject(
                subject,
                name,
                semester,
                credits);

            Console.WriteLine("\nSubject updated successfully.");
        }

        private void DeleteSubject()
        {
            Subject? subject = EntitySelector.SelectSubject(_dataStore);

            if (subject == null)
                return;

            bool result = _subjectService.DeleteSubject(subject);

            Console.WriteLine(result
                ? "\nSubject deleted successfully."
                : "\nSubject could not be deleted.");
        }
    }
}