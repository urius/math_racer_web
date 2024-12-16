using System;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEngine.Assertions;
#endif

namespace Helpers
{
    public static class LevelPointsHelper
    {
        private const int MinLevelPoints = 300;
        
        public static int GetExpPointsForLevel(int targetLevel)
        {
            var targetLevelIndex = targetLevel - 1;
            
            if (targetLevelIndex <= 0)
            {
                return 0;
            }

            const int firstTerm = MinLevelPoints;
            var lastTerm = firstTerm + (targetLevelIndex - 1) * 100;
            var totalPoints = targetLevelIndex * (firstTerm + lastTerm) / 2;

            return totalPoints;
        }
        
        public static int GetLevelByExpPointsAmount(int reachedPoints)
        {
            if (reachedPoints < 0)
            {
                throw new ArgumentException("Reached points must be non-negative.");
            }

            const int startLevelToCheck = 1;

            for (var i = 0; i < 1000; i++)
            {
                var levelToCheck = startLevelToCheck + i;
                var expPointsForLevel = GetExpPointsForLevel(levelToCheck);
                if (reachedPoints < expPointsForLevel)
                {
                    return levelToCheck - 1;
                }
            }
            
            //1 2   3   4
            //0 250 550 1050
            
            return -1;
        }

#if UNITY_EDITOR
        public static void TestLevelPointsHelper()
        {
            for (var level = 1; level < 300; level++)
            {
                var expPointsForLevel = LevelPointsHelper.GetExpPointsForLevel(level);
                Debug.Log("exp for level " + level + " : " + expPointsForLevel);
                if (expPointsForLevel > 0)
                {
                    var expMinusPoints = expPointsForLevel - 1;
                    var prevLevel = LevelPointsHelper.GetLevelByExpPointsAmount(expMinusPoints);
                    Debug.Log("level fo exp points " + expMinusPoints + " : " + prevLevel);
                    Assert.AreEqual(prevLevel, level - 1);
                }

                var sameLevel = LevelPointsHelper.GetLevelByExpPointsAmount(expPointsForLevel);
                Debug.Log("level fo exp points " + expPointsForLevel + " : " + sameLevel);
                Assert.AreEqual(sameLevel, level);

                var expPlusPoints = expPointsForLevel + 1;
                var sameLevel2 = LevelPointsHelper.GetLevelByExpPointsAmount(expPlusPoints);
                Debug.Log("level fo exp points " + expPlusPoints + " : " + sameLevel);
                Assert.AreEqual(sameLevel, sameLevel2);
            }
        }
#endif
    }
}