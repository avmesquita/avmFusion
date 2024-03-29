﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Fusion.Exchange.Configuration.Contexto;
using Fusion.Framework.Exchange.Entity;
using System.Data.Entity;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mail;
using System.Net;
using System.Data.Entity.Validation;
using Fusion.Framework.Exchange.Service;
using Fusion.Framework.Exchange.Models;

namespace Fusion.Exchange.SalaDeReuniao
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }

        [NoAutomaticTrigger]
        public void processarSalasDeReuniao()
        {
            #region Criar Arquivo de Log
            var path = Directory.GetCurrentDirectory() + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string arquivo = @path + "\\SalaDeReuniao_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";
            TextWriter log = File.CreateText(arquivo);
            #endregion

            SalaDeReuniaoContexto SalaDeReuniaoContexto = new SalaDeReuniaoContexto();

            var salas = SalaDeReuniaoContexto.DispositivoModels
                                                   .Include(x => x.Cliente)
                                                   .Where(x => x.Ativo == true && x.CodigoCliente != null && x.Smtp != "")
                                                   .ToList();

            if (salas.Count > 0)
            {

                foreach (var sala in salas)
                {
                    string storeId = "";
                    string smtp = "";

                    var client = new svcExchange.FusionExchangeServiceClient();


                    var disponibilidadeSalaDispositivo = client.obterDisponibilidadeExchange(sala.Cliente.Smtp,
                                                                                             sala.Cliente.Senha,
                                                                                             sala.Smtp,
                                                                                             sala.TimeZone);

                    try
                    {
                        gravarDisponibilidade(disponibilidadeSalaDispositivo);
                    }
                    catch { }

                    bool enviouEmailAviso = false;

                    switch (disponibilidadeSalaDispositivo.StatusDisponibilidade)
                    {
                        case StatusDisponibilidade.EmReuniao:
                            //
                            log.WriteLine("Opa! Encontrei um evento que indica reunião com 15 minutos de vida.");

                            bool arduinoStatusAviso = true;
                            var salasDeReuniaoAviso = GetSalas().Result;
                            var procurarSalaAviso = salasDeReuniaoAviso.Where(x => x.Smtp.ToLower().Equals(sala.Serial.ToLower()))
                                                                       .FirstOrDefault();

                            if (procurarSalaAviso != null)
                            {
                                if (procurarSalaAviso.DataAtualizacao < DateTime.Now.AddMinutes(-3))
                                {
                                    string msg = "O dispositivo " + sala.Serial + " está sem atualizar o status há mais de 3 minutos.";
                                    notificarNaoFuncionamento(sala.Serial, msg);
                                    break;
                                }
                                else
                                {
                                    arduinoStatusAviso = procurarSalaAviso.HasPeople == null ? false : Convert.ToBoolean(procurarSalaAviso.HasPeople);
                                }
                            }



                            if (!arduinoStatusAviso)
                            {
                                log.WriteLine("Opa! Verifiquei ainda que não existem pessoas na sala.");
                                foreach (var evento in disponibilidadeSalaDispositivo.ListaEventos)
                                {
                                    log.WriteLine("O assunto dela é " + evento.Details.Subject);

                                    storeId = evento.Details.StoreId;
                                    smtp = sala.Smtp;

                                    // Obtém a informação se o e-mail de AVISO já foi enviado anteriormente
                                    var emailEnviado = SalaDeReuniaoContexto.DisponibilidadeEmailModels.Where(x => x.Smtp.Equals(smtp) && x.StoreId.Equals(storeId) && x.StatusEnvio.Equals("A")).FirstOrDefault();
                                    enviouEmailAviso = (emailEnviado != null);

                                    if (!enviouEmailAviso)
                                    {
                                        enviouEmailAviso = true;

                                        var bs = new SalaDeReuniaoBS();
                                        bs.email = sala.Cliente.Smtp;
                                        bs.senha = sala.Cliente.Senha;
                                        bs.AlertaSalaVazia(storeId, smtp);

                                        try
                                        {
                                            // Registra envio na tabela
                                            var email = new ExchangeAvaiabilityEmail(smtp, storeId, "A");
                                            SalaDeReuniaoContexto.DisponibilidadeEmailModels.Add(email);
                                            SalaDeReuniaoContexto.SaveChanges();
                                        }
                                        catch { }

                                        log.WriteLine("Informei aos seus membros que a sala encontra-se vazia.");
                                    }
                                    else
                                    {
                                        log.WriteLine("Já enviei o aviso de 15 minutos de reunião anteriormente. O status permanece o mesmo.");
                                    }
                                }
                            }
                            //
                            break;

                        case StatusDisponibilidade.EmReuniao30:
                            //
                            log.WriteLine("Opa! Encontrei um evento que indica reunião com 30 minutos de vida.");
                            log.WriteLine("Verificando pelo dispositivo se existe alguém na sala... um momento...");

                            bool arduinoStatus = true;
                            var salasDeReuniao = GetSalas().Result;
                            var procurarSala = salasDeReuniao.Where(x => x.Smtp.ToLower().Equals(sala.Serial.ToLower())).FirstOrDefault();

                            if (procurarSala != null)
                            {
                                if (procurarSala.DataAtualizacao < DateTime.Now.AddMinutes(-3))
                                {
                                    string msg = "O dispositivo " + sala.Serial + " está sem atualizar o status há mais de 3 minutos.";
                                    notificarNaoFuncionamento(sala.Serial, msg);
                                    break;
                                }
                                else
                                {
                                    arduinoStatus = procurarSala.HasPeople == null ? false : Convert.ToBoolean(procurarSala.HasPeople);
                                }
                            }

                            if (!arduinoStatus)
                            {
                                log.WriteLine("Opa! Verifiquei ainda que não existem pessoas na sala.");
                                foreach (var evento in disponibilidadeSalaDispositivo.ListaEventos)
                                {
                                    log.WriteLine("O assunto dela é " + evento.Details.Subject);

                                    storeId = evento.Details.StoreId;
                                    smtp = sala.Smtp;

                                    var bs = new SalaDeReuniaoBS();
                                    bs.email = sala.Cliente.Smtp;
                                    bs.senha = sala.Cliente.Senha;

                                    try
                                    {
                                        var email = new ExchangeAvaiabilityEmail(smtp.Trim(), storeId.Trim(), "C");
                                        SalaDeReuniaoContexto.DisponibilidadeEmailModels.Add(email);
                                        SalaDeReuniaoContexto.SaveChanges();
                                    }
                                    catch { }

                                    var cancelou = bs.CancelarEvento(storeId, smtp);
                                    if (cancelou)
                                    {
                                        log.WriteLine("Reunião cancelada e seus membros notificados.");
                                    }
                                }
                            }
                            break;

                        default:
                            log.WriteLine(string.Format("Nada a processar para a sala de reunião {0}.", sala.Smtp.ToString()));
                            break;

                    }
                    // END SWITCH
                }
                // END FOREACH
            }
            // END IF COUNT
            else
            {
                log.WriteLine("Nada a processar.");
            }
            log.Flush();
            log.Close();
            log.Dispose();

        }

        [NoAutomaticTrigger]
        public static async Task<IEnumerable<Fusion.Framework.Exchange.Models.SalaDeReuniao>> GetSalas()
        {
            string path = "http://Fusionexchangearduino.azurewebsites.net/Fusion.Exchange.Arduino/api/Arduino/Get";

            HttpClient cliente = new HttpClient();

            var x = cliente.GetAsync(path);
            var y = x.Result;

            var formatters = new List<MediaTypeFormatter>() {
                new JsonMediaTypeFormatter(),
                new XmlMediaTypeFormatter()
            };

            var lista = y.Content.ReadAsAsync<IEnumerable<Fusion.Framework.Exchange.Models.SalaDeReuniao>>(formatters);

            return await lista;
        }

        [NoAutomaticTrigger]
        private static void gravarDisponibilidade(Framework.Exchange.Models.Status disponibilidade)
        {
            SalaDeReuniaoContexto SalaDeReuniaoContexto = new SalaDeReuniaoContexto();

            // GRAVA O HEADER
            var exchangeAvaiability = new ExchangeAvaiability();
            exchangeAvaiability.Mensagem = disponibilidade.Mensagem;
            exchangeAvaiability.Nome = disponibilidade.Mensagem;
            exchangeAvaiability.TipoMeetingAtendee = 0;
            SalaDeReuniaoContexto.DisponibilidadeModels.Add(exchangeAvaiability);
            SalaDeReuniaoContexto.SaveChanges();


            // GRAVA O STATUS
            var exchangeStatus = new ExchangeAvaiabilityStatus();
            exchangeStatus.Message = disponibilidade.Mensagem;
            exchangeStatus.NextMeetingRoomSugestion = disponibilidade.SugestaoProximaReuniao;

            // FK
            exchangeStatus.CodigoExchangeAvaiability = exchangeAvaiability.CodigoExchangeAvaiability;

            SalaDeReuniaoContexto.DisponibilidadeStatusModels.Add(exchangeStatus);
            SalaDeReuniaoContexto.SaveChanges();

            // GRAVA O DETALHE
            foreach (var item in disponibilidade.ListaEventos)
            {
                var detalhe = new ExchangeAvaiabilityStatusDetail();

                detalhe.DataInicio = item.StartTime;
                detalhe.DataFim = item.EndTime;
                detalhe.IsException = item.Details.IsException;
                detalhe.IsMeeting = item.Details.IsMeeting;
                detalhe.IsPrivative = item.Details.IsPrivate;
                detalhe.IsReminderSet = item.Details.IsReminderSet;
                detalhe.Location = item.Details.Location;
                detalhe.StoreId = item.Details.StoreId;
                detalhe.Subject = item.Details.Subject;

                // FK
                detalhe.CodigoExchangeAvaiabilityStatus = exchangeStatus.CodigoExchangeAvaibilityStatus;
                detalhe.ExchangeAvaiabilityStatus = exchangeStatus;

                SalaDeReuniaoContexto.DisponibilidadeStatusDetalheModels.Add(detalhe);
            }
            SalaDeReuniaoContexto.SaveChanges();

        }

        [NoAutomaticTrigger]
        private static void notificarNaoFuncionamento(string serial, string msg)
        {
            MailMessage mensagem = new MailMessage();
            mensagem.To.Add(new MailAddress("avmFusion@gmail.com", "Andre Mesquita"));
            mensagem.To.Add(new MailAddress("avmDevies@gmail.com", "Devices"));
            mensagem.From = new MailAddress("avmFusion@gmail.com");
            mensagem.Subject = "[SALA DE REUNIÃO] - [Falha] - " + serial;

            string corpo = "<H2>DISPOSITIVO " + serial + "</H2>" +
                           "<H5>Indicação de Falha no Funcionamento</H5>" +
                           "<P>" + msg + "</P>";

            mensagem.Body = corpo;
            mensagem.IsBodyHtml = true;

            mensagem.Priority = MailPriority.High;


            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("avmFusion@gmail.com", "Fusion.mesquita.andre");
            smtp.Send(mensagem);

        }

        [NoAutomaticTrigger]
        public void notificacaoGeral(string assunto, string msg)
        {
            MailMessage mensagem = new MailMessage();
            mensagem.To.Add(new MailAddress("avmFusion@gmail.com", "Andre Mesquita"));
            mensagem.To.Add(new MailAddress("avmDevices@gmail.com", "Devices"));
            mensagem.From = new MailAddress("avmFusion@gmail.com");
            mensagem.Subject = "[SALA DE REUNIÃO] - [" + assunto + "]";

            string corpo = "<H2>" + assunto + "</H2>" +
                           "<H5>Notificação Geral</H5>" +
                           "<P>" + msg + "</P>";

            mensagem.Body = corpo;
            mensagem.IsBodyHtml = true;

            mensagem.Priority = MailPriority.Normal;


            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("avmFusion@gmail.com", "Fusion");
            smtp.Send(mensagem);

        }
    }
}
