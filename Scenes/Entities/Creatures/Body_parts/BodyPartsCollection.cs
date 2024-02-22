using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;

public partial class BodyPartsCollection : Node
{
    private readonly Dictionary<BodyPartType, Dictionary<int, string>> bodyParts;

    public BodyPartsCollection()
    {
        bodyParts = new();
        InitializeBodyParts();
    }

    private void InitializeBodyParts()
    {
        string baseDir = "Scenes/Entities/Creatures/Body_parts/";

        // Initialize body parts for bodies
        string bodiesDir = baseDir + "Body/Bodies/";
        bodyParts[BodyPartType.Body] = GetBodyParts(bodiesDir);

        // Initialize body parts for eyes
        string eyesDir = baseDir + "Eye/Eyes/";
        bodyParts[BodyPartType.Eye] = GetBodyParts(eyesDir);

        // Initialize body parts for mouth
        string mouthDir = baseDir + "Mouth/Mouths/";
        bodyParts[BodyPartType.Mouth] = GetBodyParts(mouthDir);
    }

    private static Dictionary<int, string> GetBodyParts(string directory)
    {
        Dictionary<int, string> bodyParts = new();
        // GD.Print($"Loading body parts from {directory}");
        if (Directory.Exists(directory))
        {
            string[] files = Directory.GetFiles(directory, "*.tres");

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (int.TryParse(fileName[(fileName.Find("_") + 1)..], out int id))
                {
                    bodyParts[id] = file;
                }
            }
        }
        foreach (var file in Directory.GetFiles(Utils.GetParentDirectory(directory), "*.tscn"))
        {
            bodyParts[0] = file;
        }

        return bodyParts;
    }

    public string GetBodyPartResourseOfTypeByRandomIndex(BodyPartType type)
    {
        return bodyParts[type][GD.RandRange(0, bodyParts[type].Count)];
    }

    public string GetBodyPartResourseOfTypeByIndex(BodyPartType type, int id)
    {
        return bodyParts[type][id];
    }
}
