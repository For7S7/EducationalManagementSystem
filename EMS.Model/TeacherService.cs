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

            if (!teacher.Position.HasValue ||
                !Enum.IsDefined(typeof(TeacherPosition), teacher.Position.Value))
                throw new ArgumentException("A valid teacher position must be specified.", nameof(teacher));

            if (_dataStore.Teachers.Any(t => t.Id == teacher.Id))
                throw new InvalidOperationException("Teacher already exists.");

            _dataStore.Teachers.Add(teacher);

            // Keep the Department entity synchronized with the existing Department name field.
            if (!string.IsNullOrWhiteSpace(teacher.Department))
            {
                string departmentName = teacher.Department.Trim();
                Department department = _dataStore.Departments.FirstOrDefault(d =>
                    string.Equals(d.Name, departmentName, StringComparison.OrdinalIgnoreCase));

                if (department == null)
                {
                    department = new Department { Name = departmentName };
                    _dataStore.Departments.Add(department);
                }

                if (!department.Teachers.Any(t => t.Id == teacher.Id))
                    department.Teachers.Add(teacher);

                teacher.Department = department.Name;
            }
        }

        public bool DeleteTeacher(Teacher teacher)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            Teacher existingTeacher = _dataStore.Teachers.FirstOrDefault(t => t.Id == teacher.Id);

            if (existingTeacher == null)
                return false;

            if (_dataStore.Grades.Any(g => g.Teacher.Id == existingTeacher.Id))
                throw new InvalidOperationException("Cannot delete a teacher who has assigned grades.");

            foreach (Department department in _dataStore.Departments)
            {
                department.Teachers.RemoveAll(t => t.Id == existingTeacher.Id);

                if (department.Head != null && department.Head.Id == existingTeacher.Id)
                    department.Head = null;
            }

            return _dataStore.Teachers.Remove(existingTeacher);
        }
        public void AssignSubject(Teacher teacher, Subject subject)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            Teacher existingTeacher = _dataStore.Teachers.FirstOrDefault(t => t.Id == teacher.Id);

            Subject existingSubject = _dataStore.Subjects.FirstOrDefault(s => s.Id == subject.Id);

            if (existingTeacher == null || existingSubject == null)
                throw new InvalidOperationException("Teacher or subject does not exist.");

            if (existingTeacher.Subjects.Any(s => s.Id == existingSubject.Id))
                throw new InvalidOperationException("Subject is already assigned.");

            existingTeacher.Subjects.Add(existingSubject);
        }

        public IReadOnlyList<Subject> GetSubjects(Teacher teacher)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            return teacher.Subjects.AsReadOnly();
        }

        public bool RemoveSubject(Teacher teacher, Subject subject)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            Teacher existingTeacher = _dataStore.Teachers.FirstOrDefault(t => t.Id == teacher.Id);

            if (existingTeacher == null)
                return false;

            Subject existingSubject = existingTeacher.Subjects.FirstOrDefault(s => s.Id == subject.Id);

            if (existingSubject == null)
                return false;

            if (_dataStore.Grades.Any(g => g.Teacher.Id == existingTeacher.Id && g.Subject.Id == existingSubject.Id))
                throw new InvalidOperationException("Cannot remove a subject that has assigned grades.");

            return existingTeacher.Subjects.Remove(existingSubject);
        }
    }
}
