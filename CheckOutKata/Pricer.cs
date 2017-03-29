using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckOutKata
{

    public interface IPricer
    {
        decimal GetPrice(Basket basket);
    }


    public class Basket
    {
        public readonly IEnumerable<SKU> SKUs;

        public Basket(IEnumerable<SKU> skus) {
            SKUs = skus;
        }
    }



    public class Pricer : IPricer
    {
        IOffer[] _offers;

        public Pricer(params IOffer[] offers) {
            _offers = offers;
        }

        public decimal GetPrice(Basket basket) {
            var x = new Context {
                Basket = basket,
                SKUs = new Stack<SKU>(basket.SKUs.OrderBy(s => s))
            };

            while(x.SKUs.Any()) {
                var _ = _offers.FirstOrDefault(o => o.TryApply(x)) //enumerates through offers, executing them till one returns true
                            ?? throw new InvalidOperationException("No suitable offer found for SKU list!");
            }

            return x.TotalPrice;
        }
    }

}
