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


    public class Pricer : IPricer
    {
        public decimal GetPrice(IEnumerable<SKU> skus) {
            return 0M;
        }
    }




    public class PricerTests
    {        
        [Fact(DisplayName = "Pricer returns price")]
        public void Pricer_ReturnsPrice() {
            var pricer = new Pricer();
            var price = pricer.GetPrice(new[] { new SKU('A'), new SKU('B'), new SKU('C') });            
        }
                

    }
}
