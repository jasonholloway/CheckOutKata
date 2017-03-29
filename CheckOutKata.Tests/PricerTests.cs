using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CheckOutKata.Tests
{

    public interface IPricer
    {
        decimal GetPrice(IEnumerable<SKU> skus);
    }
    


    //public delegate bool Offer(Context x);

    public interface IOffer
    {
        bool TryApply(Context x);
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




    public class Context
    {
        public Stack<SKU> SKUs = new Stack<SKU>(); //simpler to use Stack for time being
        public decimal TotalPrice = 0M;
    }


    public class Pricer : IPricer
    {
        IOffer[] _offers;

        public Pricer(params IOffer[] offers) {
            _offers = offers;
        }

        public decimal GetPrice(IEnumerable<SKU> skus) {            
            var x = new Context {
                SKUs = new Stack<SKU>(skus.OrderBy(s => s.Char))
            };

            while(x.SKUs.Any()) {
                var _ = _offers.FirstOrDefault(o => o.TryApply(x)) //enumerates through offers, executing them till one returns true
                            ?? throw new InvalidOperationException("No suitable offer found for SKU list!");
            }                                       

            return x.TotalPrice;
        }
    }

    
    


    public class PricerTests
    {

        [Fact(DisplayName = "Pricer throws when no strategy")]
        public void Pricer_Throws_WhenNoStrategy() {
            var pricer = new Pricer();
            var skus = CreateSKUs('A', 'B', 'C');

            Should.Throw<InvalidOperationException>(() => {
                pricer.GetPrice(skus);
            });         
        }

        
        [Fact(DisplayName = "Pricer uses single PricingStrategy")]
        public void Pricer_UsesSinglePricingStrategy() {
            var pricer = new Pricer(new DummyOffer());            
            var skus = CreateSKUs('A', 'B', 'C', 'D');

            var price = pricer.GetPrice(skus);

            price.ShouldBe(6M);
        }


        class DummyOffer : IOffer
        {
            public bool TryApply(Context x) {
                if(x.SKUs.Pop().Char > 'B') x.TotalPrice += 2;
                else x.TotalPrice += 1;

                return true;
            }
        }
        




        [Fact(DisplayName = "Pricer uses multiple PricingStrategies")]
        public void Pricer_UsesMultiplePricingStrategies() 
        {
            var pricer = new Pricer(
                            CreateOffer('A', 1),
                            CreateOffer('B', 2),
                            CreateOffer('C', 3),
                            CreateOffer('D', 4)
                            );

            var skus = CreateSKUs('A', 'A', 'C', 'D');

            var price = pricer.GetPrice(skus);

            price.ShouldBe(9);
        }


        [Fact(DisplayName = "Pricer sorts SKUs before passing them to strategies")]
        public void Pricer_SortsSKUs() 
        {
            var pricer = new Pricer(    //below acts as spy
                            new AdHocOffer(x => {
                                x.SKUs.ShouldBeInAnyOrder(); //the direction of the ordering doesn't matter to the strategies, 
                                                             //as long as they get chance to greedily consume grouped SKUs (which any ordering will do)
                                x.SKUs.Clear();                                         
                                return true;
                            }));
            
            pricer.GetPrice(CreateSKUs('D', 'A', 'C', 'D'));
        }





        [Fact(DisplayName = "MultiBuyOffer picks out specified number of same SKU, yields to lower strategy if too few")]
        public void MultiBuyOffer_PicksSpecifiedNumberOfSKU() {
            var pricer = new Pricer(
                            new MultiBuyOffer(new SKU('A'), 3, 10M),
                            new SingleItemOffer(new SKU('A'), 4M)
                            );

            var price = pricer.GetPrice(CreateSKUs('A', 'A', 'A', 'A'));

            price.ShouldBe(14M);            
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

            public bool TryApply(Context x) 
            {                
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




        #region bits

        SKU[] CreateSKUs(params char[] chars)
            => chars.Select(c => new SKU(c)).ToArray();

        
        IOffer CreateOffer(char @char, decimal price)
            => CreateOffer(sku => sku.Char == @char, price);

        IOffer CreateOffer(Func<SKU, bool> predicate, decimal price)
            => new AdHocOffer(x => {
                    if(predicate(x.SKUs.Peek())) {
                        x.SKUs.Pop();
                        x.TotalPrice += price;
                        return true;
                    }
                    else {                    
                        return false;
                    }
                });




        #endregion


    }
}
