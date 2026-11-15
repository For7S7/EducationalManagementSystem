using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model.Grading
{
    public class GradingRange
    {
        public int MinValue { get; }
        public int MaxValue { get; }
        public string LetterValue { get; }
        public bool IsPassing { get; }

        public GradingRange(
            int minValue,
            int maxValue,
            string letterValue,
            bool isPassing)
        {
            if (minValue < 0 || maxValue > 100 || minValue > maxValue)
                throw new ArgumentOutOfRangeException();

            if (string.IsNullOrWhiteSpace(letterValue))
                throw new ArgumentException("Letter value cannot be empty.");

            MinValue = minValue;
            MaxValue = maxValue;
            LetterValue = letterValue;
            IsPassing = isPassing;
        }

        public bool Contains(double numericValue)
        {
            return numericValue >= MinValue && numericValue <= MaxValue;
        }
    }
}
