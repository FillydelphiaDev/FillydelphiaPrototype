using System;
using log4net;
using UnityEngine;

namespace Utils
{
    public static class CommonUtils
    {
        /// <summary>
        /// Create simple array copy.
        /// </summary>
        public static T[] Copy<T>(this T[] arr)
        {
            T[] copy = new T[arr.Length];
            Array.Copy(arr, copy, arr.Length);
            return copy;
        }

        public static string ToStringG3(this Vector2 vec)
        {
            return vec.ToString("G3");
        }

        public static string ToStringG6(this Vector2 vec)
        {
            return vec.ToString("G6");
        }

        public static string ToStringG3(this Vector3 vec)
        {
            return vec.ToString("G3");
        }

        public static string ToStringG6(this Vector3 vec)
        {
            return vec.ToString("G6");
        }

        public static ILog GetInstanceLogger<T>(int instance)
        {
            return LogManager.GetLogger(typeof(T).FullName + ".Instance." + instance);
        }
    }
}
