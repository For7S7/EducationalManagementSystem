using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model.Reports.ReportModels
{
    public class SubjectTranscriptEntry
    {
        public string SubjectName { get; }
        public IReadOnlyList<double> Grades { get; }
        public double? Average { get; }

        public SubjectTranscriptEntry(string subjectName, IEnumerable<double> grades)
        {
            SubjectName = subjectName
                ?? throw new ArgumentNullException(nameof(subjectName));

            if (grades == null)
                throw new ArgumentNullException(nameof(grades));

            List<double> gradeList = grades.ToList();

            Grades = gradeList.AsReadOnly();

            Average = gradeList.Count > 0 ? gradeList.Average() : null;
        }
    }
}
