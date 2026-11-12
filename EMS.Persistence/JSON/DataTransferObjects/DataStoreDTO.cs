using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Persistence.JSON.DataTransferObjects
{
    public class DataStoreDTO
    {
        public List<StudentDTO> Students { get; set; } = new();
        public List<TeacherDTO> Teachers { get; set; } = new();
        public List<SubjectDTO> Subjects { get; set; } = new();
        public List<GradeDTO> Grades { get; set; } = new();

    }
}
