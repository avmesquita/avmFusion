using Impeto.Framework.Exchange.Models;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Impeto.Exchange.wcfsvcapp
{
    public class ImpetoExchangeService : IImpetoExchangeService
    {
        private const string AutoDiscoverURL = "https://outlook.office365.com/EWS/Exchange.asmx";


        public List<Impeto.Framework.Exchange.Models.ResultadoDisponibilidadeSalaReuniao> obterDisponibilidadeSalaReuniao()
        {
            var bs = new Impeto.Framework.Exchange.Service.SalaDeReuniaoBS();
            bs.email = "";
            bs.senha = "";
            bs.autodiscoverUrl = AutoDiscoverURL;

            var retorno = new Impeto.Framework.Exchange.Service.SalaDeReuniaoBS().obterDisponibilidadeSalaReuniao("");
            return retorno;
        }

        public List<Impeto.Framework.Exchange.Models.ResultadoDisponibilidadeSalaReuniao> obterDisponibilidadeSalaReuniao(string email, string senha)
        {
            var bs = new Impeto.Framework.Exchange.Service.SalaDeReuniaoBS();
            bs.email = email;
            bs.senha = senha;
            bs.autodiscoverUrl = AutoDiscoverURL;

            var retorno = new Impeto.Framework.Exchange.Service.SalaDeReuniaoBS().obterDisponibilidadeSalaReuniao("");
            return retorno;
        }


        public List<Impeto.Framework.Exchange.Models.ResultadoDisponibilidadeSalaReuniao> obterDisponibilidadeSalaReuniaoTimeZone(string timeZone = "")
        {
            var bs = new Impeto.Framework.Exchange.Service.SalaDeReuniaoBS();
            bs.email = "";
            bs.senha = "";
            bs.autodiscoverUrl = AutoDiscoverURL;

            if (timeZone == null)
            {
                timeZone = "";
            }
            var retorno = bs.obterDisponibilidadeSalaReuniao(timeZone);
            return retorno;
        }

        public List<Impeto.Framework.Exchange.Models.ResultadoDisponibilidadeSalaReuniao> obterDisponibilidadeTimeZoneFull(string email, string senha, string timeZone = "")
        {
            var bs = new Impeto.Framework.Exchange.Service.SalaDeReuniaoBS();
            bs.email = email;
            bs.senha = senha;
            bs.autodiscoverUrl = AutoDiscoverURL;

            if (timeZone == null)
            {
                timeZone = "";
            }
            var retorno = bs.obterDisponibilidadeSalaReuniao(timeZone);
            return retorno;
        }

        public Impeto.Framework.Exchange.Models.Status obterDisponibilidadeExchange(string email, string senha, string smtp, string timeZone = "")
        {
            var bs = new Impeto.Framework.Exchange.Service.SalaDeReuniaoBS();
            bs.email = email;
            bs.senha = senha;
            bs.autodiscoverUrl = AutoDiscoverURL;

            var retorno = bs.obterDisponibilidadeExchange(smtp);
            return retorno;
        }

        public EmailAddressCollection getBuildingRoomList(string email, string senha)
        {
            var bs = new Impeto.Framework.Exchange.Service.SalaDeReuniaoBS();
            bs.email = email;
            bs.senha = senha;
            bs.autodiscoverUrl = AutoDiscoverURL;

            var rooms = bs.getRoomLists();
            return rooms;
        }

        public Collection<EmailAddress> getRooms(string email, string senha, string smtpBuildingRoom)
        {
            var bs = new Impeto.Framework.Exchange.Service.SalaDeReuniaoBS();
            bs.email = email;
            bs.senha = senha;
            bs.autodiscoverUrl = AutoDiscoverURL;

            var rooms = bs.getRooms(smtpBuildingRoom);
            return rooms;        
        }
    }
}
