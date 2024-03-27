using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    public static string EncodeObject<T>(T obj)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve,
                IncludeFields = true,
                Converters = { new JsonStringEnumConverter() },
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Serialize(obj, options);
        }
        catch (Exception ex)
        {
            GD.PrintErr("Failed to encode object: ", ex.Message);
            return string.Empty;
        }
    }

    public static T DecodeObject<T>(string objString)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(
                objString,
                new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    IncludeFields = true,
                    Converters = { new JsonStringEnumConverter() },
                    PropertyNameCaseInsensitive = true
                }
            );
        }
        catch (Exception ex)
        {
            GD.PrintErr("Failed to decode object: ", ex.Message);
            return default;
        }
    }

    internal static string GetParentDirectory(string directory)
    {
        // Split the path by the separator
        string[] parts = directory.TrimEnd('/').Split('/');

        // Remove the last folder by taking all parts except the last
        return string.Join("/", parts.Take(parts.Length - 1));
    }

    public static void SaveCreatureToFile(Creature creature, string filePath)
    {
        string creatureJson = EncodeObject<Creature>(creature);
        // Step 1. Verify if file exists, if not create it
        if (!System.IO.File.Exists(filePath))
        {
            System.IO.File.Create(filePath).Close();
        }
        // Step 2. Read a collection from file
        string json = System.IO.File.ReadAllText(filePath);
        var collection = DecodeObject<CreatureCollection>(json);
        // Step 3. Add the creature to the collection
        collection.Creatures.Add(creature);
        // Step 4. Save the collection to file
        System.IO.File.WriteAllText(filePath, EncodeObject<CreatureCollection>(collection));
    }

    public static List<Creature> LoadCreaturesFromFile(string filePath)
    {
        // Step 1. Verify if file exists, if not create it
        if (!System.IO.File.Exists(filePath))
        {
            System.IO.File.Create(filePath).Close();
        }
        // Step 2. Read a collection from file
        string json = System.IO.File.ReadAllText(filePath);
        var collection = DecodeObject<CreatureCollection>(json);
        return collection.Creatures;
    }

    public static void RemoveCreatureFromFile(string filePath, Creature creature)
    {
        var creatures = LoadCreaturesFromFile(filePath);
        creatures.Remove(creature);
        SaveCreaturesToFile(filePath, creatures);
    }

    private static void SaveCreaturesToFile(string filePath, List<Creature> creatures)
    {
        // Step 1. Serialize a collection to file
        string json = EncodeObject(new CreatureCollection { Creatures = creatures });
        System.IO.File.WriteAllText(filePath, json);
    }
}

internal class CreatureCollection
{
    public List<Creature> Creatures { get; set; } = new List<Creature>();
}