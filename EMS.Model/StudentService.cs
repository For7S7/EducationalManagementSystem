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

            Student existingStudent = _dataStore.Students.FirstOrDefault(s => s.Id == student.Id);

            if (existingStudent == null)
                return false;

            if (_dataStore.Grades.Any(g => g.Student.Id == existingStudent.Id))
                throw new InvalidOperationException("Cannot delete a student who has grades.");

            return _dataStore.Students.Remove(existingStudent);
        }

        public void AssignSubject(Student student, Subject subject)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            Student existingStudent = _dataStore.Students.FirstOrDefault(s => s.Id == student.Id);

            Subject existingSubject = _dataStore.Subjects.FirstOrDefault(s => s.Id == subject.Id);

            if (existingStudent == null || existingSubject == null)
                throw new InvalidOperationException("Student or subject does not exist.");

            if (existingStudent.Subjects.Any(s => s.Id == existingSubject.Id))
                throw new InvalidOperationException("Subject is already assigned.");

            existingStudent.Subjects.Add(existingSubject);
        }

        public bool RemoveSubject(Student student, Subject subject)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            Student existingStudent = _dataStore.Students.FirstOrDefault(s => s.Id == student.Id);

            if (existingStudent == null)
                return false;

            Subject existingSubject = existingStudent.Subjects.FirstOrDefault(s => s.Id == subject.Id);

            if (existingSubject == null)
                return false;

            if (_dataStore.Grades.Any(g => g.Student.Id == existingStudent.Id && g.Subject.Id == existingSubject.Id))
                throw new InvalidOperationException("Cannot remove a subject that has grades.");

            return existingStudent.Subjects.Remove(existingSubject);
        }

        public IReadOnlyList<Subject> GetSubjects(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            return student.Subjects.AsReadOnly();
        }
    }
}
