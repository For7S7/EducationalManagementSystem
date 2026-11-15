using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model.Reports.ReportModels
{
    public class TeacherSemesterStatistics
    {
        public string TeacherName { get; }
        public AcademicYear AcademicYear { get; }
        public int Semester { get; }
        public int GradeCount { get; }

        public TeacherSemesterStatistics(
            string teacherName,
            AcademicYear academicYear,
            int semester,
            int gradeCount)
        {
            TeacherName = teacherName
                ?? throw new ArgumentNullException(nameof(teacherName));

            AcademicYear = academicYear
                ?? throw new ArgumentNullException(nameof(academicYear));

            if (semester < 1 || semester > 8)
                throw new ArgumentOutOfRangeException(nameof(semester));

            if (gradeCount < 0)
                throw new ArgumentOutOfRangeException(nameof(gradeCount));

            Semester = semester;
            GradeCount = gradeCount;
        }
    }
}
