using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CheckOutKata.Tests
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


    public interface IPricer
    {
        decimal GetPrice(IEnumerable<SKU> skus);
    }



    public delegate bool Offer(Context x);


    public class Context
    {
        public Stack<SKU> SKUs = new Stack<SKU>(); //simpler to use Stack for time being
        public decimal TotalPrice = 0M;
    }


    public class Pricer : IPricer
    {
        Offer[] _offers;

        public Pricer(params Offer[] offers) {
            _offers = offers;
        }

        public decimal GetPrice(IEnumerable<SKU> skus) {            
            var x = new Context {
                SKUs = new Stack<SKU>(skus.OrderBy(s => s.Char))
            };

            while(x.SKUs.Any()) {
                _offers.First(fn => fn(x)); //enumerates through offers, executing them till one returns true
            }                                   //this will also throw if no strategy found

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
            var pricer = new Pricer(DummyOffer);            
            var skus = CreateSKUs('A', 'B', 'C', 'D');

            var price = pricer.GetPrice(skus);

            price.ShouldBe(6M);
        }
        

        static bool DummyOffer(Context x) {
            if(x.SKUs.Pop().Char > 'B') x.TotalPrice += 2;
            else x.TotalPrice += 1;
            
            return true;
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
                            new Offer(x => {
                                x.SKUs.ShouldBeInAnyOrder(); //the direction of the ordering doesn't matter to the strategies, 
                                                             //as long as they get chance to greedily consume grouped SKUs (which any ordering will do)
                                x.SKUs.Clear();                                         
                                return true;
                            }));
            
            pricer.GetPrice(CreateSKUs('D', 'A', 'C', 'D'));
        }

        
        

        #region bits

        SKU[] CreateSKUs(params char[] chars)
            => chars.Select(c => new SKU(c)).ToArray();

        
        Offer CreateOffer(char @char, decimal price)
            => CreateOffer(sku => sku.Char == @char, price);

        Offer CreateOffer(Func<SKU, bool> predicate, decimal price)
            => (Context x) => {
                if(predicate(x.SKUs.Peek())) {
                    x.SKUs.Pop();
                    x.TotalPrice += price;
                    return true;
                }
                else {                    
                    return false;
                }
            };




        #endregion


    }
}
