
using EMS.Model;
using EMS.Persistence.JSON.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Persistence.JSON
{
    public class DataStoreMapper
    {
        public DataStoreDTO ToDto(IDataStore dataStore)
        {
            if (dataStore == null)
                throw new ArgumentNullException(nameof(dataStore));

            DataStoreDTO dto = new DataStoreDTO();

            dto.Students = dataStore.Students.Select(s => new StudentDTO
            {
                Id = s.Id,
                FullName = s.FullName,
                Group = s.Group,
                AdmissionYear = s.AdmissionYear,
                SubjectIds = s.Subjects.Select(sub => sub.Id).ToList()
            }).ToList();

            dto.Teachers = dataStore.Teachers.Select(t => new TeacherDTO
            {
                Id = t.Id,
                FullName = t.FullName,
                Position = t.Position,
                Department = t.Department,
                SubjectIds = t.Subjects.Select(sub => sub.Id).ToList()
            }).ToList();

            dto.Subjects = dataStore.Subjects.Select(s => new SubjectDTO
            {
                Id = s.Id,
                Name = s.Name,
                Semester = s.Semester,
                Credits = s.Credits
            }).ToList();

            dto.Grades = dataStore.Grades.Select(g => new GradeDTO
            {
                Id = g.Id,
                StudentId = g.Student.Id,
                TeacherId = g.Teacher.Id,
                SubjectId = g.Subject.Id,
                NumericValue = g.NumericValue,
                LetterValue = g.LetterValue,
                Date = g.Date
            }).ToList();

            dto.Departments = dataStore.Departments.Select(d => new DepartmentDTO
            {
                Id = d.Id,
                Name = d.Name,
                HeadTeacherId = d.Head?.Id,
                TeacherIds = d.Teachers.Select(t => t.Id).ToList()
            }).ToList();

            return dto;
        }

        public void LoadFromDto(DataStoreDTO dto, IDataStore dataStore)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dataStore == null)
                throw new ArgumentNullException(nameof(dataStore));

            var subjectDtos = dto.Subjects ?? new List<SubjectDTO>();
            var studentDtos = dto.Students ?? new List<StudentDTO>();
            var teacherDtos = dto.Teachers ?? new List<TeacherDTO>();
            var gradeDtos = dto.Grades ?? new List<GradeDTO>();
            var departmentDtos = dto.Departments ?? new List<DepartmentDTO>();

            var subjects = subjectDtos.Select(s => new Subject
            {
                Id = s.Id,
                Name = s.Name,
                Semester = s.Semester,
                Credits = s.Credits
            }).ToDictionary(s => s.Id);

            var students = studentDtos.Select(s => new Student
            {
                Id = s.Id,
                FullName = s.FullName,
                Group = s.Group,
                AdmissionYear = s.AdmissionYear
            }).ToDictionary(s => s.Id);

            var teachers = teacherDtos.Select(t => new Teacher
            {
                Id = t.Id,
                FullName = t.FullName,
                Position = t.Position,
                Department = t.Department
            }).ToDictionary(t => t.Id);

            foreach (var studentDto in studentDtos)
            {
                Student student = students[studentDto.Id];

                foreach (Guid subjectId in studentDto.SubjectIds ?? new List<Guid>())
                {
                    if (!subjects.TryGetValue(subjectId, out Subject subject))
                        throw new InvalidOperationException(
                            $"Subject {subjectId} was not found.");

                    student.Subjects.Add(subject);
                }
            }

            foreach (var teacherDto in teacherDtos)
            {
                Teacher teacher = teachers[teacherDto.Id];

                foreach (Guid subjectId in teacherDto.SubjectIds ?? new List<Guid>())
                {
                    if (!subjects.TryGetValue(subjectId, out Subject subject))
                        throw new InvalidOperationException(
                            $"Subject {subjectId} was not found.");

                    teacher.Subjects.Add(subject);
                }
            }

            var grades = gradeDtos.Select(g =>
            {
                if (!students.TryGetValue(g.StudentId, out Student student))
                    throw new InvalidOperationException("Student was not found.");

                if (!teachers.TryGetValue(g.TeacherId, out Teacher teacher))
                    throw new InvalidOperationException("Teacher was not found.");

                if (!subjects.TryGetValue(g.SubjectId, out Subject subject))
                    throw new InvalidOperationException("Subject was not found.");

                return new Grade
                {
                    Id = g.Id,
                    Student = student,
                    Teacher = teacher,
                    Subject = subject,
                    NumericValue = g.NumericValue,
                    LetterValue = g.LetterValue,
                    Date = g.Date
                };
            }).ToList();

            // Restore departments and their relationships with teachers.
            var departments = new List<Department>();

            foreach (var departmentDto in departmentDtos)
            {
                Department department = new Department
                {
                    Id = departmentDto.Id,
                    Name = departmentDto.Name
                };

                foreach (Guid teacherId in departmentDto.TeacherIds ?? new List<Guid>())
                {
                    if (!teachers.TryGetValue(teacherId, out Teacher teacher))
                        throw new InvalidOperationException(
                            $"Teacher {teacherId} from department '{department.Name}' was not found.");

                    if (!department.Teachers.Any(t => t.Id == teacher.Id))
                        department.Teachers.Add(teacher);

                    teacher.Department = department.Name;
                }

                if (departmentDto.HeadTeacherId.HasValue)
                {
                    if (!teachers.TryGetValue(departmentDto.HeadTeacherId.Value, out Teacher head))
                        throw new InvalidOperationException(
                            $"Department head {departmentDto.HeadTeacherId.Value} was not found.");

                    department.Head = head;

                    if (!department.Teachers.Any(t => t.Id == head.Id))
                        department.Teachers.Add(head);

                    head.Department = department.Name;
                }

                departments.Add(department);
            }

            // Support old JSON files and keep Teacher.Department synchronized.
            // If a department was not saved yet, create it from the teacher's department name.
            foreach (Teacher teacher in teachers.Values)
            {
                if (string.IsNullOrWhiteSpace(teacher.Department))
                    continue;

                string departmentName = teacher.Department.Trim();
                Department department = departments.FirstOrDefault(d =>
                    string.Equals(d.Name, departmentName, StringComparison.OrdinalIgnoreCase));

                if (department == null)
                {
                    department = new Department { Name = departmentName };
                    departments.Add(department);
                }

                if (!department.Teachers.Any(t => t.Id == teacher.Id))
                    department.Teachers.Add(teacher);
            }

            dataStore.Students.Clear();
            dataStore.Teachers.Clear();
            dataStore.Subjects.Clear();
            dataStore.Grades.Clear();
            dataStore.Departments.Clear();

            foreach (var subject in subjects.Values)
                dataStore.Subjects.Add(subject);

            foreach (var student in students.Values)
                dataStore.Students.Add(student);

            foreach (var teacher in teachers.Values)
                dataStore.Teachers.Add(teacher);

            foreach (var grade in grades)
                dataStore.Grades.Add(grade);

            foreach (var department in departments)
                dataStore.Departments.Add(department);
        }
    }
}
