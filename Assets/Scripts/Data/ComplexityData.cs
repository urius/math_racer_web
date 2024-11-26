using System.Collections.Generic;

namespace Data
{
    public struct ComplexityData
    {
        public readonly int ComplexityLevel;
        public readonly int MaxNumbersCount;
        public readonly IReadOnlyList<string> AvailableOperators;
        public readonly int MaxNumberValueForEasyOperations;
        public readonly int MaxNumberValueForHardOperations;
        public readonly int MaxDivideOperatorsCount;

        public ComplexityData(
            int complexityLevel,
            int maxNumbersCount,
            IReadOnlyList<string> availableOperators,
            int maxNumberValueForEasyOperations,
            int maxNumberValueForHardOperations,
            int maxDivideOperatorsCount)
        {
            ComplexityLevel = complexityLevel;
            MaxNumbersCount = maxNumbersCount;
            AvailableOperators = availableOperators;
            MaxNumberValueForEasyOperations = maxNumberValueForEasyOperations;
            MaxNumberValueForHardOperations = maxNumberValueForHardOperations;
            MaxDivideOperatorsCount = maxDivideOperatorsCount;
        }
    }
}