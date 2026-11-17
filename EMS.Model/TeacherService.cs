using System;
using System.Collections.Generic;
using System.Linq;

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

            if (string.IsNullOrWhiteSpace(teacher.FullName))
                throw new ArgumentException("Teacher name cannot be empty.", nameof(teacher));

            ValidatePosition(teacher.Position);

            if (_dataStore.Teachers.Any(t => t.Id == teacher.Id))
                throw new InvalidOperationException("Teacher already exists.");

            teacher.FullName = teacher.FullName.Trim();
            _dataStore.Teachers.Add(teacher);
            SynchronizeDepartment(teacher, teacher.Department);
        }

        public bool UpdateTeacher(Teacher teacher)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            Teacher existingTeacher = _dataStore.Teachers.FirstOrDefault(t => t.Id == teacher.Id);

            if (existingTeacher == null)
                return false;

            if (string.IsNullOrWhiteSpace(teacher.FullName))
                throw new ArgumentException("Teacher name cannot be empty.", nameof(teacher));

            ValidatePosition(teacher.Position);

            existingTeacher.FullName = teacher.FullName.Trim();
            existingTeacher.Position = teacher.Position;
            SynchronizeDepartment(existingTeacher, teacher.Department);

            return true;
        }

        private static void ValidatePosition(TeacherPosition? position)
        {
            if (!position.HasValue || !Enum.IsDefined(typeof(TeacherPosition), position.Value))
                throw new ArgumentException("A valid teacher position must be specified.", nameof(position));
        }

        private void SynchronizeDepartment(Teacher teacher, string? departmentName)
        {
            if (string.IsNullOrWhiteSpace(departmentName))
            {
                foreach (Department department in _dataStore.Departments)
                {
                    department.Teachers.RemoveAll(t => t.Id == teacher.Id);

                    if (department.Head != null && department.Head.Id == teacher.Id)
                        department.Head = null;
                }

                teacher.Department = string.Empty;
                return;
            }

            departmentName = departmentName.Trim();

            Department targetDepartment = _dataStore.Departments.FirstOrDefault(d =>
                string.Equals(d.Name, departmentName, StringComparison.OrdinalIgnoreCase));

            if (targetDepartment == null)
            {
                targetDepartment = new Department { Name = departmentName };
                _dataStore.Departments.Add(targetDepartment);
            }

            // Remove the teacher from any previous department before assigning the new one.
            foreach (Department department in _dataStore.Departments)
            {
                if (department.Id == targetDepartment.Id)
                    continue;

                department.Teachers.RemoveAll(t => t.Id == teacher.Id);

                if (department.Head != null && department.Head.Id == teacher.Id)
                    department.Head = null;
            }

            if (!targetDepartment.Teachers.Any(t => t.Id == teacher.Id))
                targetDepartment.Teachers.Add(teacher);

            teacher.Department = targetDepartment.Name;
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
