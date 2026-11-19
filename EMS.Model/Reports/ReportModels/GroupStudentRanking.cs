using System;

namespace EMS.Model.Reports.ReportModels
{
    public class GroupStudentRanking
    {
        public int Rank { get; }
        public Guid StudentId { get; }
        public string StudentName { get; }
        public string Group { get; }
        public int GradeCount { get; }
        public double AverageGrade { get; }

        public GroupStudentRanking(
            int rank,
            Guid studentId,
            string studentName,
            string group,
            int gradeCount,
            double averageGrade)
        {
            if (rank <= 0)
                throw new ArgumentOutOfRangeException(nameof(rank));

            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID cannot be empty.", nameof(studentId));

            StudentName = studentName
                ?? throw new ArgumentNullException(nameof(studentName));

            Group = group
                ?? throw new ArgumentNullException(nameof(group));

            if (gradeCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(gradeCount));

            if (averageGrade < 0 || averageGrade > 100)
                throw new ArgumentOutOfRangeException(nameof(averageGrade));

            Rank = rank;
            StudentId = studentId;
            GradeCount = gradeCount;
            AverageGrade = averageGrade;
        }
    }
}
