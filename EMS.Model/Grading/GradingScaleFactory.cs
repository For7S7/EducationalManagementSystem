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
                new GradingRange(90, 100, "A", true),
                new GradingRange(82, 89, "B", true),
                new GradingRange(74, 81, "C", true),
                new GradingRange(64, 73, "D", true),
                new GradingRange(60, 63, "E", true),
                new GradingRange(35, 59, "FX", false),
                new GradingRange(0, 34, "F", false)
            });
        }

        public static IGradingScale CreateFivePointScale()
        {
            return new GradingScale(new[]
            {
                new GradingRange(90, 100, "5", true),
                new GradingRange(74, 89, "4", true),
                new GradingRange(60, 73, "3", true),
                new GradingRange(0, 59, "2", false)
            });
        }
    }
}
