using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model.Reports.ReportModels
{
    public class SemesterAcademicRecord
    {
        public AcademicYear AcademicYear { get; }
        public int Semester { get; }
        public IReadOnlyList<SubjectTranscriptEntry> Subjects { get; }
        public double? SemesterAverage { get; }

        public SemesterAcademicRecord(
            AcademicYear academicYear,
            int semester,
            IEnumerable<SubjectTranscriptEntry> subjects)
        {
            AcademicYear = academicYear
                ?? throw new ArgumentNullException(nameof(academicYear));

            if (semester < 1 || semester > 8)
                throw new ArgumentOutOfRangeException(nameof(semester));

            if (subjects == null)
                throw new ArgumentNullException(nameof(subjects));

            Semester = semester;

            List<SubjectTranscriptEntry> subjectList = subjects.ToList();

            Subjects = subjectList.AsReadOnly();

            List<double> grades = subjectList.SelectMany(subject => subject.Grades).ToList();

            SemesterAverage = grades.Count > 0 ? grades.Average() : null;
        }
    }
}
