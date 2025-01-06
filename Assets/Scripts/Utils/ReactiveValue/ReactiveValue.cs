using System;

namespace Utils.ReactiveValue
{
    public class ReactiveValue<T>
        where T : struct
    {
        public event Action<T, T> ValueChanged;

        public ReactiveValue(T initialValue = default)
        {
            _value = initialValue;
        }
        
        private T _value;
        
        public T Value
        {
            get => _value;
            set => SetValue(value);
        }

        private void SetValue(T value)
        {
            if (_value.Equals(value)) return;

            var prevValue = _value;
            _value = value;

            ValueChanged?.Invoke(_value, prevValue);
        }
    }
}