using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model
{
    public class Student
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; }
        public string Group { get; set; }
        public int AdmissionYear { get; set; }
        public List<Subject> Subjects { get; set; }

        public Student()
        {
            Subjects = new List<Subject>();
        }
    }
}
