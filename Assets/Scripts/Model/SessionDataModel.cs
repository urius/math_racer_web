using Utils.ReactiveValue;

namespace Model
{
    public class SessionDataModel
    {
        public readonly ReactiveFlag IsBankPopupOpened = new();
    }
}