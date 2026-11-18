using EMS.Model.Reports.ReportModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Model.Reports
{
    public class DepartmentReportService
    {
        private readonly IDataStore _dataStore;

        public DepartmentReportService(IDataStore dataStore)
        {
            _dataStore = dataStore
                ?? throw new ArgumentNullException(nameof(dataStore));
        }

        public DepartmentStatistics GenerateDepartmentStatistics(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            Department existingDepartment = _dataStore.Departments
                .FirstOrDefault(d => d.Id == department.Id);

            if (existingDepartment == null)
                throw new InvalidOperationException("Department does not exist.");

            // Use the department's teacher list as the source of its teaching staff.
            // Resolve teachers through the main data store to avoid using stale references.
            var departmentTeacherIds = existingDepartment.Teachers
                .Select(teacher => teacher.Id)
                .ToHashSet();

            var departmentTeachers = _dataStore.Teachers
                .Where(teacher => departmentTeacherIds.Contains(teacher.Id))
                .ToList();

            // A subject is counted once even when several department teachers teach it.
            int subjectCount = departmentTeachers
                .SelectMany(teacher => teacher.Subjects)
                .Select(subject => subject.Id)
                .Distinct()
                .Count();

            var departmentTeacherIdSet = departmentTeachers
                .Select(teacher => teacher.Id)
                .ToHashSet();

            // Department grades are grades issued by teachers who currently belong
            // to this department. Each student's average is calculated first,
            // so every student has equal weight in the department average.
            var studentsById = _dataStore.Students
                .ToDictionary(student => student.Id);

            var studentGradeGroups = _dataStore.Grades
                .Where(grade => departmentTeacherIdSet.Contains(grade.Teacher.Id))
                .GroupBy(grade => grade.Student.Id);

            var studentPerformances = new List<DepartmentStudentPerformance>();

            foreach (var gradeGroup in studentGradeGroups)
            {
                if (!studentsById.TryGetValue(gradeGroup.Key, out Student student))
                    continue;

                studentPerformances.Add(new DepartmentStudentPerformance(
                    student.Id,
                    student.FullName,
                    student.Group,
                    gradeGroup.Count(),
                    gradeGroup.Average(grade => grade.NumericValue)));
            }

            var rankedStudents = studentPerformances
                .OrderByDescending(student => student.AverageGrade)
                .ThenBy(student => student.StudentName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(student => student.StudentId)
                .ToList();

            double? departmentAverage = rankedStudents.Count > 0
                ? rankedStudents.Average(student => student.AverageGrade)
                : null;

            return new DepartmentStatistics(
                existingDepartment.Id,
                existingDepartment.Name,
                subjectCount,
                rankedStudents.Count,
                departmentAverage,
                rankedStudents.Take(5));
        }
    }
}