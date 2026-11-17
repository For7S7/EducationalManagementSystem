using System;
using System.Collections.Generic;

namespace EMS.Model
{
    public class Department
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public Teacher? Head { get; set; }
        public List<Teacher> Teachers { get; set; }

        public Department()
        {
            Teachers = new List<Teacher>();
        }
    }
}