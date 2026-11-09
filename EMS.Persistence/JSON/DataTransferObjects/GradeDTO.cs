using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Persistence.JSON.DataTransferObjects
{
    public class GradeDTO
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        public Guid SubjectId { get; set; }
        public int NumericValue { get; set; }
        public string LetterValue { get; set; }
        public DateTime Date { get; set; }

    }
}
