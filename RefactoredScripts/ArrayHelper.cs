using UnityEngine;

public static class ArrayHelper
{
    /// <summary>
    /// Shuffle an array O(n).
    /// </summary>
    public static void ShuffleArray<T>(T[] arr)
    {
        int lenght = arr.Length;
        for (int i = 0; i < lenght; i++)
            Swap(arr, i, Random.Range(0, lenght - 1));
    }
    
    /// <summary>
    /// Swap the value of 2 given index of an given array
    /// </summary>
    private static void Swap<T>(T[] array, int a, int b)
    {
        (array[a], array[b]) = (array[b], array[a]);
    }
}
