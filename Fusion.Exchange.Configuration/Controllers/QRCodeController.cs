using Fusion.Exchange.Configuration.Helpers;
using MessagingToolkit.QRCode.Codec;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fusion.Exchange.Configuration.Controllers
{
    public class QRCodeController : Controller
    {
        // GET: QRCode
        public ActionResult Index()
        {            
            string htmlimagem = "<img src = 'http://Fusionexchangeconfiguration.azurewebsites.net/QRCode/Generate?texto='"+ "http://Fusionexchangeconfiguration.azurewebsites.net/" + "' alt='Ímpeto Sala de Reunião' style='' />";

            ViewBag.htmlimagem = htmlimagem;

            return View();
        }

        /// <summary>
        /// Para utilizar, use:
        /// <img src="http://localhost/QRCode/Generate?texto=<--SERIAL_DEVICE-->" alt="Registro de Dispositivo" style="position:absolute;background:#fff;margin-left:660px;zoom:70%;" />
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public ActionResult Generate(string texto)
        {
            QRCodeEncoder qrCodecEncoder = new QRCodeEncoder();
            qrCodecEncoder.QRCodeBackgroundColor = System.Drawing.Color.White;
            qrCodecEncoder.QRCodeForegroundColor = System.Drawing.Color.Black;
            qrCodecEncoder.CharacterSet = "UTF-8";
            qrCodecEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodecEncoder.QRCodeScale = 6;
            qrCodecEncoder.QRCodeVersion = 0;
            qrCodecEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;

            if (Debugger.IsAttached)
            {
                texto = "http://localhost:52526/Account/Register/?FirstDevice=" + texto;
            }
            else
            {
                texto = "http://Fusionexchangeconfiguration.azurewebsites.net/Account/Register/?FirstDevice=" + texto;
            }

            Bitmap imageQRCode;
            String data = texto;
            imageQRCode = qrCodecEncoder.Encode(data);
            
            System.IO.Stream stream = new System.IO.MemoryStream();
            imageQRCode.Save(stream, System.Drawing.Imaging.ImageFormat.Gif);
            stream.Flush();

            byte[] m_Bytes = StreamHelper.ReadToEnd(stream);

            Response.AddHeader("Content-Disposition", "inline; filename=FusionDeviceQRCode.gif");

            return File(m_Bytes, "image/gif");
        }
    }
}