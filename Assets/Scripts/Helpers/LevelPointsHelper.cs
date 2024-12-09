using System;

namespace Helpers
{
    public static class LevelPointsHelper
    {
        private const int MinLevelPoints = 250;
        
        public static int GetExpPointsForLevel(int targetLevel)
        {
            var targetLevelIndex = targetLevel - 1;
            
            if (targetLevelIndex <= 0)
            {
                return 0;
            }

            const int firstTerm = MinLevelPoints;
            var lastTerm = firstTerm + (targetLevelIndex - 1) * 50;
            var totalPoints = targetLevelIndex * (firstTerm + lastTerm) / 2;

            return totalPoints;
        }
        
        public static int GetLevelByExpPointsAmount(int reachedPoints)
        {
            if (reachedPoints < 0)
            {
                throw new ArgumentException("Reached points must be non-negative.");
            }

            // Binary search to find the level
            var low = 0;
            var high = reachedPoints / MinLevelPoints; // A rough upper bound for the level

            while (low <= high)
            {
                var mid = (low + high) / 2;
                var pointsForLevel = GetExpPointsForLevel(mid);

                if (pointsForLevel <= reachedPoints)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }

            var levelIndex = high;
            return levelIndex + 1;
        }
    }
}