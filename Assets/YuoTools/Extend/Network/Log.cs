using System;
using UnityEngine;

namespace ET
{
    public class Log
    {
        public static void Info(string msg)
        {
            Debug.Log(msg);
        }
        
        public static void Error(string msg)
        {
            Debug.LogError(msg);
        }
        
        public static void Warning(string msg)
        {
            Debug.LogWarning(msg);
        }
        
        public static void Error(Exception e)
        {
            Debug.LogException(e);
        }
    }
}