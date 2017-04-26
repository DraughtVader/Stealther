using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

internal static class RandomExtensions
{
    public static void ShuffleInPlace<T> (this T[] array)
    {
        var rng = new Random();
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    public static void ShuffleInPlace<T> (this List<T> list)
    {
        var rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }

    public static string ToHex(this Color colour)
    {
        Color32 c = colour;
        var hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);
        return hex;
    }
}