using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model
{
    public class StudentService
    {
        private readonly IDataStore _dataStore;

        public StudentService(IDataStore dataStore)
        {
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        }
        public void AddStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (_dataStore.Students.Any(s => s.Id == student.Id))
                throw new InvalidOperationException("Student already exists.");

            _dataStore.Students.Add(student);
        }

        public bool DeleteStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (_dataStore.Grades.Any(g => g.Student.Id == student.Id))
                throw new InvalidOperationException(
                    "Cannot delete a student who has grades.");

            return _dataStore.Students.Remove(_dataStore.Students.FirstOrDefault(s => s.Id == student.Id));
        }

        public void AssignSubject(Student student, Subject subject)
        {
            if (student == null || subject == null)
                throw new ArgumentNullException();

            if (!_dataStore.Students.Any(s => s.Id == student.Id) ||
                !_dataStore.Subjects.Any(s => s.Id == subject.Id))
                throw new InvalidOperationException("Student or subject does not exist.");

            if (student.Subjects.Any(s => s.Id == subject.Id))
                throw new InvalidOperationException("Subject is already assigned.");

            student.Subjects.Add(subject);
        }

        public bool RemoveSubject(Student student, Subject subject)
        {
            if (student == null || subject == null)
                throw new ArgumentNullException();

            if (_dataStore.Grades.Any(g =>
                g.Student.Id == student.Id && g.Subject.Id == subject.Id))
                throw new InvalidOperationException(
                    "Cannot remove a subject that has grades.");

            Subject existingSubject = student.Subjects.FirstOrDefault(s => s.Id == subject.Id);

            if (existingSubject == null)
                return false;

            return student.Subjects.Remove(existingSubject);
        }

        public IReadOnlyList<Subject> GetSubjects(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            return student.Subjects.AsReadOnly();
        }
    }
}
