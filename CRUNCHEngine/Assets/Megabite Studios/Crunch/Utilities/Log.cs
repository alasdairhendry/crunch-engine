using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crunch.Utilities
{
    public static class Log
    {
        public static void Send(object data)
        {
            Debug.Log(data);
        }

        public static object Clear()
        {
            return "heck";
        }
    }
}
