using Impeto.Framework.Exchange.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Exchange.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var k = new SalaDeReuniaoBS().getRoomLists();
            foreach (var item in k)
            {
                Console.WriteLine(item.ToString());
            }

        }
    }
}
