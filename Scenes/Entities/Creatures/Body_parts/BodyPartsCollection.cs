using System.Collections.Generic;
using System.IO;
using Godot;

public class BodyPartsCollection
{
    private readonly Dictionary<string, Dictionary<int, string>> bodyParts;

    public BodyPartsCollection()
    {
        bodyParts = new Dictionary<string, Dictionary<int, string>>();
        InitializeBodyParts();
    }

    private void InitializeBodyParts()
    {
        string baseDir = "Scenes/Entities/Creatures/Body_parts/";

        // Initialize body parts for bodies
        string bodiesDir = baseDir + "Body/Bodies/";
        bodyParts["Bodies"] = GetBodyParts(bodiesDir);

        // Initialize body parts for eyes
        string eyesDir = baseDir + "Eye/Eyes/";
        bodyParts["Eyes"] = GetBodyParts(eyesDir);

        // Initialize body parts for mouth
        string mouthDir = baseDir + "Mouth/Mouths/";
        bodyParts["Mouths"] = GetBodyParts(mouthDir);
    }

    private static Dictionary<int, string> GetBodyParts(string directory)
    {
        Dictionary<int, string> bodyParts = new();
        GD.Print($"Loading body parts from {directory}");
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

        return bodyParts;
    }
}
