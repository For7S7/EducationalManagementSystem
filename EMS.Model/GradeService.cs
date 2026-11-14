using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EMS.Model
{
    public class GradeService
    {
        private readonly IDataStore _dataStore;

        private string CalculateLetterGrade(int numericValue)
        {
            if (numericValue >= 90)
                return "A";

            if (numericValue >= 82)
                return "B";

            if (numericValue >= 74)
                return "C";

            if (numericValue >= 64)
                return "D";

            if (numericValue >= 60)
                return "E";

            if (numericValue >= 35)
                return "FX";

            return "F";
        }

        public GradeService(IDataStore dataStore)
        {
            _dataStore = dataStore ??
                throw new ArgumentNullException(nameof(dataStore));
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
                LetterValue = CalculateLetterGrade(numericValue),
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
            existingGrade.LetterValue = CalculateLetterGrade(newNumericValue);
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
    }
}
