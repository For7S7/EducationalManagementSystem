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
            _dataStore = dataStore;
        }

        public void AddGrade(Student student, Teacher teacher, Subject subject, int numericValue, DateTime date)
        {
            if (student == null || teacher == null || subject == null)
                throw new ArgumentNullException();

            if (!student.Subjects.Contains(subject))
                throw new InvalidOperationException("Student is not assigned to this subject.");

            if (!teacher.Subjects.Contains(subject))
                throw new InvalidOperationException("Teacher does not teach this subject.");

            if (numericValue < 0 || numericValue > 100)
                throw new ArgumentOutOfRangeException(nameof(numericValue));

            if (date > DateTime.Now)
                throw new ArgumentException("Grade date cannot be in the future.", nameof(date));

            Grade grade = new Grade
            {
                Student = student,
                Teacher = teacher,
                Subject = subject,
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

            if (!_dataStore.Grades.Contains(grade))
                throw new InvalidOperationException("Grade not found.");

            if (newNumericValue < 0 || newNumericValue > 100)
                throw new ArgumentOutOfRangeException(nameof(newNumericValue));

            grade.NumericValue = newNumericValue;
            grade.LetterValue = CalculateLetterGrade(newNumericValue);
        }

        public bool DeleteGrade(Grade grade)
        {
            if (grade == null)
                throw new ArgumentNullException(nameof(grade));

            return _dataStore.Grades.Remove(grade);
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

            return _dataStore.Grades.Where(g => g.Student.Id == student.Id && g.Subject == subject).ToList().AsReadOnly();
        }
    }
}
