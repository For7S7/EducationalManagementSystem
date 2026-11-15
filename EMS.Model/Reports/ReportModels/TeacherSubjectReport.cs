using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model.Reports.ReportModels
{
    public class TeacherSubjectReport
    {
        public string TeacherName { get; }
        public string SubjectName { get; }
        public AcademicYear AcademicYear { get; }
        public int Semester { get; }
        public int GradeCount { get; }
        public double? AverageGrade { get; }

        public TeacherSubjectReport(
            string teacherName,
            string subjectName,
            AcademicYear academicYear,
            int semester,
            int gradeCount,
            double? averageGrade)
        {
            TeacherName = teacherName
                ?? throw new ArgumentNullException(nameof(teacherName));

            SubjectName = subjectName
                ?? throw new ArgumentNullException(nameof(subjectName));

            AcademicYear = academicYear
                ?? throw new ArgumentNullException(nameof(academicYear));

            if (semester < 1 || semester > 8)
                throw new ArgumentOutOfRangeException(nameof(semester));

            if (gradeCount < 0)
                throw new ArgumentOutOfRangeException(nameof(gradeCount));

            if (averageGrade < 0 || averageGrade > 100)
                throw new ArgumentOutOfRangeException(nameof(averageGrade));

            Semester = semester;
            GradeCount = gradeCount;
            AverageGrade = averageGrade;
        }
    }
}
