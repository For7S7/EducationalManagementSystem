using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Model
{
    public class DepartmentService
    {
        private readonly IDataStore _dataStore;

        public DepartmentService(IDataStore dataStore)
        {
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        }

        public void AddDepartment(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Department name cannot be empty.", nameof(name));

            name = name.Trim();

            if (_dataStore.Departments.Any(d =>
                string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Department already exists.");

            _dataStore.Departments.Add(new Department { Name = name });
        }

        public bool UpdateDepartment(Department department, string newName)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Department name cannot be empty.", nameof(newName));

            Department existingDepartment = _dataStore.Departments.FirstOrDefault(d => d.Id == department.Id);

            if (existingDepartment == null)
                return false;

            string oldName = existingDepartment.Name;
            newName = newName.Trim();

            if (_dataStore.Departments.Any(d => d.Id != existingDepartment.Id &&
                string.Equals(d.Name, newName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Department with this name already exists.");

            existingDepartment.Name = newName;

            foreach (Teacher teacher in _dataStore.Teachers)
            {
                if (existingDepartment.Teachers.Any(t => t.Id == teacher.Id) ||
                    string.Equals(teacher.Department, oldName, StringComparison.OrdinalIgnoreCase))
                {
                    teacher.Department = newName;

                    if (!existingDepartment.Teachers.Any(t => t.Id == teacher.Id))
                        existingDepartment.Teachers.Add(teacher);
                }
            }

            return true;
        }

        public bool DeleteDepartment(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            Department existingDepartment = _dataStore.Departments.FirstOrDefault(d => d.Id == department.Id);

            if (existingDepartment == null)
                return false;

            foreach (Teacher teacher in _dataStore.Teachers)
            {
                if (existingDepartment.Teachers.Any(t => t.Id == teacher.Id) ||
                    string.Equals(teacher.Department, existingDepartment.Name, StringComparison.OrdinalIgnoreCase))
                {
                    teacher.Department = string.Empty;
                }
            }

            existingDepartment.Teachers.Clear();
            existingDepartment.Head = null;

            return _dataStore.Departments.Remove(existingDepartment);
        }

        public void AssignTeacher(Department department, Teacher teacher)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            Department existingDepartment = _dataStore.Departments.FirstOrDefault(d => d.Id == department.Id);
            Teacher existingTeacher = _dataStore.Teachers.FirstOrDefault(t => t.Id == teacher.Id);

            if (existingDepartment == null || existingTeacher == null)
                throw new InvalidOperationException("Department or teacher does not exist.");

            // A teacher belongs to one department at a time.
            foreach (Department otherDepartment in _dataStore.Departments)
            {
                if (otherDepartment.Id == existingDepartment.Id)
                    continue;

                otherDepartment.Teachers.RemoveAll(t => t.Id == existingTeacher.Id);

                if (otherDepartment.Head != null && otherDepartment.Head.Id == existingTeacher.Id)
                    otherDepartment.Head = null;
            }

            if (!existingDepartment.Teachers.Any(t => t.Id == existingTeacher.Id))
                existingDepartment.Teachers.Add(existingTeacher);

            existingTeacher.Department = existingDepartment.Name;
        }

        public bool RemoveTeacher(Department department, Teacher teacher)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            Department existingDepartment = _dataStore.Departments.FirstOrDefault(d => d.Id == department.Id);
            Teacher existingTeacher = _dataStore.Teachers.FirstOrDefault(t => t.Id == teacher.Id);

            if (existingDepartment == null || existingTeacher == null)
                return false;

            bool removed = existingDepartment.Teachers.RemoveAll(t => t.Id == existingTeacher.Id) > 0;

            if (!removed)
                return false;

            if (existingDepartment.Head != null && existingDepartment.Head.Id == existingTeacher.Id)
                existingDepartment.Head = null;

            if (string.Equals(existingTeacher.Department, existingDepartment.Name, StringComparison.OrdinalIgnoreCase))
                existingTeacher.Department = string.Empty;

            return true;
        }

        public void SetHead(Department department, Teacher teacher)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            Department existingDepartment = _dataStore.Departments.FirstOrDefault(d => d.Id == department.Id);
            Teacher existingTeacher = _dataStore.Teachers.FirstOrDefault(t => t.Id == teacher.Id);

            if (existingDepartment == null || existingTeacher == null)
                throw new InvalidOperationException("Department or teacher does not exist.");

            if (!existingDepartment.Teachers.Any(t => t.Id == existingTeacher.Id))
                throw new InvalidOperationException("The department head must be a member of this department.");

            existingDepartment.Head = existingTeacher;
        }

        public bool RemoveHead(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            Department existingDepartment = _dataStore.Departments.FirstOrDefault(d => d.Id == department.Id);

            if (existingDepartment == null || existingDepartment.Head == null)
                return false;

            existingDepartment.Head = null;
            return true;
        }
    }
}
