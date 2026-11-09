using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model
{
    public interface IDataStore
    {
        ICollection<Student> Students { get; }
        ICollection<Teacher> Teachers { get; }
        ICollection<Subject> Subjects { get; }
        ICollection<Grade> Grades { get; }
    }
}
