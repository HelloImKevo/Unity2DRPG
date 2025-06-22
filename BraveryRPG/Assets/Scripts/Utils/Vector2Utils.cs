using System;
using UnityEngine;

public class Vector2Utils
{
    public static Vector2[] DeepCopy(Vector2[] originalArray)
    {
        if (originalArray == null) return null;

        Vector2[] copiedArray = new Vector2[originalArray.Length];
        Array.Copy(originalArray, copiedArray, originalArray.Length);
        return copiedArray;
    }
}
