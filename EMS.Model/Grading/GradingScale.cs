using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EMS.Model.Grading
{
    public class GradingScale : IGradingScale
    {
        private readonly List<GradingRange> _ranges;

        public GradingScale(IEnumerable<GradingRange> ranges)
        {
            if (ranges == null)
                throw new ArgumentNullException(nameof(ranges));

            List<GradingRange> rangeList = ranges.ToList();

            if (!rangeList.Any())
                throw new ArgumentException("Grading scale cannot be empty.");

            if (rangeList.Any(r => r == null))
                throw new ArgumentException("Grading ranges cannot contain null values.");

            _ranges = rangeList.OrderBy(r => r.MinValue).ToList();

            if (_ranges[0].MinValue != 0)
                throw new ArgumentException("Grading scale must start at 0.");

            if (_ranges[_ranges.Count - 1].MaxValue != 100)
                throw new ArgumentException("Grading scale must end at 100.");

            for (int i = 1; i < _ranges.Count; i++)
            {
                if (_ranges[i].MinValue != _ranges[i - 1].MaxValue + 1)
                    throw new ArgumentException(
                        "Grading ranges must be continuous and cannot overlap.");
            }
        }

        public string CalculateLetterGrade(int numericValue)
        {
            if (numericValue < 0 || numericValue > 100)
                throw new ArgumentOutOfRangeException(nameof(numericValue));

            GradingRange range = _ranges.FirstOrDefault(r => r.Contains(numericValue));

            if (range == null)
                throw new InvalidOperationException(
                    "No grading range found for this value.");

            return range.LetterValue;
        }
    }
}
