using ArduinoAPI;
using Impeto.Framework.Exchange.Service;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using System.Timers;
using System.Collections;

namespace Impeto.Framework.Exchange
{
    /// <summary>
    /// O Console é utilizado somente para testes
    /// O código indicado abaixo não representa o robô atual
    /// </summary>
    public class Program
    {
        public static bool enviouEmailAviso = false;
        public static ArrayList dispositivosPareados = new ArrayList();

        static void Main(string[] args)
        {
            System.Console.WriteLine("Aguarde enquanto iniciamos a conexão...");
            System.Console.WriteLine("");
            System.Console.WriteLine("Processando...");
            System.Console.WriteLine("");

            while (true)
            {
                Robo();                                
            }
            //Disponibilidade();

            //System.Console.WriteLine("Pressione Enter para sair");
            //System.Console.Read();
        }

        public static void Robo(bool debug = false)
        {
            string storeId = "";
            string smtp = "";

            GetDispositivosPareados(); 

            // Teste de funções gerais
            var disponibilidade = new SalaDeReuniaoBS().obterDisponibilidadeSalaReuniao();
            foreach (var sala in disponibilidade)
            {
                switch (sala.Status.StatusDisponibilidade)
                {
                    case Models.StatusDisponibilidade.EmReuniao:
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
                            arduinoStatusAviso = procurarSalaAviso.HasPeople == null ? false : Convert.ToBoolean(GetAmostragemDispositivo(smtp));
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
                                if (!enviouEmailAviso)
                                {
                                    enviouEmailAviso = true;
                                    new SalaDeReuniaoBS().AlertaSalaVazia(storeId, smtp);
                                    if (debug)
                                    {
                                        Console.WriteLine(" ");
                                        Console.WriteLine("Informei aos seus membros que a sala encontra-se vazia.");
                                    }
                                }
                            }
                        }
                        //
                        break;

                    case Models.StatusDisponibilidade.EmReuniao30:
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
                                    if (debug)
                                    {
                                        Console.WriteLine(" ");
                                        Console.WriteLine("Reunião cancelada e seus membros notificados.");
                                    }
                                }
                            }
                        }
                        //
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

                    if (item.Status.StatusDisponibilidade == Models.StatusDisponibilidade.EmReuniao)
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

                    if (item.Status.StatusDisponibilidade == Models.StatusDisponibilidade.EmReuniao30)
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

        public static async Task<IEnumerable<ArduinoAPI.Models.SalaDeReuniao>> GetSalas()
        {
            string path = "http://impetoexchangearduino.azurewebsites.net/Impeto.Exchange.Arduino/api/Arduino/Get";

            HttpClient cliente = new HttpClient();

            var x = cliente.GetAsync(path);
            var y = x.Result;

            var formatters = new List<MediaTypeFormatter>() {
                new JsonMediaTypeFormatter(),
                new XmlMediaTypeFormatter()
            };

            var lista = y.Content.ReadAsAsync<IEnumerable<ArduinoAPI.Models.SalaDeReuniao>>(formatters);

            return await lista;
        }

        public static Task<bool> GetAmostragemDispositivo(string smtp)
        {
            string path = string.Format("http://impetoexchangearduino.azurewebsites.net/Impeto.Exchange.Arduino/api/LogEvento/GetAmostragemDispositivoAtivo?smtp={0}",smtp);

            HttpClient cliente = new HttpClient();

            var x = cliente.GetAsync(path);
            var y = x.Result;

            var formatters = new List<MediaTypeFormatter>() {
                new JsonMediaTypeFormatter(),
                new XmlMediaTypeFormatter()
            };

            var valor = y.Content.ReadAsAsync<bool>(formatters);

            return valor;
        }

        private static void GetDispositivosPareados()
        {
            //string path = "http://localhost:52526/api/Device/GetDispositivosPareados";
            string path = "http://impetoexchangearduino.azurewebsites.net/api/Device/GetDispositivosPareados";

            HttpClient cliente = new HttpClient();

            var x = cliente.GetAsync(path);
            var y = x.Result;

            var formatters = new List<MediaTypeFormatter>() {
                    new JsonMediaTypeFormatter(),
                    new XmlMediaTypeFormatter()
            };

            var listaDispositivo = y.Content.ReadAsAsync<ICollection<Entity.Dispositivo>>(formatters);

            foreach (var dispositivo in listaDispositivo.Result)
            {
                var retorno = dispositivo.Serial + " | " + dispositivo.DataAtivacao.ToString() + " | " + dispositivo.Ativo.ToString();

                if (Program.dispositivosPareados.IndexOf(retorno) == -1)
                {
                    Program.dispositivosPareados.Add(retorno);
                }
            }
        }

    }

}
