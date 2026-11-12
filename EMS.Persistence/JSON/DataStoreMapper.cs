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

            return dto;
        }

        public void LoadFromDto(DataStoreDTO dto, IDataStore dataStore)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dataStore == null)
                throw new ArgumentNullException(nameof(dataStore));

            var subjects = dto.Subjects.Select(s => new Subject
            {
                Id = s.Id,
                Name = s.Name,
                Semester = s.Semester,
                Credits = s.Credits
            }).ToDictionary(s => s.Id);

            var students = dto.Students.Select(s => new Student
            {
                Id = s.Id,
                FullName = s.FullName,
                Group = s.Group,
                AdmissionYear = s.AdmissionYear
            }).ToDictionary(s => s.Id);

            var teachers = dto.Teachers.Select(t => new Teacher
            {
                Id = t.Id,
                FullName = t.FullName,
                Department = t.Department
            }).ToDictionary(t => t.Id);

            foreach (var studentDto in dto.Students)
            {
                Student student = students[studentDto.Id];

                foreach (Guid subjectId in studentDto.SubjectIds)
                {
                    if (!subjects.TryGetValue(subjectId, out Subject subject))
                        throw new InvalidOperationException(
                            $"Subject {subjectId} was not found.");

                    student.Subjects.Add(subject);
                }
            }

            foreach (var teacherDto in dto.Teachers)
            {
                Teacher teacher = teachers[teacherDto.Id];

                foreach (Guid subjectId in teacherDto.SubjectIds)
                {
                    if (!subjects.TryGetValue(subjectId, out Subject subject))
                        throw new InvalidOperationException(
                            $"Subject {subjectId} was not found.");

                    teacher.Subjects.Add(subject);
                }
            }

            var grades = dto.Grades.Select(g =>
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

            dataStore.Students.Clear();
            dataStore.Teachers.Clear();
            dataStore.Subjects.Clear();
            dataStore.Grades.Clear();

            foreach (var subject in subjects.Values)
                dataStore.Subjects.Add(subject);

            foreach (var student in students.Values)
                dataStore.Students.Add(student);

            foreach (var teacher in teachers.Values)
                dataStore.Teachers.Add(teacher);

            foreach (var grade in grades)
                dataStore.Grades.Add(grade);
        }
    }
}
