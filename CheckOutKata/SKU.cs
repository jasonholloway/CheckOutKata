using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckOutKata
{

    public struct SKU : IComparable<SKU>, IComparable
    {
        public readonly char Char;

        public SKU(char @char) {
            Char = @char;
        }

        //equality operators to be overridden here
        //...

        public int CompareTo(SKU other)
            => Char.CompareTo(other.Char);

        public int CompareTo(object obj)
            => obj != null && obj is SKU
                ? CompareTo((SKU)obj)
                : throw new InvalidOperationException();
    }

}
