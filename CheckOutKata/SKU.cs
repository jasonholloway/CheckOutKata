using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckOutKata
{

    public struct SKU : IComparable<SKU>, IComparable, IEquatable<SKU>
    {
        public readonly char Char;

        public SKU(char @char) {
            Char = @char;
        }


        #region Overrides

        public int CompareTo(SKU other)
            => Char.CompareTo(other.Char);

        public int CompareTo(object obj)
            => obj != null && obj is SKU
                ? CompareTo((SKU)obj)
                : throw new InvalidOperationException();

        public bool Equals(SKU other)        
            => other.Char.Equals(Char);         

        public override int GetHashCode()
            => Char.GetHashCode();

        public override bool Equals(object obj)
            => obj != null && obj is SKU
                ? Equals((SKU)obj)
                : false;

        public static bool operator ==(SKU a, SKU b)
            => a.Equals(b);

        public static bool operator !=(SKU a, SKU b)
            => !a.Equals(b);

        #endregion

    }

}
