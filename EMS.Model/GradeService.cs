using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EMS.Model.Grading;

namespace EMS.Model
{
    public class GradeService
    {
        private readonly IDataStore _dataStore;
        private readonly IGradingScale _gradingScale;

        public GradeService(IDataStore dataStore, IGradingScale gradingScale)
        {
            _dataStore = dataStore ??
                throw new ArgumentNullException(nameof(dataStore));

            _gradingScale = gradingScale ??
                throw new ArgumentNullException(nameof(gradingScale));
        }

        public void AddGrade(Student student, Teacher teacher, Subject subject, int numericValue, DateTime date)
        {
            Student existingStudent = _dataStore.Students.FirstOrDefault(s => s.Id == student.Id);
            Teacher existingTeacher = _dataStore.Teachers.FirstOrDefault(t => t.Id == teacher.Id);
            Subject existingSubject = _dataStore.Subjects.FirstOrDefault(s => s.Id == subject.Id);

            if (student == null || teacher == null || subject == null)
                throw new ArgumentNullException();

            if (!student.Subjects.Any(s => s.Id == subject.Id))
                throw new InvalidOperationException("Student is not assigned to this subject.");

            if (existingStudent == null || existingTeacher == null || existingSubject == null)
                throw new InvalidOperationException("Student, teacher or subject does not exist.");

            if (!teacher.Subjects.Contains(subject))
                throw new InvalidOperationException("Teacher does not teach this subject.");

            if (numericValue < 0 || numericValue > 100)
                throw new ArgumentOutOfRangeException(nameof(numericValue));

            if (date > DateTime.Now)
                throw new ArgumentException("Grade date cannot be in the future.", nameof(date));

            Grade grade = new Grade
            {
                Student = existingStudent,
                Teacher = existingTeacher,
                Subject = existingSubject,
                NumericValue = numericValue,
                LetterValue = _gradingScale.CalculateLetterGrade(numericValue),
                Date = date
            };

            _dataStore.Grades.Add(grade);
        }

        public void UpdateGrade(Grade grade, int newNumericValue)
        {
            if (grade == null)
                throw new ArgumentNullException(nameof(grade));

            Grade existingGrade = _dataStore.Grades.FirstOrDefault(g => g.Id == grade.Id);

            if (existingGrade == null)
                throw new InvalidOperationException("Grade not found.");

            if (newNumericValue < 0 || newNumericValue > 100)
                throw new ArgumentOutOfRangeException(nameof(newNumericValue));

            existingGrade.NumericValue = newNumericValue;
            existingGrade.LetterValue = _gradingScale.CalculateLetterGrade(newNumericValue);
        }

        public bool DeleteGrade(Grade grade)
        {
            if (grade == null)
                throw new ArgumentNullException(nameof(grade));

            Grade existingGrade = _dataStore.Grades.FirstOrDefault(g => g.Id == grade.Id);

            if (existingGrade == null)
                return false;

            return _dataStore.Grades.Remove(existingGrade);
        }

        public IReadOnlyList<Grade> GetStudentGrades(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            return _dataStore.Grades.Where(g => g.Student.Id == student.Id).ToList().AsReadOnly();
        }

        public IReadOnlyList<Grade> GetSubjectGrades(Student student, Subject subject)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            return _dataStore.Grades.Where(g => g.Student.Id == student.Id && g.Subject.Id == subject.Id).ToList().AsReadOnly();
        }

        public double? GetStudentSubjectAverage(Student student, Subject subject)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            List<Grade> grades = _dataStore.Grades.Where(g => g.Student.Id == student.Id && g.Subject.Id == subject.Id).ToList();

            if (!grades.Any())
                return null;

            return grades.Average(g => g.NumericValue);
        }

        public double? GetStudentSemesterAverage(Student student, int semester)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (semester <= 0)
                throw new ArgumentOutOfRangeException(nameof(semester));

            List<Guid> semesterSubjectIds = _dataStore.Subjects.Where(s => s.Semester == semester).Select(s => s.Id).ToList();

            List<Grade> grades = _dataStore.Grades.Where(g => g.Student.Id == student.Id && semesterSubjectIds.Contains(g.Subject.Id)).ToList();

            if (!grades.Any())
                return null;

            return grades.Average(g => g.NumericValue);
        }

        public IReadOnlyList<Grade> GetStudentSemesterGrades(Student student, AcademicYear academicYear, int semester)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (academicYear == null)
                throw new ArgumentNullException(nameof(academicYear));

            if (semester < 1 || semester > 8)
                throw new ArgumentOutOfRangeException(nameof(semester));

            return _dataStore.Grades.Where(grade => grade.Student.Id == student.Id && grade.Subject.Semester == semester &&
                    AcademicYear.FromDate(grade.Date).StartYear == academicYear.StartYear).ToList().AsReadOnly();
        }

        public bool IsPassingGrade(Grade grade)
        {
            if (grade == null)
                throw new ArgumentNullException(nameof(grade));

            return IsPassingGrade(grade.NumericValue);
        }

        public bool IsPassingGrade(double numericValue)
        {
            return _gradingScale.IsPassingGrade(numericValue);
        }
    }
}
