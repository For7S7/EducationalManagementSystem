using EMS.Model;
using EMS.Model.Grading;
using EMS.Model.Reports;
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
            DepartmentService departmentService = new DepartmentService(dataStore);
            SubjectService subjectService = new SubjectService(dataStore);

            IGradingScale gradingScale = GradeMenu.SelectGradingScale();

            GradeService gradeService = new GradeService(dataStore, gradingScale);

            StudentReportService studentReportService = new StudentReportService(gradeService);
            GroupReportService groupReportService = new GroupReportService(dataStore, gradeService);
            TeacherReportService teacherReportService = new TeacherReportService(dataStore);
            DepartmentReportService departmentReportService = new DepartmentReportService(dataStore);

            TeacherReportMenu teacherReportMenu = new TeacherReportMenu(dataStore, teacherReportService);
            StudentMenu studentMenu = new StudentMenu(dataStore, studentService, studentReportService);
            TeacherMenu teacherMenu = new TeacherMenu(dataStore, teacherService);
            DepartmentMenu departmentMenu = new DepartmentMenu(
                dataStore,
                departmentService,
                departmentReportService);
            SubjectMenu subjectMenu = new SubjectMenu(dataStore, subjectService);
            GradeMenu gradeMenu = new GradeMenu(dataStore, gradeService);
            GroupReportMenu groupReportMenu = new GroupReportMenu(dataStore, groupReportService);

            ConsoleMenu consoleMenu = new ConsoleMenu(
                studentMenu,
                teacherMenu,
                departmentMenu,
                subjectMenu,
                gradeMenu,
                groupReportMenu,
                teacherReportMenu,
                jsonService,
                dataStore);

            consoleMenu.Run();
        }
    }
}