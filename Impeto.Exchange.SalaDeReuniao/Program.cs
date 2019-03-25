using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Impeto.Exchange.Configuration.Contexto;
using Impeto.Framework.Exchange.Entity;
using Impeto.Framework.Exchange.Models;
using System.Data.Entity;
using Impeto.Framework.Exchange.Service;
using Impeto.Exchange.SalaDeReuniao.svcExchange;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace Impeto.Exchange.SalaDeReuniao
{    
    class Program
    {
        static void Main()
        {
            try
            {
                new Functions().processarSalasDeReuniao();
            }
            catch (Exception ex)
            {
                string assunto = "Falha no Processo Robô";

                string mensagem = "MESSAGE     = " + ex.Message != null ? ex.Message.ToString() : "";
                string inner    = "INNER       = " + ex.InnerException != null ? ex.InnerException.ToString() : "";
                string source   = "SOURCE      = " + ex.Source != null ? ex.Source.ToString() : "";
                string stack    = "STACK TRACE = " + ex.StackTrace != null ? ex.StackTrace.ToString() : "";
                string target   = "TARGET      = " + ex.TargetSite != null ? ex.TargetSite.ToString() : "";

                string body = "";
                body += "<h5>Detalhe do Erro<h5>" + Environment.NewLine;
                body += "<ul>" + Environment.NewLine;
                body += "  <li>" + mensagem + "</li>" + Environment.NewLine;
                body += "  <li>" + inner + "</li>" + Environment.NewLine;
                body += "  <li>" + source + "</li>" + Environment.NewLine;
                body += "  <li>" + stack + "</li>" + Environment.NewLine;
                body += "  <li>" + target + "</li>" + Environment.NewLine;
                body += "</ul>" + Environment.NewLine;

                new Functions().notificacaoGeral(assunto, body);
            }
        }
            
    } 
}
