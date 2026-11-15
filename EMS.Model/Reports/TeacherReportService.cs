using EMS.Model.Reports.ReportModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EMS.Model.Reports
{
    public class TeacherReportService
    {
        private readonly IDataStore _dataStore;

        public TeacherReportService(IDataStore dataStore)
        {
            _dataStore = dataStore
                ?? throw new ArgumentNullException(nameof(dataStore));
        }

        public TeacherSubjectReport GenerateTeacherSubjectReport(
            Teacher teacher,
            Subject subject,
            AcademicYear academicYear,
            int semester)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (academicYear == null)
                throw new ArgumentNullException(nameof(academicYear));

            if (semester < 1 || semester > 8)
                throw new ArgumentOutOfRangeException(nameof(semester));

            if (subject.Semester != semester)
                throw new ArgumentException("The selected subject does not belong to this semester.", nameof(subject));

            var grades = _dataStore.Grades.Where(g => g.Teacher.Id == teacher.Id && g.Subject.Id == subject.Id &&
                    AcademicYear.FromDate(g.Date).StartYear == academicYear.StartYear).ToList();

            double? averageGrade = grades.Count > 0 ? grades.Average(g => g.NumericValue) : null;

            return new TeacherSubjectReport(
                teacher.FullName,
                subject.Name,
                academicYear,
                semester,
                grades.Count,
                averageGrade);
        }

        public TeacherSemesterStatistics GenerateTeacherSemesterStatistics(
            Teacher teacher,
            AcademicYear academicYear,
            int semester)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            if (academicYear == null)
                throw new ArgumentNullException(nameof(academicYear));

            if (semester < 1 || semester > 8)
                throw new ArgumentOutOfRangeException(nameof(semester));

            var grades = _dataStore.Grades.Where(g => g.Teacher.Id == teacher.Id && g.Subject.Semester == semester &&
                    AcademicYear.FromDate(g.Date).StartYear == academicYear.StartYear).ToList();

            return new TeacherSemesterStatistics(
                teacher.FullName,
                academicYear,
                semester,
                grades.Count);
        }
    }
}
