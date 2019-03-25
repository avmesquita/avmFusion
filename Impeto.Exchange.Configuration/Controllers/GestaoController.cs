using Impeto.Exchange.Configuration.Contexto;
using Impeto.Exchange.Configuration.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Impeto.Framework.Exchange.Service;

namespace Impeto.Exchange.Portal.Controllers
{
    public class GestaoController : Controller
    {
        private ConfiguracaoContexto db = new ConfiguracaoContexto();

        [Authorize]
        public ActionResult MinhaConta(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (id != usuario.Codigo))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return RedirectToAction("Detalhes", "Clientes", id);
        }

        [Authorize]
        public ActionResult StatusDispositivos(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (id != usuario.Codigo))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int codigoCliente = Convert.ToInt32(id);

            var dispositivos = db.DispositivoModels.Where(t => t.CodigoCliente == codigoCliente).ToList();                               

            //status
            StringBuilder retorno = new StringBuilder();

            retorno.AppendLine("<br /><br /><br />");
            retorno.AppendLine("<h2>Status dos Dispositivos</h2>");

            retorno.AppendLine("<table class='table'>");
            retorno.AppendLine("  <tr>");
            retorno.AppendLine("    <th>Dispositivo</th>");
            retorno.AppendLine("    <th>Número de Série</th>");
            retorno.AppendLine("    <th>SMTP Sala de Reunião</th>");
            retorno.AppendLine("    <th>Data da Ativação</th>");
            retorno.AppendLine("    <th>Fuso Horário</th>");
            retorno.AppendLine("    <th>Status de Cadastro</th>");
            retorno.AppendLine("    <th>Última Atualização</th>");
            retorno.AppendLine("    <th>Último Status do Sensor</th>");
            retorno.AppendLine("  </tr>");

            foreach (var item in dispositivos)
            {
                var status = new DispositivoController().GetSala(item.Serial);

                retorno.AppendLine("  <tr>");
                retorno.AppendLine("    <td>" + item.Nome.ToString() + "</td>");
                retorno.AppendLine("    <td>" + item.Serial.ToString() + "</td>");
                retorno.AppendLine("    <td>" + item.Smtp.ToString() + "</td>");
                retorno.AppendLine("    <td>" + item.DataAtivacao.ToString() + "</td>");
                retorno.AppendLine("    <td>" + item.TimeZone.ToString() + "</td>");
                retorno.AppendLine("    <td>" + (item.Ativo ? "Ativo" : "Inativo") + "</td>");
                if ((status != null) && (status.Result != null))
                {
                    retorno.AppendLine("    <td>" + status.Result.DataAtualizacao.ToString() + "</td>");
                    retorno.AppendLine("    <td>" + (status.Result.HasPeople ? "Sala Não Vazia" : "Sala Vazia") + "</td>");
                }
                else
                {
                    retorno.AppendLine("    <td>Sem atualização</td>");
                    retorno.AppendLine("    <td>Sem atualização</td>");
                }
                retorno.AppendLine("  </tr>");
            }
            retorno.AppendLine("</table>");

            ViewBag.Retorno = retorno.ToString();

            return View();
        }

        [Authorize]
        public ActionResult StatusSalas()
        {
            return View();
        }

        [Authorize]
        public ActionResult GetExchangeRooms(int? id)
        {
            var usuario = db.ClienteModels.FirstOrDefault(u => u.Smtp == User.Identity.Name);
            if ((id == null) || (id != usuario.Codigo))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int codigoCliente = Convert.ToInt32(id);

            var cliente = db.ClienteModels.Where(t => t.Codigo == codigoCliente).FirstOrDefault();
        
            string email = cliente.Smtp;
            string senha = cliente.Senha;           

            Configuration.svcExchange.ImpetoExchangeServiceClient client = new Configuration.svcExchange.ImpetoExchangeServiceClient();

            #region Obter Disponibilidade

            if (cliente != null && !string.IsNullOrEmpty(cliente.Smtp))
            {
                var disponibilidade = client.obterDisponibilidadeTimeZoneFull(email, senha, "E. South America Standard Time");

                if (disponibilidade.Count() > 0)
                {
                    string listagem = "<table class='table' width='90%'>";

                    listagem += "<tr>";
                    listagem += "<td>SMTP</td>";
                    listagem += "<td>STATUS</td>";
                    listagem += "</tr>";

                    foreach (var sala in disponibilidade)

                    {
                        listagem += "<tr>";
                        listagem += "<td>" + sala.Smtp + "</td>";
                        listagem += "<td>" + sala.Status.StatusDisponibilidade.ToString() + "</td>";
                        listagem += "</tr>";
                    }
                    listagem += "</table>";

                    ViewBag.O365 = listagem;
                }
                else
                {
                    string listagem = string.Empty;
                    var roomLists = new SalaDeReuniaoBS().getRoomLists();
                    if (roomLists != null && roomLists.Count() > 0)
                    {
                        foreach (var room in roomLists)
                        {
                            var rooms = new SalaDeReuniaoBS().getRooms(room.ToString());
                            listagem = "<table class='table' width='90%'>";
                            listagem += "<tr>";
                            listagem += "<td>SMTP</td>";
                            listagem += "</tr>";
                            foreach (var item in rooms)
                            {
                                listagem += "<tr>";
                                listagem += "<td>" + item.ToString() + "</td>";
                                listagem += "</tr>";
                            }
                            listagem += "</table>";
                        }
                    }
                    else
                    {
                        listagem = "<table class='table' width='90%'>";
                        listagem += "<tr>";
                        listagem += "<td>Não foram encontradas salas de reunião disponíveis.</td>";
                        listagem += "</tr>";
                        listagem += "</table>";                     
                    }
                    ViewBag.O365 = listagem;
                }
            }
            else
            {
                string listagem = "<table class='table' width='90%'>";
                listagem += "<tr>";
                listagem += "<td>Não existem dispositivos vinculados à(s) sala(s) de reunião.</td>";
                listagem += "</tr>";
                listagem += "</table>";
                ViewBag.O365 = listagem;
            }
            #endregion

            return View();
        }


    }
}