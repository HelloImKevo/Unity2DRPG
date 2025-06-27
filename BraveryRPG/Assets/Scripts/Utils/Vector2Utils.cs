using System;
using UnityEngine;

/// <summary>
/// Utility class providing helper methods for working with Vector2 arrays and operations.
/// </summary>
public class Vector2Utils
{
    /// <summary>
    /// Creates a deep copy of a Vector2 array, preserving all element values.
    /// </summary>
    /// <param name="originalArray">The original Vector2 array to copy.</param>
    /// <returns>A new Vector2 array containing copies of all elements, or null if the input is null.</returns>
    public static Vector2[] DeepCopy(Vector2[] originalArray)
    {
        if (originalArray == null) return null;

        Vector2[] copiedArray = new Vector2[originalArray.Length];
        Array.Copy(originalArray, copiedArray, originalArray.Length);
        return copiedArray;
    }
}
