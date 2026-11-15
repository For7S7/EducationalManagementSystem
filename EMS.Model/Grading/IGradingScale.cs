using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Model.Grading
{
    public interface IGradingScale
    {
        string CalculateLetterGrade(int numericValue);
        bool IsPassingGrade(double numericValue);
    }
}
