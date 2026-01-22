

using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

public class IDebug : MonoBehaviour
{
    public static void Log(String text)
    {
        MethodBase methodInfo = new StackTrace().GetFrame(1).GetMethod();
        String methodName = methodInfo.Name;
        String className = methodInfo.ReflectedType.Name;

        UnityEngine.Debug.Log($"[{className}.{methodName}] {text}");
    }
}