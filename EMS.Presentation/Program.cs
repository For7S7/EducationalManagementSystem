using EMS.Model;
using EMS.Persistence;
using EMS.Persistence.JSON;
using EMS.Presentation.Menus;

namespace EMS.Presentation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IDataStore dataStore = new InMemoryDataStore();

            JsonDataService jsonService = new JsonDataService();
            jsonService.LoadData(dataStore);

            StudentService studentService = new StudentService(dataStore);
            TeacherService teacherService = new TeacherService(dataStore);
            SubjectService subjectService = new SubjectService(dataStore);
            GradeService gradeService = new GradeService(dataStore);

            StudentMenu studentMenu = new StudentMenu(dataStore, studentService);
            TeacherMenu teacherMenu = new TeacherMenu(dataStore, teacherService);
            SubjectMenu subjectMenu = new SubjectMenu(dataStore, subjectService);
            GradeMenu gradeMenu = new GradeMenu(dataStore, gradeService);

            ConsoleMenu consoleMenu = new ConsoleMenu(
                studentMenu,
                teacherMenu,
                subjectMenu,
                gradeMenu,
                jsonService,
                dataStore);

            consoleMenu.Run();
        }
    }
}