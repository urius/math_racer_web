using System.Collections.Generic;

namespace Data
{
    public struct ComplexityData
    {
        public readonly int MaxNumbersCount;
        public readonly IReadOnlyList<string> AvailableOperators;
        public readonly int MaxNumberValueForEasyOperations;
        public readonly int MaxNumberValueForHardOperations;
        public readonly int MaxDivideOperatorsCount;

        public ComplexityData(
            int maxNumbersCount,
            IReadOnlyList<string> availableOperators,
            int maxNumberValueForEasyOperations,
            int maxNumberValueForHardOperations, 
            int maxDivideOperatorsCount)
        {
            MaxNumbersCount = maxNumbersCount;
            AvailableOperators = availableOperators;
            MaxNumberValueForEasyOperations = maxNumberValueForEasyOperations;
            MaxNumberValueForHardOperations = maxNumberValueForHardOperations;
            MaxDivideOperatorsCount = maxDivideOperatorsCount;
        }
    }
}