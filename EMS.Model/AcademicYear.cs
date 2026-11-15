using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model
{
    public class AcademicYear
    {
        public int StartYear { get; }
        public int EndYear { get; }

        public AcademicYear(int startYear)
        {
            if (startYear < 1900)
            {
                throw new ArgumentOutOfRangeException(nameof(startYear));
            }

            StartYear = startYear;
            EndYear = startYear + 1;
        }

        public static AcademicYear FromDate(DateTime date)
        {
            int startYear = date.Month >= 9
                ? date.Year
                : date.Year - 1;

            return new AcademicYear(startYear);
        }

        public override string ToString()
        {
            return $"{StartYear}/{EndYear}";
        }
    }
}
