using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Impeto.Exchange.Configuration.Service
{
    public class RegistroDeLogsService
    {
        String mapPath;

        /// <summary>
        /// Passar o seguinte no construtor:
        /// Server.MapPath("~/logs")
        /// </summary>
        /// <param name="mPath"></param>
        public RegistroDeLogsService(String mPath)
        {
            mapPath = mPath;
        }

        public void RegistrarLog(Exception excecao)
        {
            if (excecao != null)
            {
                var nomeDoArquivo = String.Concat(DateTime.Now.ToString("yyyy-MM-dd"), ".log");
                var caminhoDoArquivo = Path.Combine(mapPath, nomeDoArquivo);

                if (mapPath.Length > 0 && !Directory.Exists(mapPath))
                {
                    Directory.CreateDirectory(mapPath);
                }

                using (StreamWriter file = new StreamWriter(caminhoDoArquivo, true))
                {
                    file.WriteLine(String.Concat("Data/Hora: ", DateTime.Now));
                    file.WriteLine(String.Concat("Mensagem de erro: ", excecao.Message != null ? excecao.Message : "mensagem nula"));
                    if (excecao.InnerException != null)
                        file.WriteLine(String.Concat("Mensagem de erro interna: ", excecao.InnerException != null ? excecao.InnerException.ToString() : "mensagem nula"));
                    file.WriteLine(String.Concat("Rastreamento da pilha: ", excecao.StackTrace != null ? excecao.StackTrace : "mensagem nula"));
                    file.WriteLine();
                    file.Close();
                }
                //redirecionar para página de erro padrão com mensagem tratada para o usuário
            }
        }

        public void RegistrarMensagem(string mensagem)
        {
            if (!string.IsNullOrEmpty(mensagem))
            {
                var nomeDoArquivo = String.Concat(DateTime.Now.ToString("yyyy-MM-dd"), ".log");
                var caminhoDoArquivo = Path.Combine(mapPath, nomeDoArquivo);

                if (mapPath.Length > 0 && !Directory.Exists(mapPath))
                {
                    Directory.CreateDirectory(mapPath);
                }

                using (StreamWriter file = new StreamWriter(caminhoDoArquivo, true))
                {
                    file.WriteLine(String.Concat("Data/Hora: ", DateTime.Now));
                    file.WriteLine(String.Concat("Mensagem: ", mensagem));
                    file.WriteLine();
                    file.Close();
                }
            }
        }
    }
}