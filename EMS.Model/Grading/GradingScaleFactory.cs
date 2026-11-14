using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model.Grading
{
    public static class GradingScaleFactory
    {
        public static IGradingScale CreateEctsScale()
        {
            return new GradingScale(new[]
            {
                new GradingRange(90, 100, "A"),
                new GradingRange(82, 89, "B"),
                new GradingRange(74, 81, "C"),
                new GradingRange(64, 73, "D"),
                new GradingRange(60, 63, "E"),
                new GradingRange(35, 59, "FX"),
                new GradingRange(0, 34, "F")
            });
        }

        public static IGradingScale CreateFivePointScale()
        {
            return new GradingScale(new[]
            {
                new GradingRange(90, 100, "5"),
                new GradingRange(74, 89, "4"),
                new GradingRange(60, 73, "3"),
                new GradingRange(0, 59, "2")
            });
        }
    }
}
