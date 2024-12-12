namespace Utils.ReactiveValue
{
    public class ReactiveFlag : ReactiveValue<bool>
    {
        public ReactiveFlag(bool initialValue = default) 
            : base(initialValue)
        {
            
        }
    }
}