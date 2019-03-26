using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Impeto.Framework.Exchange.Service;
using Impeto.Framework.Exchange.Models;
using System.Net.Http;
using System.Net.Http.Formatting;
using Impeto.Exchange.Web.ArduinoAPI.Models;

namespace Impeto.Exchange.WebJob
{
    public class Sala
    {
        public string ID { get; set; }
        public bool EnviouEmail15 { get; set; }
        public bool EnviouEmail30 { get; set; }
    }

    class Program
    {
        // É usado para a percepção de envio de e-mail para não enviar duas ou mais vezes
        public static List<Sala> listaSala = new List<Sala>();

        //public static bool enviouEmailAviso = false;

        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }
            
            var host = new JobHost();


            // Chama o método ROBO
            host.Call(typeof(Program).GetMethod("Robo"));

            // Garante execução contínua do JOB
            host.RunAndBlock();          
        }

        public static void Robo(bool debug = false)
        {
            string storeId = "";
            string smtp = "";

            // Teste de funções gerais
            var disponibilidade = new SalaDeReuniaoBS().obterDisponibilidadeSalaReuniao();
            foreach (var sala in disponibilidade)
            {

                var k = listaSala.Where(x => x.ID == sala.Smtp).ToList();
                if (k == null)
                { 
                    Sala room = new Sala();
                    room.ID = sala.Smtp;                    
                    room.EnviouEmail15 = false;
                    room.EnviouEmail30 = false;
                    listaSala.Add(room);
                }


                switch (sala.Status.StatusDisponibilidade)
                {
                    case StatusDisponibilidade.EmReuniao:
                        //
                        if (debug)
                        {                           
                            Console.WriteLine("Opa! Encontrei um evento que indica reunião com 15 minutos de vida.");
                            Console.WriteLine(" ");
                        }
                        bool arduinoStatusAviso = true;
                        var salasDeReuniaoAviso = GetSalas().Result;
                        var procurarSalaAviso = salasDeReuniaoAviso.Where(x => x.Smtp.ToLower().Equals(sala.Smtp.ToLower())).FirstOrDefault();
                        if (procurarSalaAviso != null)
                        {
                            arduinoStatusAviso = procurarSalaAviso.HasPeople == null ? false : Convert.ToBoolean(procurarSalaAviso.HasPeople);
                        }
                        if (!arduinoStatusAviso)
                        {
                            if (debug)
                            {
                                Console.WriteLine(" ");
                                Console.WriteLine("Opa! Verifiquei ainda que não existem pessoas na sala.");
                            }
                            foreach (var evento in sala.Status.ListaEventos)
                            {
                                if (debug)
                                {
                                    Console.WriteLine(" ");
                                    Console.WriteLine("O assunto dela é " + evento.Details.Subject);
                                }
                                storeId = evento.Details.StoreId;
                                smtp = sala.Smtp;

                                var pegaSalaDaLista = listaSala.Where(x => x.ID == sala.Smtp).FirstOrDefault();
                                if (pegaSalaDaLista != null)
                                {                                    

                                    if (!pegaSalaDaLista.EnviouEmail15)
                                    {
                                        //enviouEmailAviso = true;
                                        new SalaDeReuniaoBS().AlertaSalaVazia(storeId, smtp);
                                        pegaSalaDaLista.EnviouEmail15 = true;
                                        if (debug)
                                        {
                                            Console.WriteLine(" ");
                                            Console.WriteLine("Informei aos seus membros que a sala encontra-se vazia.");
                                        }
                                    }
                                }
                            }
                        }
                        //
                        break;

                    case StatusDisponibilidade.EmReuniao30:
                        //
                        if (debug)
                        {
                            Console.WriteLine("Opa! Encontrei um evento que indica reunião com 30 minutos de vida.");
                            Console.WriteLine(" ");
                            Console.WriteLine("Verificando pelo dispositivo se existe alguém na sala... um momento...");
                        }
                        bool arduinoStatus = true;
                        var salasDeReuniao = GetSalas().Result;
                        var procurarSala = salasDeReuniao.Where(x => x.Smtp.ToLower().Equals(sala.Smtp.ToLower())).FirstOrDefault();
                        if (procurarSala != null)
                        {
                            arduinoStatus = procurarSala.HasPeople == null ? false : Convert.ToBoolean(procurarSala.HasPeople);
                        }
                        if (!arduinoStatus)
                        {
                            if (debug)
                            {
                                Console.WriteLine(" ");
                                Console.WriteLine("Opa! Verifiquei ainda que não existem pessoas na sala.");
                            }
                            foreach (var evento in sala.Status.ListaEventos)
                            {
                                if (debug)
                                {
                                    Console.WriteLine(" ");
                                    Console.WriteLine("O assunto dela é " + evento.Details.Subject);
                                }
                                storeId = evento.Details.StoreId;
                                smtp = sala.Smtp;
                                var cancelou = new SalaDeReuniaoBS().CancelarEvento(storeId, smtp);
                                if (cancelou)
                                {
                                    var pegaSalaDaLista = listaSala.Where(x => x.ID == sala.Smtp).FirstOrDefault();
                                    if (pegaSalaDaLista != null)
                                    {
                                        pegaSalaDaLista.EnviouEmail15 = false;
                                        pegaSalaDaLista.EnviouEmail30 = true;
                                    }

                                    if (debug)
                                    {
                                        Console.WriteLine(" ");
                                        Console.WriteLine("Reunião cancelada e seus membros notificados.");
                                    }
                                }
                            }
                        }                        
                        break;

                    default:
                        if (debug)
                        {
                            Console.WriteLine(string.Format("Nada a processar para a sala de reunião {0}.", sala.Smtp.ToString()));
                        }
                        break;
                }

            }
            disponibilidade = null;
        }

        public static void Disponibilidade()
        {
            var disponibilidade = new SalaDeReuniaoBS().obterDisponibilidadeSalaReuniao();

            if (disponibilidade != null && disponibilidade.Count > 0)
            {
                System.Console.Clear();
                System.Console.WriteLine("========================================");
                System.Console.WriteLine("           STATUS FROM ROOMS            ");
                System.Console.WriteLine(
                           string.Format("       UTC = {0}", DateTime.Now.ToUniversalTime()));
                System.Console.WriteLine("========================================");
                foreach (var item in disponibilidade)
                {
                    string situacao = "";

                    if (item.Status.StatusDisponibilidade == StatusDisponibilidade.EmReuniao)
                    {
                        // verifica status do arduino
                        // se não tiver ninguém na sala, envia e-mail avisando
                        bool arduinoStatus = false;
                        if (arduinoStatus)
                        {
                            situacao = "Reunião encontrada com pessoas.";
                        }
                        else
                        {
                            situacao = "Reunião iniciada há mais de 15 minutos e menos de 30, sem pessoas na sala. Enviamos um e-mail de alerta informando que a reunião será cancelada em 15 minutos.";
                            //new SendMailBS().Sendmail("andre.mesquita@impeto.com.br", situacao);
                        }
                    }

                    if (item.Status.StatusDisponibilidade == StatusDisponibilidade.EmReuniao30)
                    {
                        // verifica status do arduino
                        // se não tiver ninguém na sala, envia e-mail avisando que a reunião está sendo cancelada pelo Ímpeto Conference Auditor
                        bool arduinoStatus = false;
                        if (arduinoStatus)
                        {
                            situacao = "Reunião iniciada há mais de 30 minutos com pessoas.";
                        }
                        else
                        {
                            situacao = "Reunião iniciada há mais de 30 minutos SEM pessoas na sala. Enviamos um e-mail de alerta informando que a reunião foi cancelada.";
                            //new SendMailBS().Sendmail("andre.mesquita@impeto.com.br",situacao);
                        }
                    }

                    System.Console.WriteLine("NAME       = " + (item.Nome != null ? item.Nome.ToString() : ""));
                    System.Console.WriteLine("SMTP       = " + item.Smtp.ToString());
                    System.Console.WriteLine("TYPE       = " + item.Tipo.ToString());
                    System.Console.WriteLine("STATUS     = " + item.Status.StatusDisponibilidade.ToString());
                    System.Console.WriteLine("SUGGESTION = " + item.Status.SugestaoProximaReuniao.ToString());
                    System.Console.WriteLine("MESSAGE    = " + item.Status.Mensagem.ToString());
                    System.Console.WriteLine("PLOFT FLAG = " + situacao);
                    System.Console.WriteLine("===================================");
                }
            }
            else System.Console.WriteLine("No rooms were found.");
        }
        
        public static async Task<IEnumerable<Framework.Exchange.Models.SalaDeReuniao>> GetSalas()
        {
            string path = "http://impetoexchangearduino.azurewebsites.net/Impeto.Exchange.Arduino/api/Arduino/Get";

            HttpClient cliente = new HttpClient();

            var x = cliente.GetAsync(path);
            var y = x.Result;

            var formatters = new List<MediaTypeFormatter>() {
                new JsonMediaTypeFormatter(),
                new XmlMediaTypeFormatter()
            };

            var lista = y.Content.ReadAsAsync<IEnumerable<Framework.Exchange.Models.SalaDeReuniao>>(formatters);

            return await lista;            
        }        
    }
}
