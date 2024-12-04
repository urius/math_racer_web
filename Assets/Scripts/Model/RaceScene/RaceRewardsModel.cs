using System;
using Data;

namespace Model.RaceScene
{
    public class RaceRewardsModel
    {
        private readonly int _cashRewardDefault;
        private readonly int _goldRewardDefault;

        private int _rewardsMultiplier = 1;

        public RaceRewardsModel(int distanceMeters, RaceResultsModel raceResultsModel, ComplexityData complexityData)
        {
            _cashRewardDefault = GetCashRewardDefault(distanceMeters, raceResultsModel, complexityData);
            _goldRewardDefault = GetGoldRewardDefault(raceResultsModel, complexityData);
        }

        public int CashReward => _cashRewardDefault * _rewardsMultiplier;
        public int GoldReward => _goldRewardDefault * _rewardsMultiplier;

        public void EnableDoubleRewards()
        {
            _rewardsMultiplier = 2;
        }

        private static int GetCashRewardDefault(int distanceMeters, RaceResultsModel raceResultsModel,
            ComplexityData complexityData)
        {
            return (distanceMeters / 10 + raceResultsModel.PlayerSpeed + raceResultsModel.TurboBoostsCount * 50) *
                   (int)Math.Ceiling(complexityData.ComplexityLevel / 3f);
        }

        private static int GetGoldRewardDefault(RaceResultsModel raceResultsModel, ComplexityData complexityData)
        {
            var result = 0;

            result += raceResultsModel.IsFirst ? 1 : 0;
            result += raceResultsModel.WrongAnswersCount <= 0 ? 1 : 0;
            result += complexityData.ComplexityLevel / 5;

            return result;
        }
    }
}