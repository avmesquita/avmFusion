using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impeto.Framework.Database.SQLite;

namespace Impeto.Framework.Database.SQLite.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteDatabase db = new SQLiteDatabase(".\\ImpetoExchange.s3db; Version=3;New=True;");

            var cliente = db.GetCliente(1);
            var dispositivo = db.GetDispositivo(1, 1, "cid");

            if ((cliente == null) && (dispositivo == null))
            {
                throw new Exception("Teste falhou!");
            }

        }
    }
}
