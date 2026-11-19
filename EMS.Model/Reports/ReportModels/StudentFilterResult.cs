using System;
using EMS.Model;

namespace EMS.Model.Reports.ReportModels
{
    public class StudentFilterResult
    {
        public Student Student { get; }
        public int GradeCount { get; }
        public double? AverageGrade { get; }

        public StudentFilterResult(
            Student student,
            int gradeCount,
            double? averageGrade)
        {
            Student = student
                ?? throw new ArgumentNullException(nameof(student));

            if (gradeCount < 0)
                throw new ArgumentOutOfRangeException(nameof(gradeCount));

            if (averageGrade < 0 || averageGrade > 100)
                throw new ArgumentOutOfRangeException(nameof(averageGrade));

            if (averageGrade.HasValue && gradeCount == 0)
                throw new ArgumentException(
                    "Average grade cannot be specified when there are no grades.",
                    nameof(averageGrade));

            GradeCount = gradeCount;
            AverageGrade = averageGrade;
        }
    }
}