using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Formatting;
using Impeto.Exchange.Web.ArduinoAPI.Models;
using Impeto.Framework.Exchange.Service;

namespace Impeto.Exchange.WebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }

        [NoAutomaticTriggerAttribute]
        public static async Task ProcessSomething(TextWriter log)
        {
            Thread.Sleep(7000);
            log.WriteLine("Process Something called at : " + DateTime.Now.ToShortDateString() + " , " + DateTime.Now.ToShortTimeString());
        }
    }
}
