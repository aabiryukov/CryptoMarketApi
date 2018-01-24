using System;

namespace Bitfinex.Converters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BitfinexPropertyAttribute: Attribute
    {
        public int Index { get; }

        public BitfinexPropertyAttribute(int index)
        {
            Index = index;
        }
    }
}
