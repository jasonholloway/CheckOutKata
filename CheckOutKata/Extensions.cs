using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckOutKata
{
    public static class Extensions
    {
        public static void PopMany<T>(this Stack<T> @this, int count) {
            for(int i = 0; i < count; i++) {
                if(!@this.Any()) return;
                else @this.Pop();
            }
        }
            

    }
}
