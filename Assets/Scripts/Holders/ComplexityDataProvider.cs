using System;
using System.Collections.Generic;
using Data;

namespace Holders
{
    public class ComplexityDataProvider : IComplexityDataProvider
    {
        public ComplexityData GetComplexityData(int playerLevel, int complexityLevel)
        {
            if (complexityLevel < 1) complexityLevel = 1;

            var availableOperators = new List<string>(4) { Constants.OperatorPlus };
            var maxNumbersCount = 2;

            if (complexityLevel > 2) availableOperators.Add(Constants.OperatorMinus);
            if (complexityLevel > 4) availableOperators.Add(Constants.OperatorMultiply);
            if (complexityLevel > 10) availableOperators.Add(Constants.OperatorDivide);

            var maxDivideOperators = complexityLevel % 8;

            if (complexityLevel > 3) maxNumbersCount++;
            if (complexityLevel > 9) maxNumbersCount++;
            if (complexityLevel > 11) maxNumbersCount++;
            if (complexityLevel > 15) maxNumbersCount++;
            if (complexityLevel > 19) maxNumbersCount++;

            var maxNumberValueForEasyOperations = 5 * playerLevel + 10 * Math.Max(0, complexityLevel - 5);
            var maxNumberValueForHardOperations = 2 + (int)(0.5f * playerLevel) + Math.Max(0, complexityLevel - 10);

            return new ComplexityData(
                maxNumbersCount,
                availableOperators,
                maxNumberValueForEasyOperations,
                maxNumberValueForHardOperations,
                maxDivideOperators);
        }
    }

    public interface IComplexityDataProvider
    {
        public ComplexityData GetComplexityData(int playerLevel, int complexityLevel);
    }
}