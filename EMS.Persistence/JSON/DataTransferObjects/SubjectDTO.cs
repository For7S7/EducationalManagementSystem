using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Persistence.JSON.DataTransferObjects
{
    public class SubjectDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Semester { get; set; }
        public int Credits { get; set; }
    }
}
