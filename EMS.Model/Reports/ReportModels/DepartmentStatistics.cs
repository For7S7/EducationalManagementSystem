using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Model.Reports.ReportModels
{
    public class DepartmentStatistics
    {
        public Guid DepartmentId { get; }
        public string DepartmentName { get; }
        public int SubjectCount { get; }
        public int StudentCount { get; }
        public double? AverageGrade { get; }
        public IReadOnlyList<DepartmentStudentPerformance> TopStudents { get; }

        public DepartmentStatistics(
            Guid departmentId,
            string departmentName,
            int subjectCount,
            int studentCount,
            double? averageGrade,
            IEnumerable<DepartmentStudentPerformance> topStudents)
        {
            if (departmentId == Guid.Empty)
                throw new ArgumentException("Department ID cannot be empty.", nameof(departmentId));

            DepartmentId = departmentId;
            DepartmentName = departmentName
                ?? throw new ArgumentNullException(nameof(departmentName));

            if (subjectCount < 0)
                throw new ArgumentOutOfRangeException(nameof(subjectCount));

            if (studentCount < 0)
                throw new ArgumentOutOfRangeException(nameof(studentCount));

            if (averageGrade < 0 || averageGrade > 100)
                throw new ArgumentOutOfRangeException(nameof(averageGrade));

            if (topStudents == null)
                throw new ArgumentNullException(nameof(topStudents));

            SubjectCount = subjectCount;
            StudentCount = studentCount;
            AverageGrade = averageGrade;
            TopStudents = topStudents.ToList().AsReadOnly();
        }
    }
}