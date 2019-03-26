using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Microsoft.Exchange.WebServices.Data;
using System.Collections.ObjectModel;
using System.Web;
using Fusion.Framework.Exchange.Models;

namespace Fusion.Exchange.Web.Controllers
{
    public class ExchangeController : ApiController
    {
        [HttpGet]
        [Route("api/Exchange/obterDisponibilidadeSalaReuniao")]
        public List<ResultadoDisponibilidadeSalaReuniao> obterDisponibilidadeSalaReuniao(string zoneId = "")
        {
            var retorno = new Fusion.Framework.Exchange.Service.SalaDeReuniaoBS().obterDisponibilidadeSalaReuniao(zoneId);
            return retorno;
        }

        [HttpGet]
        [Route("api/Exchange/obterDisponibilidadeExchange/{smtp}")]
        public Status obterDisponibilidadeExchange(string smtp)
        {
            

            var retorno = new Fusion.Framework.Exchange.Service.SalaDeReuniaoBS().obterDisponibilidadeExchange(smtp);
            return retorno;
        }

        [HttpGet]
        [Route("api/Exchange/getBuildingRoomList")]
        public EmailAddressCollection getBuildingRoomList()
        {
            var rooms = new Fusion.Framework.Exchange.Service.SalaDeReuniaoBS().getRoomLists();
            return rooms;
        }

        [HttpGet]
        [Route("api/Exchange/getRooms/{smtpBuildingRoom}")]
        public Collection<EmailAddress> getRooms(string smtpBuildingRoom)
        {
            var rooms = new Fusion.Framework.Exchange.Service.SalaDeReuniaoBS().getRooms(smtpBuildingRoom);
            return rooms;
        }
    }
}
