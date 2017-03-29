using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckOutKata
{

    public interface IPricer
    {
        decimal GetPrice(IEnumerable<SKU> skus);
    }



    public class Pricer : IPricer
    {
        IOffer[] _offers;

        public Pricer(params IOffer[] offers) {
            _offers = offers;
        }

        public decimal GetPrice(IEnumerable<SKU> skus) {
            var x = new Context {
                SKUs = new Stack<SKU>(skus.OrderBy(s => s))
            };

            while(x.SKUs.Any()) {
                var _ = _offers.FirstOrDefault(o => o.TryApply(x)) //enumerates through offers, executing them till one returns true
                            ?? throw new InvalidOperationException("No suitable offer found for SKU list!");
            }

            return x.TotalPrice;
        }
    }

}
