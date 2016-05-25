using System;
using System.Collections.Generic;
using System.Text;
using TMonitor;

namespace TestingProject
{
    class Program
    {
        static void Main(string[] args)
        {
            
            CClientInspector theInspector = new CClientInspector("xXx", "Database installation on Museum");
            CNewsCollector.BasketFlush += new MessageReadyHandler(theInspector.MessageReadyAction);
            theInspector.StartCorrespondence();
            theInspector.EndCorrespondence();
            /*
            while(true)
            {
                CClientPostman.Send("http://127.0.0.1/", theMessage);
                Console.ReadLine();
            }
            */
            Console.WriteLine("CNewsBit: " + new CNewsBit(ENewsBitType.general, "first event").ToJSON());

            Console.ReadLine();
        }
    }
}
