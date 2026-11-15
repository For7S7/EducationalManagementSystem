using EMS.Model.Reports.ReportModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EMS.Model.Reports
{
    public class StudentReportService
    {
        private readonly GradeService _gradeService;

        public StudentReportService(GradeService gradeService)
        {
            _gradeService = gradeService
                ?? throw new ArgumentNullException(nameof(gradeService));
        }

        public StudentTranscript GenerateStudentTranscript(
            Student student,
            AcademicYear academicYear,
            int semester)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (academicYear == null)
                throw new ArgumentNullException(nameof(academicYear));

            if (semester < 1 || semester > 8)
                throw new ArgumentOutOfRangeException(nameof(semester));

            var semesterGrades = _gradeService.GetStudentSemesterGrades(
                student,
                academicYear,
                semester);

            var subjectEntries = student.Subjects
                .Where(subject => subject.Semester == semester)
                .OrderBy(subject => subject.Name)
                .Select(subject =>
                {
                    var subjectGrades = semesterGrades.Where(grade => grade.Subject.Id == subject.Id)
                        .Select(grade => (double)grade.NumericValue)
                        .ToList();

                    return new SubjectTranscriptEntry(subject.Name, subjectGrades);
                }).ToList();

            double? semesterAverage = semesterGrades.Count > 0
                ? semesterGrades.Average(grade => grade.NumericValue)
                : null;

            return new StudentTranscript(
                student.FullName,
                student.Group,
                student.AdmissionYear,
                academicYear,
                semester,
                subjectEntries,
                semesterAverage);
        }

        public StudentAcademicRecord GenerateStudentAcademicRecord(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            var studentGrades = _gradeService.GetStudentGrades(student);

            var semesterGroups = studentGrades
                .GroupBy(grade => new
                {
                    AcademicYearStart = AcademicYear.FromDate(grade.Date).StartYear,
                    Semester = grade.Subject.Semester
                }).OrderBy(group => group.Key.AcademicYearStart).ThenBy(group => group.Key.Semester);

            var semesterRecords = semesterGroups
                .Select(group =>
                {
                    AcademicYear academicYear =
                        new AcademicYear(group.Key.AcademicYearStart);

                    var subjectEntries = student.Subjects
                        .Where(subject => subject.Semester == group.Key.Semester)
                        .OrderBy(subject => subject.Name)
                        .Select(subject =>
                        {
                            var subjectGrades = group
                                .Where(grade => grade.Subject.Id == subject.Id)
                                .OrderBy(grade => grade.Date)
                                .Select(grade => (double)grade.NumericValue)
                                .ToList();

                            return new SubjectTranscriptEntry(
                                subject.Name,
                                subjectGrades);
                        })
                        .ToList();

                    return new SemesterAcademicRecord(
                        academicYear,
                        group.Key.Semester,
                        subjectEntries);
                }).ToList();

            return new StudentAcademicRecord(
                student.FullName,
                student.Group,
                student.AdmissionYear,
                semesterRecords);
        }
    }
}
