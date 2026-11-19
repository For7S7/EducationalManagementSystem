using System;

namespace EMS.Model.Reports.ReportModels
{
    public class DepartmentStudentPerformance
    {
        public Guid StudentId { get; }
        public string StudentName { get; }
        public string Group { get; }
        public int GradeCount { get; }
        public double AverageGrade { get; }

        public DepartmentStudentPerformance(
            Guid studentId,
            string studentName,
            string group,
            int gradeCount,
            double averageGrade)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID cannot be empty.", nameof(studentId));

            StudentId = studentId;
            StudentName = studentName
                ?? throw new ArgumentNullException(nameof(studentName));
            Group = group ?? string.Empty;

            if (gradeCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(gradeCount));

            if (averageGrade < 0 || averageGrade > 100)
                throw new ArgumentOutOfRangeException(nameof(averageGrade));

            GradeCount = gradeCount;
            AverageGrade = averageGrade;
        }
    }
}