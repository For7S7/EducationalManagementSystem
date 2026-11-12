using EMS.Model;
using EMS.Persistence.JSON;

namespace EMS.Presentation.Menus
{
    public class ConsoleMenu
    {
        private readonly StudentMenu _studentMenu;
        private readonly TeacherMenu _teacherMenu;
        private readonly SubjectMenu _subjectMenu;
        private readonly GradeMenu _gradeMenu;

        private readonly JsonDataService _jsonService;
        private readonly IDataStore _dataStore;

        public ConsoleMenu(
            StudentMenu studentMenu,
            TeacherMenu teacherMenu,
            SubjectMenu subjectMenu,
            GradeMenu gradeMenu,
            JsonDataService jsonService,
            IDataStore dataStore)
        {
            _studentMenu = studentMenu;
            _teacherMenu = teacherMenu;
            _subjectMenu = subjectMenu;
            _gradeMenu = gradeMenu;

            _jsonService = jsonService;
            _dataStore = dataStore;
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();

                Console.WriteLine("----------------------------------");
                Console.WriteLine("   EDUCATIONAL MANAGEMENT SYSTEM");
                Console.WriteLine("----------------------------------\n");

                Console.WriteLine("1. Student Management");
                Console.WriteLine("2. Teacher Management");
                Console.WriteLine("3. Subject Management");
                Console.WriteLine("4. Grade Management");

                Console.WriteLine("\n0. Save and Exit");

                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _studentMenu.Run();
                        break;

                    case "2":
                        _teacherMenu.Run();
                        break;

                    case "3":
                        _subjectMenu.Run();
                        break;

                    case "4":
                        _gradeMenu.Run();
                        break;

                    case "0":
                        _jsonService.SaveData(_dataStore);

                        Console.WriteLine("\nData saved successfully.");
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("\nInvalid option.");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}