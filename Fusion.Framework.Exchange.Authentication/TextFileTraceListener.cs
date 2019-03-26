using System;
using System.IO;
using Microsoft.Exchange.WebServices.Data;
using System.Net.Mail;
using System.Net;

namespace Fusion.Framework.Exchange.Authentication
{
  public class TraceListener : ITraceListener
  {
    public void Trace(string traceType, string traceMessage)
    {
      //CreateXMLTextFile(traceType, traceMessage);
      //Sendmail("andre@avmsistemas.net", traceMessage);
    }

    private void CreateXMLTextFile(string fileName, string traceContent)
    {
      try
      {
        if (!Directory.Exists(@"..\\TraceOutput"))
        {
          Directory.CreateDirectory(@"..\\TraceOutput");
        }

        System.IO.File.WriteAllText(@"..\\TraceOutput\\" + fileName + DateTime.Now.Ticks + ".txt", traceContent);
      }
      catch (IOException ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private void Sendmail(string to, string traceContent)
    {
        ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

        MailMessage mensagem = new MailMessage();
        mensagem.To.Add(new MailAddress(to));
        mensagem.From = new MailAddress("andre.mesquita.Fusion@gmail.com");
        mensagem.Subject = "TraceOutput";
        mensagem.Body = traceContent;
        mensagem.IsBodyHtml = false;        

        SmtpClient postman = new SmtpClient();
        postman.Credentials = new System.Net.NetworkCredential("andre.mesquita.Fusion@gmail.com", "Fusion.mesquita.andre");
        postman.Port = 465;
        postman.Host = "smtp.gmail.com";
        postman.EnableSsl = true;        

        postman.Send(mensagem);

        mensagem.Dispose();
        postman.Dispose();
    }

    private static bool CertificateValidationCallBack(
             object sender,
             System.Security.Cryptography.X509Certificates.X509Certificate certificate,
             System.Security.Cryptography.X509Certificates.X509Chain chain,
             System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        // If the certificate is a valid, signed certificate, return true.
        if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
        {
            return true;
        }

        // If there are errors in the certificate chain, look at each error to determine the cause.
        if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
        {
            if (chain != null && chain.ChainStatus != null)
            {
                foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                {
                    if ((certificate.Subject == certificate.Issuer) &&
                       (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                    {
                        // Self-signed certificates with an untrusted root are valid. 
                        continue;
                    }
                    else
                    {
                        if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                        {
                            // If there are any other errors in the certificate chain, the certificate is invalid,
                            // so the method returns false.
                            return false;
                        }
                    }
                }
            }

            // When processing reaches this line, the only errors in the certificate chain are 
            // untrusted root errors for self-signed certificates. These certificates are valid
            // for default Exchange server installations, so return true.
            return true;
        }
        else
        {
            // In all other cases, return false.
            return false;
        }
    }

  }
}
