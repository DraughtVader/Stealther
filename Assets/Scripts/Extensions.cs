using System;
using System.Collections.Generic;

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
}