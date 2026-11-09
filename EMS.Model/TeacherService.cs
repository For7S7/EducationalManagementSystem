using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model
{
    public class TeacherService
    {
        private readonly IDataStore _dataStore;

        public TeacherService(IDataStore dataStore)
        {
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        }

        public void AddTeacher(Teacher teacher)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            if (_dataStore.Teachers.Any(t => t.Id == teacher.Id))
                throw new InvalidOperationException("Teacher already exists.");

            _dataStore.Teachers.Add(teacher);
        }

        public bool DeleteTeacher(Teacher teacher)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            if (_dataStore.Grades.Any(g => g.Teacher.Id == teacher.Id))
                throw new InvalidOperationException(
                    "Cannot delete a teacher who has assigned grades.");

            return _dataStore.Teachers.Remove(_dataStore.Teachers.FirstOrDefault(t => t.Id == teacher.Id));
        }
        public void AssignSubject(Teacher teacher, Subject subject)
        {
            if (teacher == null || subject == null)
                throw new ArgumentNullException();

            if (teacher.Subjects.Any(s => s.Id == subject.Id))
                throw new InvalidOperationException("Subject is already assigned.");

            teacher.Subjects.Add(subject);
        }

        public IReadOnlyList<Subject> GetSubjects(Teacher teacher)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            return teacher.Subjects.AsReadOnly();
        }

        public bool RemoveSubject(Teacher teacher, Subject subject)
        {
            if (teacher == null || subject == null)
                throw new ArgumentNullException();

            Subject existingSubject = teacher.Subjects.FirstOrDefault(s => s.Id == subject.Id);


            return teacher.Subjects.Remove(existingSubject);
        }
    }
}
