using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Newtonsoft.Json;

namespace TMonitor
{
    //
    //the Boss class. Один Инстпектор на одну интерактивную инсталяцию. Реализовать singleton - данный класс должен быть один в инсталяции
    //Класс должен заниматься упаковкой писем, подписыванием ит.д. Он должен с некоторой переодичностью ходить и собирать различную инфу с класса NewsCollector
    //Этот классом является связующим звеном со всеми остальными, является ИНИЦИАТОРОМ всех процессов.
    //Письма пишет этот класс, т.е. он является инициатором создания КонвертовПисьма
    //
    public class CClientInspector
    {
        //public
        public float correspondenceFrequency;//Временной интервал отправки сообщений в секундах
        public string installationToken;//Токен инсталяции полученный на сервере-авторизации заранее например через Веб-интерфейс или просто сгенерированный и переданный уполномеченным человеком
        public string installationDescription;//Текстовое описание инсталяции для того чтобы человеку понимать с какой инсталяции приходят сообщения
        public string address;
        public string method;
        public string contentType;
        public IPostalService postalService;
        //private
        string sessionID; // ИД сессии будет отправляться каждый раз со всеми сообщениями - таким образом сравнивая на стороне сессии с последним ГУИ мы можем понимать когда происходили перезагрузки
        DateTime inspectorCreatingTime;
        DateTime correspondenceStartTime;
        DateTime correspondenceEndTime;
        Timer innerTimer;
        List<CMessage> messageBasket;
        //Constructor
        public CClientInspector(string pToken, string pDescription, string pAddress = "127.0.0.1", string pMethod = "POST", string pContentType = "application/json")
        {
            //currentTick = 0;
            correspondenceFrequency = 5000.0f;
            inspectorCreatingTime = DateTime.Now;
            installationToken = pToken;
            installationDescription = pDescription;
            sessionID = Guid.NewGuid().ToString();
            messageBasket = new List<CMessage>();
            postalService = new CClientPostman("http://" + pAddress, pMethod, pContentType);
        }
        public void SendCorrespondence(object source, ElapsedEventArgs e)
        {
            //Console.WriteLine("Correspondence have sent at " + DateTime.Now);
            CheckCorrespondenceSources();
            if (messageBasket.Count != 0)
            {
                postalService.Send(JsonConvert.SerializeObject(messageBasket));
                messageBasket.Clear();
            }
            else
                postalService.Send(new CMessage(EMessageType.UPTIME, installationToken, installationDescription, "uptime").ToJSON());
            //return 0;
        }
        public void Watch()
        {
            
        }
        public void Unwatch()
        {
            
        }
        public string ShowInfo()
        {
            string _result = "CClientInspector#" + installationToken + "\nWhere: " + installationDescription
                + "\nCreateTime: " + inspectorCreatingTime.ToLocalTime();
            return _result;
        }
        /*
        Методы кые надо реализовать
        public void SubscribeToNewsCollector();
        public void Unsubscribe()
        public void WatchUpTime();
        public void UnwatchUpTime();
        */
        public void MessageReadyAction(string pMessage)
        {
            //Console.WriteLine("[New message from NewsCollector!!!]: " + pMessage);
            messageBasket.Add(new CMessage(EMessageType.NEWS, installationToken, installationDescription, pMessage));
        }
        public void StartCorrespondence()
        {
            correspondenceStartTime = DateTime.Now;
            correspondenceEndTime = DateTime.Now;

            innerTimer = new System.Timers.Timer();
            innerTimer.Elapsed += new ElapsedEventHandler(SendCorrespondence);
            innerTimer.Interval = correspondenceFrequency;
            innerTimer.Enabled = true;
            //GC.KeepAlive(innerTimer);
        }
        public void EndCorrespondence()
        {
            correspondenceEndTime = DateTime.Now;
            innerTimer.Enabled = false;
            innerTimer.Close();
            innerTimer.Dispose();
            innerTimer = null;
        }
        public void CheckCorrespondenceSources()
        {
            CheckCorrespondenceFromNewsCollector();
        }
        public void CheckCorrespondenceFromNewsCollector()
        {
            if(CNewsCollector.newsBasket.Count != 0)
                messageBasket.Add(new CMessage(EMessageType.NEWS, installationToken, installationDescription, CNewsCollector.GetNews()));
            CNewsCollector.newsBasket.Clear();
        }
    }
}
