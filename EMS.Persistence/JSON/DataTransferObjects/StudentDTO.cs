using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Persistence.JSON.DataTransferObjects
{
    public class StudentDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Group { get; set; }
        public int AdmissionYear { get; set; }
        public List<Guid> SubjectIds { get; set; } = new();
    }
}
