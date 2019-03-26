using Fusion.Framework.Exchange.Models;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Fusion.Exchange.wcfsvcapp
{
    [ServiceContract]
    public interface IFusionExchangeService
	{
        [OperationContract]
        List<ResultadoDisponibilidadeSalaReuniao> obterDisponibilidadeSalaReuniao();

        [OperationContract]
        List<ResultadoDisponibilidadeSalaReuniao> obterDisponibilidadeSalaReuniaoTimeZone(string timeZone = "");

        [OperationContract]
        List<ResultadoDisponibilidadeSalaReuniao> obterDisponibilidadeTimeZoneFull(string email, string senha, string timeZone = "");


        //Status obterDisponibilidadeExchange(string smtp);
        [OperationContract]
        Status obterDisponibilidadeExchange(string email, string senha, string smtp, string timeZone = "");

        [OperationContract]
        EmailAddressCollection getBuildingRoomList(string email, string senha);
        //EmailAddressCollection getBuildingRoomList();

        [OperationContract]
        Collection<EmailAddress> getRooms(string email, string senha, string smtpBuildingRoom);
        //Collection<EmailAddress> getRooms(string smtpBuildingRoom);
    }

}
