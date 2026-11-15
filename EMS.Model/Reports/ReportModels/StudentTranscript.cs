using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model.Reports.ReportModels
{
    public class StudentTranscript
    {
        public string StudentName { get; }
        public string Group { get; }
        public int AdmissionYear { get; }

        public AcademicYear AcademicYear { get; }
        public int Semester { get; }

        public IReadOnlyList<SubjectTranscriptEntry> Subjects { get; }

        public double? SemesterAverage { get; }

        public StudentTranscript(
            string studentName,
            string group,
            int admissionYear,
            AcademicYear academicYear,
            int semester,
            IEnumerable<SubjectTranscriptEntry> subjects,
            double? semesterAverage)
        {
            StudentName = studentName
                ?? throw new ArgumentNullException(nameof(studentName));

            Group = group
                ?? throw new ArgumentNullException(nameof(group));

            if (academicYear == null)
                throw new ArgumentNullException(nameof(academicYear));

            if (semester < 1 || semester > 8)
                throw new ArgumentOutOfRangeException(nameof(semester));

            if (subjects == null)
                throw new ArgumentNullException(nameof(subjects));

            AcademicYear = academicYear;
            Semester = semester;
            AdmissionYear = admissionYear;

            Subjects = new List<SubjectTranscriptEntry>(subjects).AsReadOnly();

            SemesterAverage = semesterAverage;
        }
    }
}
