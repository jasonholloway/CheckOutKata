using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckOutKata
{
    
    public interface IOffer
    {
        bool TryApply(Context x);
    }


    public class Context
    {
        public Stack<SKU> SKUs = new Stack<SKU>(); //simpler to use Stack for time being
        public decimal TotalPrice = 0M;
    }



    public class SingleItemOffer : IOffer
    {
        SKU _sku;
        decimal _price;

        public SingleItemOffer(SKU sku, decimal price) {
            _sku = sku;
            _price = price;
        }

        public bool TryApply(Context x) {
            if(x.SKUs.Peek().Char == _sku.Char) {   //should do SKU.Equals()...
                x.SKUs.Pop();
                x.TotalPrice += _price;
                return true;
            }

            return false;
        }
    }

    public class AdHocOffer : IOffer
    {
        Func<Context, bool> _fn;

        public AdHocOffer(Func<Context, bool> fn) {
            _fn = fn;
        }

        public bool TryApply(Context x) {
            return _fn(x);
        }
    }


    public class MultiBuyOffer : IOffer
    {
        SKU _sku;
        int _quantity;
        decimal _price;

        public MultiBuyOffer(SKU sku, int quantity, decimal price) {
            _sku = sku;
            _quantity = quantity;
            _price = price;
        }

        public bool TryApply(Context x) {
            var span = x.SKUs.TakeWhile(s => s.Char == _sku.Char).ToArray();

            if(span.Length >= _quantity) {
                x.TotalPrice += _price;
                x.SKUs.PopMany(_quantity);
                return true;
            }
            else {
                return false;
            }
        }
    }

}
