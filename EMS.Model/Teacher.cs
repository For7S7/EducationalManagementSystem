using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model
{
    public class Teacher
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; }
        public string Department { get; set; }
        public List<Subject> Subjects { get; set; }

        public Teacher()
        {
            Subjects = new List<Subject>();
        }
    }
}
