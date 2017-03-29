using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckOutKata.Tests
{
    public static class Extensions
    {

        enum Direction
        {
            None,
            Ascending,
            Descending
        }

        static Direction GetDirectionOfOrdering<T>(this T last, T next)
            where T : IComparable 
        {
            var difference = next.CompareTo(last);

            return difference < 0 ? Direction.Descending
                    : (difference > 0 ? Direction.Ascending
                    : Direction.None);
        }




        public static void ShouldBeInAnyOrder<T>(this IEnumerable<T> @this)
            where T : IComparable 
        {
            T last = default(T);
            Direction? lastDirection = null;
            bool first = true;

            foreach(var item in @this) {
                if(!first) {
                    var direction = item.GetDirectionOfOrdering(last);

                    if(direction != 0 && lastDirection.HasValue && direction != lastDirection) {
                        throw new InvalidOperationException("Not in consistent order!");
                    }

                    if(direction != 0 && !lastDirection.HasValue) {
                        lastDirection = direction;
                    }
                }

                last = item;
                first = false;
            }
        }


        public static void ShouldBeInAscendingOrder<T>(this IEnumerable<T> @this)
            where T : IComparable 
        {
            T last = default(T);
            bool first = true;

            foreach(var item in @this) {
                if(!first && item.CompareTo(last) < 0) {
                    throw new InvalidOperationException("Not in ascending order!");
                }

                last = item;
                first = false;
            }
        }



        public static void ShouldBeInDescendingOrder<T>(this IEnumerable<T> @this)
            where T : IComparable 
        {
            T last = default(T);
            bool first = true;

            foreach(var item in @this) {
                if(!first && item.CompareTo(last) > 0) {
                    throw new InvalidOperationException("Not in descending order!");
                }

                last = item;
                first = false;
            }
        }


    }
}
