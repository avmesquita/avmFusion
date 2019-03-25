using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Impeto.Exchange.Configuration.Utils
{

    public class FusoHorario
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public FusoHorario()
        {

        }

    }
    public class FusoHorarioUtils
    {
        public List<FusoHorario> GetAllFusos()
        {
            List<FusoHorario> retorno = new List<FusoHorario>();

            System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> tz;
            tz = TimeZoneInfo.GetSystemTimeZones();

            foreach (var item in tz)
            {
                FusoHorario fuso = new Utils.FusoHorario();
                fuso.ID = item.Id;
                fuso.Name = item.DisplayName;
                retorno.Add(fuso);
            }
            return retorno;
        }
    }
}