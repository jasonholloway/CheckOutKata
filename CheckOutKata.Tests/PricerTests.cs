using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CheckOutKata.Tests
{
    
    public class PricerTests
    {

        [Fact(DisplayName = "Pricer throws when no strategy")]
        public void Pricer_Throws_WhenNoStrategy() {
            var pricer = new Pricer();
            var basket = CreateBasket('A', 'B', 'C');

            Should.Throw<InvalidOperationException>(() => {
                pricer.GetPrice(basket);
            });         
        }

        
        [Fact(DisplayName = "Pricer uses single PricingStrategy")]
        public void Pricer_UsesSinglePricingStrategy() {
            var pricer = new Pricer(new DummyOffer());            
            var basket = CreateBasket('A', 'B', 'C', 'D');

            var price = pricer.GetPrice(basket);

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

            var basket = CreateBasket('A', 'A', 'C', 'D');

            var price = pricer.GetPrice(basket);

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

            var basket = CreateBasket('D', 'A', 'C', 'D');

            pricer.GetPrice(basket);
        }





        [Fact(DisplayName = "MultiBuyOffer picks out specified number of same SKU, yields to lower strategy if too few")]
        public void MultiBuyOffer_PicksSpecifiedNumberOfSKU() {
            var pricer = new Pricer(
                            new MultiBuyOffer(new SKU('A'), 3, 10M),
                            new SingleItemOffer(new SKU('A'), 4M)
                            );

            var price = pricer.GetPrice(CreateBasket('A', 'A', 'A', 'A'));

            price.ShouldBe(14M);            
        }

        
        

        #region bits


        SKU[] CreateSKUs(params char[] chars)
            => chars.Select(c => new SKU(c)).ToArray();

        Basket CreateBasket(params char[] chars)
            => new Basket(CreateSKUs(chars));

        
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
