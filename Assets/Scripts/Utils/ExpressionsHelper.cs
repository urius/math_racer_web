using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Data;

namespace Utils
{
    public static class ExpressionsHelper
    {
        public static string GenerateExpression(ComplexityData complexityData)
        {
            return GenerateExpression(
                complexityData.MaxNumbersCount,
                complexityData.AvailableOperators,
                complexityData.MaxNumberValueForEasyOperations,
                complexityData.MaxNumberValueForHardOperations,
                complexityData.MaxDivideOperatorsCount);
        }

        public static string GenerateExpression(
            int maxNumbersCount,
            IReadOnlyList<string> availableOperators,
            int maxNumberEasyOperatorValue,
            int maxNumberHardOperatorValue,
            int maxDivideOperatorsCount)
        {
            if (maxNumbersCount < 2 || availableOperators == null || availableOperators.Count == 0)
            {
                throw new ArgumentException("Invalid input parameters.");
            }

            var random = new Random();
            var numbersCount = random.Next(2, maxNumbersCount);
            //Debug.Log("maxNumbersCount: " + maxNumbersCount + " numbersCount: " + numbersCount);
            var expression = new StringBuilder();
            var openBracesCounter = 0;
            var operatorBuffer = string.Empty;
            var availableOperatorsWithoutDivide =
                availableOperators.Where(o => o != Constants.OperatorDivide).ToArray();
            var divideOperatorsCount = 0;

            for (var i = 0; i < numbersCount; i++)
            {
                var maxNumberValue = IsHardOperator(operatorBuffer) || openBracesCounter > 0
                    ? maxNumberHardOperatorValue
                    : maxNumberEasyOperatorValue;
                
                expression.Append(random.Next(1, maxNumberValue));
                
                if (openBracesCounter > 0)
                {
                    openBracesCounter--;
                    if (openBracesCounter <= 0)
                    {
                        expression.Append(")");
                    }
                }

                if (i < numbersCount - 1)
                {
                    operatorBuffer = TakeRandomElement(divideOperatorsCount < maxDivideOperatorsCount
                        ? availableOperators
                        : availableOperatorsWithoutDivide, random);
                    
                    expression.Append(operatorBuffer);

                    if (operatorBuffer == Constants.OperatorMultiply
                        && numbersCount - i > 2
                        && openBracesCounter <= 0
                        && random.NextDouble() < 0.9f)
                    {
                        openBracesCounter = random.Next(2, numbersCount - i);
                        expression.Append("(");
                    }
                    else if (operatorBuffer == Constants.OperatorDivide)
                    {
                        divideOperatorsCount++;
                    }
                }
            }

            return expression.ToString();
        }

        public static double EvaluateExpression(string expression)
        {
            var result = new DataTable().Compute(expression, string.Empty);
            
            return Convert.ToDouble(result);
        }

        private static string TakeRandomElement(IReadOnlyList<string> availableOperators, Random random)
        {
            return availableOperators[random.Next(availableOperators.Count)];
        }

        private static bool IsHardOperator(string operatorStr)
        {
            return operatorStr is Constants.OperatorDivide or Constants.OperatorMultiply;
        }
    }
}