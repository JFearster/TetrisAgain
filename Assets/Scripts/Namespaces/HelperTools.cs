using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelperTools
{
    public class HTools
    {
        /// <summary>
        /// shuffle objects in the container using the Fisher-Yates algorithm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + Random.Range(0, n - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }
        /// <summary>
        /// shuffle objects in the container using the Fisher-Yates algorithm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                int r = i + Random.Range(0, n - i);
                T t = list[r];
                list[r] = list[i];
                list[i] = t;
            }
        }
    }
}