using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EMS.Model.Reports.ReportModels
{
    public class GroupSemesterStatistics
    {
        public string Group { get; }
        public AcademicYear AcademicYear { get; }
        public int Semester { get; }
        public int StudentCount { get; }
        public double? GroupAverage { get; }

        public GroupSemesterStatistics(
            string group,
            AcademicYear academicYear,
            int semester,
            int studentCount,
            double? groupAverage)
        {
            Group = group
                ?? throw new ArgumentNullException(nameof(group));

            AcademicYear = academicYear
                ?? throw new ArgumentNullException(nameof(academicYear));

            if (semester < 1 || semester > 8)
                throw new ArgumentOutOfRangeException(nameof(semester));

            if (studentCount < 0)
                throw new ArgumentOutOfRangeException(nameof(studentCount));

            if (groupAverage < 0 || groupAverage > 100)
                throw new ArgumentOutOfRangeException(nameof(groupAverage));

            Semester = semester;
            StudentCount = studentCount;
            GroupAverage = groupAverage;
        }
    }
}
