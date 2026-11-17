using System;
using System.Collections.Generic;

namespace EMS.Persistence.JSON.DataTransferObjects
{
    public class DepartmentDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? HeadTeacherId { get; set; }
        public List<Guid> TeacherIds { get; set; } = new();
    }
}