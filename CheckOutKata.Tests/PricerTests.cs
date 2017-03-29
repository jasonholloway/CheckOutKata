using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CheckOutKata.Tests
{

    public struct SKU
    {
        public readonly char Char;

        public SKU(char @char) {
            Char = @char;
        }
    }


    public interface IPricer
    {
        decimal GetPrice(IEnumerable<SKU> skus);
    }



    public delegate bool PricingStrategy(Context x);


    public class Context
    {
        public Stack<SKU> SKUs = new Stack<SKU>(); //simpler to use Stack for time being
        public decimal TotalPrice = 0M;
    }


    public class Pricer : IPricer
    {
        PricingStrategy[] _strategies;

        public Pricer(params PricingStrategy[] strategies) {
            _strategies = strategies;
        }

        public decimal GetPrice(IEnumerable<SKU> skus) {            
            var x = new Context {
                SKUs = new Stack<SKU>(skus)
            };

            while(x.SKUs.Any()) {
                _strategies.First(fn => fn(x)); //enumerates through strategies, executing them till one returns true
            }                                   //this will also throw if no strategy found

            return x.TotalPrice;
        }
    }

    
    


    public class PricerTests
    {        
        [Fact(DisplayName = "Pricer throws when no strategy")]
        public void Pricer_Throws_WhenNoStrategy() {
            var pricer = new Pricer();

            Should.Throw<InvalidOperationException>(() => {
                pricer.GetPrice(new[] { new SKU('A'), new SKU('B'), new SKU('C') });
            });         
        }

        
        [Fact(DisplayName = "Pricer uses single PricingStrategy")]
        public void Pricer_UsesSinglePricingStrategy() {
            var pricer = new Pricer(DummyStrategy);

            var price = pricer.GetPrice(new[] { new SKU('A'), new SKU('B'), new SKU('C'), new SKU('D') });

            price.ShouldBe(6M);
        }


        static bool DummyStrategy(Context x) {
            if(x.SKUs.Pop().Char > 'B') x.TotalPrice += 2;
            else x.TotalPrice += 1;
            
            return true;
        }




    }
}
