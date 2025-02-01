using UnityEngine;

namespace Events
{
    public struct UIRequestMoneyFlyAnimationEvent
    {
        public readonly int CashRewardAmount;
        public readonly int GoldRewardAmount;
        public readonly Vector3 ItemWorldPosition;
        
        public UIRequestMoneyFlyAnimationEvent(int cashRewardAmount, int goldRewardAmount, Vector3 itemWorldPosition)
        {
            CashRewardAmount = cashRewardAmount;
            GoldRewardAmount = goldRewardAmount;
            ItemWorldPosition = itemWorldPosition;
        }
    }
}