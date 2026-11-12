using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model
{
    public class Subject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int Semester {  get; set; }
        public int Credits { get; set; }
    }
}
