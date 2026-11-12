using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model
{
    public class SubjectService
    {
        private readonly IDataStore _dataStore;

        public SubjectService(IDataStore dataStore)
        {
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        }

        public void AddSubject(Subject subject)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (_dataStore.Subjects.Any(s => s.Id == subject.Id))
                throw new InvalidOperationException("Subject already exists.");

            _dataStore.Subjects.Add(subject);
        }

        public bool DeleteSubject(Subject subject)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            bool isAssignedToStudent = _dataStore.Students
                .Any(s => s.Subjects.Any(sub => sub.Id == subject.Id));

            bool isAssignedToTeacher = _dataStore.Teachers
                .Any(t => t.Subjects.Any(sub => sub.Id == subject.Id));

            bool hasGrades = _dataStore.Grades
                .Any(g => g.Subject.Id == subject.Id);

            if (isAssignedToStudent || isAssignedToTeacher || hasGrades)
                throw new InvalidOperationException(
                    "Cannot delete a subject that is being used.");

            Subject existingSubject = _dataStore.Subjects.FirstOrDefault(s => s.Id == subject.Id);

            if (existingSubject == null)
                return false;

            return _dataStore.Subjects.Remove(existingSubject);
        }
        public void UpdateSubject(Subject subject, string newName, int newSemester, int newCredits)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Subject name cannot be empty.");

            if (newSemester <= 0)
                throw new ArgumentOutOfRangeException(nameof(newSemester));

            if (newSemester <= 0)
                throw new ArgumentOutOfRangeException(nameof(newCredits));

            subject.Name = newName;
            subject.Semester = newSemester;
            subject.Credits = newCredits;
        }

        public Subject GetSubjectInfo(Subject subject)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            return _dataStore.Subjects.FirstOrDefault(s => s.Id == subject.Id);
        }

        public IReadOnlyList<Subject> GetAllSubjects()
        {
            return _dataStore.Subjects.ToList().AsReadOnly();
        }
    }
}
