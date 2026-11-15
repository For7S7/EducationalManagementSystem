using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model.Reports.ReportModels
{
    public class FailedStudentRecord
    {
        public string StudentName { get; }
        public string Group { get; }
        public string SubjectName { get; }
        public double AverageGrade { get; }

        public FailedStudentRecord(
            string studentName,
            string group,
            string subjectName,
            double averageGrade)
        {
            StudentName = studentName
                ?? throw new ArgumentNullException(nameof(studentName));

            Group = group
                ?? throw new ArgumentNullException(nameof(group));

            SubjectName = subjectName
                ?? throw new ArgumentNullException(nameof(subjectName));

            if (averageGrade < 0 || averageGrade > 100)
                throw new ArgumentOutOfRangeException(nameof(averageGrade));

            AverageGrade = averageGrade;
        }
    }
}
