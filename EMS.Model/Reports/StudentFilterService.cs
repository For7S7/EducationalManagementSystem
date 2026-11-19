using EMS.Model.Reports.ReportModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Model.Reports
{
    public class StudentFilterService
    {
        private readonly IDataStore _dataStore;
        private readonly GradeService _gradeService;

        public StudentFilterService(
            IDataStore dataStore,
            GradeService gradeService)
        {
            _dataStore = dataStore
                ?? throw new ArgumentNullException(nameof(dataStore));

            _gradeService = gradeService
                ?? throw new ArgumentNullException(nameof(gradeService));
        }

        public IReadOnlyList<StudentFilterResult> FilterStudents(
            string? group = null,
            int? semester = null,
            double? minAverage = null,
            double? maxAverage = null)
        {
            if (semester.HasValue && (semester.Value < 1 || semester.Value > 8))
                throw new ArgumentOutOfRangeException(
                    nameof(semester),
                    "Semester must be between 1 and 8.");

            ValidateAverage(minAverage, nameof(minAverage));
            ValidateAverage(maxAverage, nameof(maxAverage));

            if (minAverage.HasValue && maxAverage.HasValue &&
                minAverage.Value > maxAverage.Value)
            {
                throw new ArgumentException(
                    "Minimum average cannot be greater than maximum average.");
            }

            var filteredStudents = new List<StudentFilterResult>();

            foreach (Student student in _dataStore.Students)
            {
                if (!string.IsNullOrWhiteSpace(group) &&
                    !string.Equals(
                        student.Group,
                        group,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var studentGrades = _gradeService.GetStudentGrades(student);

                if (semester.HasValue)
                {
                    bool hasAssignedSubjectInSemester = student.Subjects
                        .Any(subject => subject.Semester == semester.Value);

                    studentGrades = studentGrades
                        .Where(grade => grade.Subject.Semester == semester.Value)
                        .ToList()
                        .AsReadOnly();

                    // A student is included in a semester filter if they are
                    // assigned to at least one subject in that semester or
                    // have a recorded grade for one of its subjects.
                    if (!hasAssignedSubjectInSemester && studentGrades.Count == 0)
                        continue;
                }

                double? averageGrade = studentGrades.Count > 0
                    ? studentGrades.Average(grade => grade.NumericValue)
                    : null;

                // Average boundaries can only be checked for students
                // who have at least one grade in the selected scope.
                if ((minAverage.HasValue || maxAverage.HasValue) &&
                    !averageGrade.HasValue)
                {
                    continue;
                }

                if (minAverage.HasValue &&
                    (!averageGrade.HasValue || averageGrade.Value < minAverage.Value))
                {
                    continue;
                }

                if (maxAverage.HasValue &&
                    (!averageGrade.HasValue || averageGrade.Value > maxAverage.Value))
                {
                    continue;
                }

                filteredStudents.Add(new StudentFilterResult(
                    student,
                    studentGrades.Count,
                    averageGrade));
            }

            return filteredStudents
                .OrderBy(result => result.Student.Group, StringComparer.OrdinalIgnoreCase)
                .ThenBy(result => result.Student.FullName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(result => result.Student.Id)
                .ToList()
                .AsReadOnly();
        }

        private static void ValidateAverage(double? average, string parameterName)
        {
            if (average.HasValue && (average.Value < 0 || average.Value > 100))
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    "Average grade must be between 0 and 100.");
            }
        }
    }
}