using EMS.Model;
using EMS.Persistence.JSON;

namespace EMS.Presentation.Menus
{
    public class ConsoleMenu
    {
        private readonly StudentMenu _studentMenu;
        private readonly TeacherMenu _teacherMenu;
        private readonly DepartmentMenu _departmentMenu;
        private readonly SubjectMenu _subjectMenu;
        private readonly GradeMenu _gradeMenu;
        private readonly GroupReportMenu _groupReportMenu;
        private readonly TeacherReportMenu _teacherReportMenu;

        private readonly JsonDataService _jsonService;
        private readonly IDataStore _dataStore;

        public ConsoleMenu(
            StudentMenu studentMenu,
            TeacherMenu teacherMenu,
            DepartmentMenu departmentMenu,
            SubjectMenu subjectMenu,
            GradeMenu gradeMenu,
            GroupReportMenu groupReportMenu,
            TeacherReportMenu teacherReportMenu,
            JsonDataService jsonService,
            IDataStore dataStore)
        {
            _studentMenu = studentMenu;
            _teacherMenu = teacherMenu;
            _departmentMenu = departmentMenu;
            _subjectMenu = subjectMenu;
            _gradeMenu = gradeMenu;
            _groupReportMenu = groupReportMenu;
            _teacherReportMenu = teacherReportMenu;
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
                Console.WriteLine("5. Group Reports");
                Console.WriteLine("6. Teacher Reports");
                Console.WriteLine("7. Department Management");

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

                    case "5":
                        _groupReportMenu.Run();
                        break;

                    case "6":
                        _teacherReportMenu.Run();
                        break;

                    case "7":
                        _departmentMenu.Run();
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