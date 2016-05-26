using System;
using System.Collections.Generic;
using System.Text;

namespace TestingProject
{
    public class Debug
    {
        public static void Log(Object pValue)
        {
            Console.WriteLine(pValue);
        }
    }
    public class Application
    {
        public static string persistentDataPath = @"C:\Users\Tilli\Documents\Visual Studio 2015\Projects\TMonitor\TestingProject\bin\Debug";
        public static string dataPath = @"C:\Users\Tilli\Documents\Visual Studio 2015\Projects\TMonitor\TestingProject";
    }
}
