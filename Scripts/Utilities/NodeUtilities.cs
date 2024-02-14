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
}
