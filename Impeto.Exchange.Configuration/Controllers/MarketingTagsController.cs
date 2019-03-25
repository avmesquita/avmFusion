using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Impeto.Exchange.Configuration.Controllers
{
    public class MarketingTagsController : Controller
    {
        #region Google Tags
        public ActionResult GoogleInit()
        {
            return PartialView("GoogleInit", "");
        }
        /// <summary>
        /// Retorna a ActionResult com Código da Tag do Google Analytics, contendo userID ou não.
        /// </summary>
        /// <returns>Google Analytics Tag Code</returns>
        public ActionResult GoogleAnalytics()
        {
            return PartialView("GoogleAnalytics", "");
        }
        #endregion

        #region Facebook Tags
        public ActionResult FacebookSDK()
        {
            return PartialView("FacebookSDK", "");
        }
        #endregion

        #region Métodos Privados
        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        #endregion
    }
}