using System;
using System.Collections.Generic;
using System.Text;
using EMS.Model;

namespace EMS.Persistence
{
    public class InMemoryDataStore : IDataStore
    {
        public ICollection<Student> Students { get; } = new List<Student>();
        public ICollection<Teacher> Teachers { get; } = new List<Teacher>();
        public ICollection<Subject> Subjects { get; } = new List<Subject>();
        public ICollection<Grade> Grades { get; } = new List<Grade>();

    }
}
