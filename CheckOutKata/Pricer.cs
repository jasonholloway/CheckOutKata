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
        Func<IEnumerable<IOffer>> _fnOffers;
                
        public Pricer(Func<IEnumerable<IOffer>> fnOffers) {
            _fnOffers = fnOffers;
        }

        public Pricer(params IOffer[] offers)
            : this(() => offers) { }


        public decimal GetPrice(Basket basket) {
            var x = new Context {
                Basket = basket,
                SKUs = new Stack<SKU>(basket.SKUs.OrderBy(s => s))
            };

            var offers = _fnOffers();

            while(x.SKUs.Any()) {
                //enumerates through offers, executing them till one returns true
                var found = offers.FirstOrDefault(o => o.TryApply(x)) != null; 

                if(!found) {
                    throw new InvalidOperationException("No suitable offer found for SKU list!");
                }                    
            }

            return x.TotalPrice;
        }
    }

}
