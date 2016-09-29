namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public struct Minutes
    {
        private readonly decimal _value;

        public Minutes(decimal value)
        {
            _value = value;
        }

        public static implicit operator decimal(Minutes minutes)
        {
            return minutes._value;
        }

        public static implicit operator double(Minutes minutes)
        {
            return (double)minutes._value;
        }

        public static implicit operator Minutes(decimal value)
        {
            return new Minutes(value);
        }

        public override string ToString()
        {
            return _value.ToString("F");
        }
    }
}