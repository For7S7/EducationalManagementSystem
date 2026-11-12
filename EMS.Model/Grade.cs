using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model
{
    public class Grade
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Subject Subject { get; set; }
        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
        public int NumericValue { get; set; }
        public string LetterValue { get; set; }
        public DateTime Date { get; set; }
    }
}
