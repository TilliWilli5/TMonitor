using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TMonitor
{
    public enum EMessageType
    {
        unstandardized,
        uptime
    }

    //
    //Класс отправки сообщений по. Ур-нь абстракции
    //
    [JsonObject(MemberSerialization.OptIn)]
    public class CMessageEnvelope
    {
        static uint currentTick = 0;
        [JsonProperty]
        uint tick;
        [JsonProperty]
        DateTime sendingTime;
        [JsonProperty]
        CMessage theMessage;
        public CMessageEnvelope(CMessage pMessage)
        {
            theMessage = pMessage;
        }
        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
        public void AutoSigning()
        {
            tick = currentTick++;
            sendingTime = DateTime.Now;
        }

    }
    //
    //Базовый класс сообщений
    //
    [JsonObject(MemberSerialization.OptIn)]
    public class CMessage
    {
        [JsonProperty]
        string installationSignature;//aka токен инсталяции - идентифицирующий инсталяцию
        [JsonProperty]
        EMessageType messageType;//Тип сообщения для того что бы сервер знал что за сообщение ему пришло и какой обработчик использовать
        [JsonProperty]
        string body;//JSON'ed мессадж
        int tick;
        string time;
        string token;
        string description;
        public CMessage(int pTick, string pTime, string pToken, string pDescription)
        {
            tick = pTick;
            time = pTime;
            token = pToken;
            description = pDescription;
        }
    }
}
