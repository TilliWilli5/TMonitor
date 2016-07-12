using System;
using System.Collections.Generic;
using System.Text;
using TMonitor;
using Newtonsoft.Json;
using System.IO;

namespace TestingProject
{
    using NewNews = CNewsCollector;
    class SendPingOnly
    {
        //Для класса CAutoLoader
        public static string configurationFileName = "conf.json";
        //
        public static string serverAddress = "127.0.0.1";
        public static string httpMethod;
        public static string httpContentType;
        public static int correspondenceFrequency;
        public static string installationToken;
        public static string installationDescription;
        //Необходимо добавить следующие параметры всюду
        public static string scheme = "http://";
        public static uint port = 8812;
        public static uint requestTimeout = 1000;
        //Дописать кастомизацию (из скрипта CLogger)
        public static string logDirectory = "Logs/";
        public static string defaultFileName = "log";
        public static string prefixToFileName;
        public static string postfixToFileName;
        public static string timeFormatString = "[yyyy-MM-ddTHH]";
        public static string fileExtension = "nfl";//.nfl - news feed log

        static void Main(string[] args)
        {
            //Как только приложение стартует добавляем первую новость о том что приложение запустилось
            NewNews.ApplicationStart("mark!main function");
            //Узнаем полный путь к файлу настроек
            string pathNameToConf = Application.dataPath + "/" + configurationFileName;
            //Составляем полный путь к директории куда будут сохраняться файлы логов
            string logDirectory = Application.persistentDataPath + "/Logs/";//Необходимо дописать кастомизацию
            //Используем класс автозагрузки для получения всех интересующих нас параметров для дальнейшей работы остальных модулей
            if (CAutoLoader.LoadFromFile(pathNameToConf) == false)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Debug.Log("Error loading config file.");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            //Входные параметры от скрипта имеют больший приоритет посему делаем подмену там где это надо
            //Дальнейшая работа должна происходить исключительно с параметрами из словаря setup
            Dictionary<string, string> setup = CAutoLoader.GetClonedConfiguration();
            if (serverAddress != null)
                setup["serverAddress"] = serverAddress;
            if (httpMethod != null)
                setup["httpMethod"] = httpMethod;
            if (httpContentType != null)
                setup["httpContentType"] = httpContentType;
            if (correspondenceFrequency != 0)
                setup["correspondenceFrequency"] = correspondenceFrequency.ToString();
            if (installationToken != null)
                setup["installationToken"] = installationToken;
            if (installationDescription != null)
                setup["installationDescription"] = installationDescription;
            if (logDirectory != null)
                setup["logDirectory"] = logDirectory;
            //Дальше создаём моего любимого инспектора (ака "the Boss")
            //Инициализируем с помощью словаря setup
            CClientInspector theInspector = new CClientInspector(setup["installationToken"], setup["installationDescription"]);
            theInspector.Initialize(setup);
            //Создаем и настраиваем Почтальона кый берет на себе весь гемор работы с сетью
            CClientPostman thePostman = new CClientPostman();
            thePostman.Initialize(setup);
            //Создаем и настраиваем класс Архиватора кый отвечает за локальное хранение данных
            CLogger theLogger = new CLogger();
            theLogger.Initialize(setup);
            //Добавляем почтальона как дефолтную почтовую службу к Инспектору.
            //Добавляем Архиватора как дефлотную службу локального хранения данных
            theInspector.postalService = thePostman;
            theInspector.archiveService = theLogger;
            //Проверяем заданный токен инсталяции, есть ли такой в БД на сервере - чтоб удостовериться что Телеметрия бдует принематься по данной инсталяции.
            //Если такой ключ не зарегестрирован то кидаем исключение в качестве напоминания разработчику что надо обновить ключ епт
            //theInspector.CheckInstallationToken();
            //Если передача данных не удалась что делать? Вешаем обработчики для такой ситуации
            thePostman.ErrorDelivery += new ErrorDeliveryHandler(theInspector.MessageReturn);//Необходимо продумать ситуацию в кой данные передаются из уже записанного лог файла и следовательно повторно архивировать в случае не удачи их не нужно
            //Создаем инициативу кая будет каждую минуту слать Пинг-покет на сервер
            //CInitiative thePingSignalToServer = new CInitiative(theInspector.SendPingSignalToServer, 1 * 3 * 1000, true, true, "Ping 1m", "Каждую минуту отсылает Пинг-сигнал на сервер");
            //CInitiative theLogsToServer = new CInitiative(theInspector.SendLogsToServer, 1 * 7 * 1000, true, true, "SendingLogs 1h", "Отсылка статистики из логов на сервер");
            //CInitiative theNewsFeedToLogs = new CInitiative(theInspector.NewsFeedToLogs, 1 * 3 * 1000, true, true, "NewsFeedToLogs 15m", "Забираем данные из ленты новостей и помещаем в логи, после чего очищаем ленту новостей");
            //CInitiative theWindowDressing = new CInitiative(theInspector.WindowDressing, 1 * 1 * 1000, true, true, "WindowDressing 1m", "Заполняем Ленту новостей чем-нибудь. Зондирующие сигналы");
            //
            //Тестирование фишек ниже. Все что выше будет переноситься в продакшен
            //
            //NewNews.ApplicationEnd("mark!main function");
            do
            {
                theInspector.SendPingSignalToServer();
            }
            while (Console.ReadLine() != "x");
            //Приложение заканчивает свою работу. Генерируем новость об этом. Эта строчка должна оставаться последней
            //NewNews.ApplicationEnd("mark!main function");
        }
    }
}
