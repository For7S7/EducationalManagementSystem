using EMS.Model.Reports.ReportModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Model.Reports
{
    public class GroupReportService
    {
        private readonly IDataStore _dataStore;
        private readonly GradeService _gradeService;

        public GroupReportService(
            IDataStore dataStore,
            GradeService gradeService)
        {
            _dataStore = dataStore
                ?? throw new ArgumentNullException(nameof(dataStore));

            _gradeService = gradeService
                ?? throw new ArgumentNullException(nameof(gradeService));
        }

        public GroupSemesterStatistics GenerateGroupSemesterStatistics(
            string group,
            AcademicYear academicYear,
            int semester)
        {
            if (string.IsNullOrWhiteSpace(group))
                throw new ArgumentException("Group cannot be empty.", nameof(group));

            if (academicYear == null)
                throw new ArgumentNullException(nameof(academicYear));

            if (semester < 1 || semester > 8)
                throw new ArgumentOutOfRangeException(nameof(semester));

            var students = _dataStore.Students
                .Where(student => string.Equals(student.Group, group, StringComparison.OrdinalIgnoreCase)).ToList();

            var studentAverages = students
                .Select(student =>
                {
                    var grades = _gradeService.GetStudentGrades(student)
                        .Where(grade =>
                            grade.Subject.Semester == semester &&
                            AcademicYear.FromDate(grade.Date).StartYear ==
                            academicYear.StartYear)
                        .ToList();

                    if (grades.Count == 0)
                        return (double?)null;

                    return grades.Average(grade => grade.NumericValue);
                })
                .Where(average => average.HasValue)
                .Select(average => average.Value)
                .ToList();

            double? groupAverage = studentAverages.Count > 0 ? studentAverages.Average() : null;

            return new GroupSemesterStatistics(
                group,
                academicYear,
                semester,
                studentAverages.Count,
                groupAverage);
        }

        public IReadOnlyList<FailedStudentRecord> GenerateFailedStudentsReport(
    string group,
    Subject subject,
    AcademicYear academicYear,
    int semester)
        {
            if (string.IsNullOrWhiteSpace(group))
                throw new ArgumentException("Group cannot be empty.", nameof(group));

            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (academicYear == null)
                throw new ArgumentNullException(nameof(academicYear));

            if (semester < 1 || semester > 8)
                throw new ArgumentOutOfRangeException(nameof(semester));

            if (subject.Semester != semester)
                throw new ArgumentException("Subject does not belong to the selected semester.", nameof(subject));

            var students = _dataStore.Students.Where(student => string.Equals(student.Group, group, StringComparison.OrdinalIgnoreCase)).ToList();

            var failedStudents = students
                .Select(student =>
                {
                    var subjectGrades = _gradeService.GetStudentGrades(student)
                        .Where(grade =>
                            grade.Subject.Id == subject.Id &&
                            AcademicYear.FromDate(grade.Date).StartYear ==
                            academicYear.StartYear).ToList();

                    if (subjectGrades.Count == 0)
                        return null;

                    double averageGrade = subjectGrades.Average(grade => grade.NumericValue);

                    if (_gradeService.IsPassingGrade(averageGrade))
                        return null;

                    return new FailedStudentRecord(
                        student.FullName,
                        student.Group,
                        subject.Name,
                        averageGrade);
                })
                .Where(record => record != null).ToList();

            return failedStudents.AsReadOnly();
        }
    }
}
