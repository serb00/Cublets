using Godot;
using System;

public static class Utils
{
    private static readonly Random random = new();

    /// <summary>
    /// Search up the nodes tree for the first parent of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="currentNode"></param>
    /// <returns></returns>
    public static T GetFirstParentOfType<T>(Node currentNode) where T : class
    {
        while (currentNode != null)
        {
            if (currentNode is T target)
            {
                return target;
            }
            currentNode = currentNode.GetParent();
        }
        return null;
    }

    /// <summary>
    /// Gets the random enum value of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRandomEnumValue<T>() where T : Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));
        return values[random.Next(values.Length)];
    }

    /// <summary>
    /// Gets the count of values in the specified enum type.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>The count of values in the enum type.</returns>
    public static int GetCountOfEnumValues<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Length;
    }

    /// <summary>
    /// Scales a value between a specified bottom and top boundary.
    /// </summary>
    /// <param name="val">The value to be scaled.</param>
    /// <param name="bottomBoundary">The bottom boundary of the scaling range.</param>
    /// <param name="topBoundary">The top boundary of the scaling range.</param>
    /// <returns>The scaled value.</returns>
    public static float ScaleValue(float val, float bottomBoundary, float topBoundary)
    {
        float scaledValue = (val - bottomBoundary) / (topBoundary - bottomBoundary);
        if (bottomBoundary < 0)
        {
            return 2 * scaledValue - 1;
        }
        else
        {
            return scaledValue;
        }
    }
}