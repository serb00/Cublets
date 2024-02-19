using System.Collections.Generic;
using System.IO;
using System.Text;
using Godot;

public class DNA
{
    public struct BodyStruct
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public float Size { get; set; }
    }

    public struct BrainStruct
    {
        public int Complexity { get; set; }
        public int NumLayers { get; set; }
    }

    public struct EyeStruct
    {
        public int ID { get; set; }
        public Vector3 Angle { get; set; }
    }

    public struct MouthStruct
    {
        public int ID { get; set; }
        public Vector3 Angle { get; set; }
    }

    public EntityType EntityType { get; set; }
    public BodyStruct BodyData { get; set; }
    public BrainStruct BrainData { get; set; }
    public Brain Brain { get; set; }
    public List<EyeStruct> EyesData { get; set; }
    public List<MouthStruct> MouthsData { get; set; }

    public DNA()
    {
        EyesData = new List<EyeStruct>();
        MouthsData = new List<MouthStruct>();
    }

    public DNA(
        EntityType entityType,
        BodyStruct bodyData,
        BrainStruct brainData,
        List<EyeStruct> eyesData,
        List<MouthStruct> mouthsData)
    {
        EntityType = entityType;
        BodyData = bodyData;
        BrainData = brainData;
        EyesData = eyesData;
        MouthsData = mouthsData;
    }

    public string GetDNAString()
    {
        StringBuilder builder = new();

        switch (EntityType)
        {
            case EntityType.Creature:
                builder.Append("");
                break;

            case EntityType.Food:

            default:
                break;
        }

        builder.Append("");
        builder.Append("");
        builder.Append("");
        builder.Append("");


        return builder.ToString();
    }
}


