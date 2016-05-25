using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using System.IO;


namespace TMonitor
{
    //
    //the Boss class. Один Инстпектор на одну интерактивную инсталяцию. Реализовать singleton - данный класс должен быть один в инсталяции
    //Класс должен заниматься упаковкой писем, подписыванием ит.д. Он должен с некоторой переодичностью ходить и собирать различную инфу с класса NewsCollector
    //Этот классом является связующим звеном со всеми остальными, является ИНИЦИАТОРОМ всех процессов.
    //Письма пишет этот класс, т.е. он является инициатором создания КонвертовПисьма
    //
    [JsonObject(MemberSerialization.OptIn)]
    public class CClientInspector
    {
        public static string console = "";
        [JsonProperty]
        public string installationToken;//Токен инсталяции полученный на сервере-авторизации заранее например через Веб-интерфейс или просто сгенерированный и переданный уполномеченным человеком
        [JsonProperty]
        public string installationDescription;//Текстовое описание инсталяции для того чтобы человеку понимать с какой инсталяции приходят сообщения
        public CPostalService postalService;
        public CArchiveService archiveService;

        [JsonProperty]
        Guid sessionID; // ИД сессии будет отправляться каждый раз со всеми сообщениями - таким образом сравнивая на стороне сессии с последним ГУИ мы можем понимать когда происходили перезагрузки
        DateTime inspectorCreatingTime;

        /*
        public CClientInspector()
        {
            inspectorCreatingTime = DateTime.Now;
            sessionID = Guid.NewGuid();
        }
        */
        public CClientInspector(string pToken, string pDescription = "")
        {
            installationToken = pToken;
            installationDescription = pDescription;
            inspectorCreatingTime = DateTime.Now;
            sessionID = Guid.NewGuid();
        }
        public bool Initialize(Dictionary<string, string> pConfiguration)
        {
            try
            {
                installationToken = pConfiguration["installationToken"];
                installationDescription = pConfiguration["installationDescription"];
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckInstallationToken(bool pExceptionMode = false)
        {
            if(pExceptionMode)
            {
                throw new Exception("Installation token is wrong. Revalidate and get new token.");
            }
            else
            {
                return true;
            }
        }
        public void MessageReturn(CMessageEnvelope pMessageEnvelope)
        {
            archiveService.ArchiveOfNewsFeed(pMessageEnvelope.message);
        }
        public void SendPingSignalToServer()
        {
            Console.WriteLine("[tilli]: SendPingSignalToServer start");
            postalService.SendPing("ping", JsonConvert.SerializeObject(this));
            Console.WriteLine("[tilli]: SendPingSignalToServer start");
        }
        public void SendLogsToServer()
        {
            //Воспользовавшись функционалом Архиватора мы получим словарь с ключами ввиде имен файлов и значениями ввиде содержимого файлов
            Dictionary<string, string> theFileCollection = archiveService.ExtractAllArchives();
            foreach (KeyValuePair<string, string> file in theFileCollection)
            {
                bool requestResult = postalService.SendLogs(file.Value, JsonConvert.SerializeObject(this));
                if(requestResult)
                {
                    File.Delete(file.Key);
                }
            }
        }
        public void NewsFeedToLogs()
        {
            archiveService.ArchiveOfNewsFeed(CNewsCollector.newsFeed.ToArray());
            CNewsCollector.ClearNewsFeed();
        }
        public void WindowDressing()
        {
            CNewsCollector.GeneralNews("WindowDressing");
            //CNewsCollector.ButtonPressed("unknown button");
        }
    }
}
