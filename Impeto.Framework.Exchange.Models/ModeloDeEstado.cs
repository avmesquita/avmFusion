using Impeto.Framework.Exchange.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;
using RestSharp;

namespace Impeto.Framework.Exchange.Models
{
    public sealed class ModeloDeEstado
    {
        #region Singleton Pattern
        private static volatile ModeloDeEstado instance;
        private static object syncRoot = new Object();

        private ModeloDeEstado()
        {
            // pré-instancia a lista de salas de reunião ##NoExceptionRules
            _listArduino = new List<SalaDeReuniao>();

            // Armazena os últimos 10 posts
            _listHistorico = new List<SalaDeReuniao>();
        }

        public static ModeloDeEstado Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ModeloDeEstado();
                    }
                }
                return instance;
            }
        }
        #endregion

        private const string BAD = "BAD";
        private const string CAD = "CAD";
        private const string OK = "OK";
        private const string VNC = "VNC";
        private const string SEP = "|";

        private List<SalaDeReuniao> _listArduino;

        private List<SalaDeReuniao> _listHistorico;

        private void incluirHistorico(string smtp, string haspeople)
        {
            bool ocupada = false;
            if (haspeople.Trim().Equals("1") ||
                haspeople.ToLower().Trim().Equals("true") ||
                haspeople.ToLower().Trim().Equals("sim") ||
                haspeople.ToLower().Trim().Equals("yes"))
            {
                ocupada = true;
            }

            SalaDeReuniao novoEstado = new SalaDeReuniao();
            novoEstado.Smtp = smtp.Trim();
            novoEstado.HasPeople = ocupada;            
            novoEstado.DataAtualizacao = DateTime.Now;

            if (_listHistorico.Count() >= 10)
            {
                var primeiro = _listHistorico.Select(x => x).OrderBy(x => x.DataAtualizacao).FirstOrDefault();
                if (primeiro != null)
                    _listHistorico.Remove(primeiro);
            }
            _listHistorico.Add(novoEstado);

        }

        public List<SalaDeReuniao> ListaDeModelosDeEstados
        {
            get { return _listArduino; }
            set { _listArduino = value; }
        }

        public List<SalaDeReuniao> ListaDeHistorico
        {
            get { return _listHistorico; }
            set { _listHistorico = value; }
        }

        public string Limpar()
        {
            foreach (var item in _listArduino)
            {
                _listArduino.Remove(item);
            }

            return _listArduino.Count() == 0 ? "OK" : "BAD";
        }

        public string LimparHistorico()
        {
            foreach (var item in _listHistorico)
            {
                _listHistorico.Remove(item);
            }
            return _listHistorico.Count() == 0 ? "OK" : "BAD";
        }

        public string SaveOrUpdate(string smtp, string haspeople)
        {
            string retorno = BAD;            

            // inclui o registro de histórico
            try
            {
                incluirHistorico(smtp, haspeople);
            }
            catch
            { }

            // inicia o processamento de armazenagem do estado
            bool Found = false;

            bool ocupada = false;
            if (haspeople.Trim().Equals("1") ||
                haspeople.ToLower().Trim().Equals("true") ||
                haspeople.ToLower().Trim().Equals("sim") ||
                haspeople.ToLower().Trim().Equals("yes"))
            {
                ocupada = true;
            }

            foreach (var item in ModeloDeEstado.Instance.ListaDeModelosDeEstados)
            {
                if (item.Smtp.Trim() == smtp.Trim())
                {
                    try
                    {
                        Found = true;
                        item.HasPeople = ocupada;
                        item.DataAtualizacao = DateTime.Now;
                        retorno = OK + SEP + item.Smtp + SEP + OK;
                    }
                    catch
                    {
                        // PRATICAMENTE IMPOSSIVEL OCORRER, MAS...
                        // DEU ERRO NA ATUALIZACAO DO SINGLETON EM MEMORIA
                        // SE DER ISSO AQUI, OU VERTICALIZA O SERVIÇO OU AUMENTA A INSTANCIA OU IMPLEMENTA LOCK NO SINGLETON
                        // ESTE METODO NAO PODE SER ASYNC
                        // DAR MAIS MEMORIA EH UMA OPCAO RAZOAVEL PARA ALTO CONSUMO DE BANDA SEM LOCK TIRANDO PERFORMANCE
                        retorno = BAD + SEP + item.Smtp + SEP + BAD;
                    }
                }
            }
            if (!Found)
            {
                SalaDeReuniao novoEstado = new SalaDeReuniao();
                novoEstado.Smtp = smtp.Trim();
                novoEstado.HasPeople = ocupada;
                novoEstado.DataAtualizacao = DateTime.Now;
                ModeloDeEstado.Instance.ListaDeModelosDeEstados.Add(novoEstado);

                bool vinculadoAumCliente = false;
                
                try
                {
                    // foi implementado de forma a bypassar o catch
                    // porque pode haver timeout na busca de informação                                        
                    Dispositivo dispositivo = GetDispositivo(smtp);


                    // IDENTIFICA A VINCULACAO DO DISPOSITIVO
                    if (dispositivo != null)
                    {                        
                        if (dispositivo.CodigoCliente != null && dispositivo.CodigoCliente > 0)
                        {
                            vinculadoAumCliente = true;
                            LogEvento(dispositivo);
                        }

                    } else
                    {
                        if (dispositivo.Token.Contains("Timeout"))
                        {
                            retorno = OK + SEP + smtp.Trim() + SEP + BAD;
                        }
                        vinculadoAumCliente = false;
                    }
                   
                }
                catch
                {
                    // DEU ERRO NA VERIFICACAO DA VINCULACAO
                    vinculadoAumCliente = false;
                    retorno = OK + SEP + smtp.Trim() + SEP + BAD;
                    return retorno;
                }

                if (vinculadoAumCliente)
                {
                    // SE ESTA VINCULADO
                    retorno = OK + SEP + smtp.Trim() + SEP + VNC;
                }
                else
                {
                    // SE NAO ESTA VINCULADO ENTAO FOI CADASTRO NOVO
                    retorno = OK + SEP + smtp.Trim() + SEP + CAD;
                }

                return retorno;
            }

            return retorno;
        }

        public void LogEvento(Dispositivo dispositivo)
        {
            string devpath = "http://localhost:52526";
            string prodpath = "http://impetoexchangeconfiguration.azurewebsites.net";

            var logEvento = new LogEvento();

            var urlAplicacao = "";
            if (Debugger.IsAttached)
                urlAplicacao = devpath;
            else
                urlAplicacao = prodpath;

            var acao = "?action=InclusaoLogEvento&codigo={0}&dataAtivacao={1}&token={2}&status={3}";
            var uri = "/api/LogEvento/InclusaoLogEvento" + acao;

            uri = string.Format(uri,
                    dispositivo.Codigo,
                    dispositivo.DataAtivacao,
                    dispositivo.Token,
                    dispositivo.Ativo);

            var client = new RestClient(urlAplicacao);
            var request = new RestRequest(uri, Method.POST);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            request.JsonSerializer.ContentType = "application/json; charset=utf-8";
            var response = client.Execute<LogEvento>(request);

            var teste = response.Data.MensagemErro;
        }

        private Dispositivo GetDispositivo(string serial)
        {
            Dispositivo dispositivo = new Dispositivo();
            try
            {
                string path = string.Format("http://impetoexchangeconfiguration.azurewebsites.net/api/Device/GetDevice?serial={0}", serial);

                HttpClient cliente = new HttpClient();

                var x = cliente.GetAsync(path);
                var y = x.Result;

                dispositivo = y.Content.ReadAsAsync<Dispositivo>().Result;
            }
            catch
            {
                dispositivo = new Dispositivo();
                dispositivo.Token = "Timeout GetDevice do Barramento de Serviços de Configuração";
            }
            return dispositivo;
        }
    }
}