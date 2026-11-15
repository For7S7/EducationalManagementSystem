using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model.Reports.ReportModels
{
    public class StudentAcademicRecord
    {
        public string StudentName { get; }
        public string Group { get; }
        public int AdmissionYear { get; }

        public IReadOnlyList<SemesterAcademicRecord> Semesters { get; }

        public double? OverallAverage { get; }

        public StudentAcademicRecord(
            string studentName,
            string group,
            int admissionYear,
            IEnumerable<SemesterAcademicRecord> semesters)
        {
            StudentName = studentName
                ?? throw new ArgumentNullException(nameof(studentName));

            Group = group
                ?? throw new ArgumentNullException(nameof(group));

            if (semesters == null)
                throw new ArgumentNullException(nameof(semesters));

            AdmissionYear = admissionYear;

            List<SemesterAcademicRecord> semesterList = semesters.OrderBy(record => record.AcademicYear.StartYear)
                .ThenBy(record => record.Semester)
                .ToList();

            Semesters = semesterList.AsReadOnly();

            List<double> grades = semesterList.SelectMany(record => record.Subjects).SelectMany(subject => subject.Grades).ToList();

            OverallAverage = grades.Count > 0 ? grades.Average() : null;
        }
    }
}
