using System;
using System.Collections.Generic;
using System.Text;

namespace TMonitor
{
    public delegate void SuccessDeliveryHandler();
    public delegate void ErrorDeliveryHandler(CMessageEnvelope pMessageEnvelope);

    public interface IPostalService
    {
        //string GetInformation();
        //string FreeInformation();
        void Send(string pCorrespondence);
        void Send(string pCorrespondence, string pSignature);
        //Ивенты
        void OnSuccessDelivery();
        void OnErrorDelivery();
    }
}
