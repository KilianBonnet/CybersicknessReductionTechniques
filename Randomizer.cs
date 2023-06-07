using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : MonoBehaviour {

    private int[] finalMatrixIndex;
    private int[] finaltrialIndex;

	// Use this for initialization
	void Start () {
        finalMatrixIndex = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        finaltrialIndex = new int [] { 0, 1, 2 };
        
        shuffleArray(finalMatrixIndex);
        shuffleArray(finaltrialIndex);
        print(GetfinalMatrixIndexPosition(0));
		
	}
	
    public static void shuffleArray(int [] arr)
    {
        int lenght = arr.Length;
        for(int i=0;i<lenght; i++)
        {
            swap(arr, i, Random.Range(0, lenght - 1));

        }

    }
	
    public static void swap(int[] arr,int a, int b)
    {
        int temp = arr[a];
        arr[a] = arr[b];
        arr[b] = temp;
    }
     public int[] GetfinalMatrixIndex()
    {
        return finalMatrixIndex;
    }
    public int GetfinalMatrixIndexPosition(int position)
    {
        return finalMatrixIndex[position];
    }

    public int[] GetfinaltrialIndex()
    {
        return finaltrialIndex;
    }
    public int GetfinaltrialIndexPosition(int position)
    {
        return finaltrialIndex[position];
    }


}
