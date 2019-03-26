using Fusion.Framework.Exchange.Models;
using Fusion.Framework.Exchange.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fusion.Exchange.Configuration.Controllers
{
    public class WebJobController : Controller
    {
        // GET: WebJob
        public ActionResult Index()
        {
            return View();
        }

        private void runWebJob()
        {
            /*
            string storeId = "";
            string smtp = "";

            // Teste de funções gerais
            var disponibilidade = new SalaDeReuniaoBS().obterDisponibilidadeSalaReuniao();
            foreach (var sala in disponibilidade)
            {
                switch (sala.Status.StatusDisponibilidade)
                {
                    case StatusDisponibilidade.EmReuniao:
                        //
                        //if (debug)
                        //{
                        //    Console.WriteLine("Opa! Encontrei um evento que indica reunião com 15 minutos de vida.");
                        //    Console.WriteLine(" ");
                        //}
                        bool arduinoStatusAviso = true;
                        var salasDeReuniaoAviso = GetSalas().Result;
                        var procurarSalaAviso = salasDeReuniaoAviso.Where(x => x.Smtp.ToLower().Equals(sala.Smtp.ToLower())).FirstOrDefault();
                        if (procurarSalaAviso != null)
                        {
                            arduinoStatusAviso = procurarSalaAviso.HasPeople == null ? false : Convert.ToBoolean(procurarSalaAviso.HasPeople);
                        }
                        if (!arduinoStatusAviso)
                        {
                            //if (debug)
                            //{
                            //    Console.WriteLine(" ");
                            //    Console.WriteLine("Opa! Verifiquei ainda que não existem pessoas na sala.");
                            //}
                            foreach (var evento in sala.Status.ListaEventos)
                            {
                                //if (debug)
                                //{
                                //    Console.WriteLine(" ");
                                //    Console.WriteLine("O assunto dela é " + evento.Details.Subject);
                                //}
                                storeId = evento.Details.StoreId;
                                smtp = sala.Smtp;
                                new SalaDeReuniaoBS().AlertaSalaVazia(storeId, smtp);
                                //if (debug)
                                //{
                                //    Console.WriteLine(" ");
                                //    Console.WriteLine("Informei aos seus membros que a sala encontra-se vazia.");
                                //
                            }
                            }
                        }
                        //
                        break;

                    case Models.StatusDisponibilidade.EmReuniao30:
                        //
                        //if (debug)
                        //{
                        //    Console.WriteLine("Opa! Encontrei um evento que indica reunião com 30 minutos de vida.");
                        //    Console.WriteLine(" ");
                        //    Console.WriteLine("Verificando pelo dispositivo se existe alguém na sala... um momento...");
                        //}
                        bool arduinoStatus = true;
                        var salasDeReuniao = GetSalas().Result;
                        var procurarSala = salasDeReuniao.Where(x => x.Smtp.ToLower().Equals(sala.Smtp.ToLower())).FirstOrDefault();
                        if (procurarSala != null)
                        {
                            arduinoStatus = procurarSala.HasPeople == null ? false : Convert.ToBoolean(procurarSala.HasPeople);
                        }
                        if (!arduinoStatus)
                        {
                            //if (debug)
                            //{
                            //    Console.WriteLine(" ");
                            //    Console.WriteLine("Opa! Verifiquei ainda que não existem pessoas na sala.");
                            //}
                            foreach (var evento in sala.Status.ListaEventos)
                            {
                                //if (debug)
                                //{
                                //    Console.WriteLine(" ");
                                //    Console.WriteLine("O assunto dela é " + evento.Details.Subject);
                                //}
                                storeId = evento.Details.StoreId;
                                smtp = sala.Smtp;
                                var cancelou = new SalaDeReuniaoBS().CancelarEvento(storeId, smtp);
                                if (cancelou)
                                {
                                    //if (debug)
                                    //{
                                    //    Console.WriteLine(" ");
                                    //    Console.WriteLine("Reunião cancelada e seus membros notificados.");
                                    //}
                                }
                            }
                        }
                        //
                        break;

                    //default:
                        //if (debug)
                        //{
                        //    Console.WriteLine(string.Format("Nada a processar para a sala de reunião {0}.", sala.Smtp.ToString()));
                        //}
                      //  break;
                }
            }
            */
        }
    }
}