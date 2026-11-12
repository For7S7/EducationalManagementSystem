using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Persistence.JSON.DataTransferObjects
{
    public class TeacherDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public List<Guid> SubjectIds { get; set; } = new();

    }
}
