using System;
using Data;

namespace Model.RaceScene
{
    public class RaceRewardsModel
    {
        private readonly int _distanceMeters;
        private readonly RaceResultsModel _raceResultsModel;
        private readonly ComplexityData _complexityData;

        private int _cashRewardDefault;
        private int _goldRewardDefault;

        private int _rewardsMultiplier = 1;

        public RaceRewardsModel(int distanceMeters, RaceResultsModel raceResultsModel, ComplexityData complexityData)
        {
            _distanceMeters = distanceMeters;
            _raceResultsModel = raceResultsModel;
            _complexityData = complexityData;
            
            UpdateRewards();
        }

        public int CashReward => _cashRewardDefault * _rewardsMultiplier;
        public int GoldReward => _goldRewardDefault * _rewardsMultiplier;
        public int ExpReward => CashReward;

        public void EnableDoubleRewards()
        {
            _rewardsMultiplier = 2;
        }

        public void UpdateRewards()
        {
            UpdateCashRewardDefault();
            UpdateGoldRewardDefault();
        }

        private void UpdateCashRewardDefault()
        {
            _cashRewardDefault = (_distanceMeters / 10 + _raceResultsModel.PlayerSpeed + _raceResultsModel.TurboBoostsCount * 10) *
                   (int)Math.Ceiling(_complexityData.ComplexityLevel / 10f);
        }

        private void UpdateGoldRewardDefault()
        {
            var result = 0;

            result += _raceResultsModel.IsFirst ? 1 : 0;
            result += _raceResultsModel.WrongAnswersCount <= 0 ? 1 : 0;

            _goldRewardDefault = result;
        }
    }
}