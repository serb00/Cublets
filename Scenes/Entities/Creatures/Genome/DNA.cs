using System.Collections.Generic;
using System.IO;
using System.Text;
using Godot;

/// <summary>
/// Class to store the DNA of an entity
/// </summary>
public class DNA
{
    public struct BodyGenes
    {
        public int ID { get; set; }
        public BodyPartType Type { get; set; }
        public float Size { get; set; }
    }

    public struct BrainGenes
    {
        public int NumHiddenLayers { get; set; }
        public NNConnectionsMethod ConnectionsMethod { get; set; }
    }

    public struct EyeGenes
    {
        public int ID { get; set; }
        public Vector3 Angle { get; set; }
    }

    public struct MouthGenes
    {
        public int ID { get; set; }
        public Vector3 Angle { get; set; }
    }

    public EntityType EntityTypeGene { get; set; }
    public BodyGenes BodyGene { get; set; }
    public BrainGenes BrainGene { get; set; }
    // public Brain Brain { get; set; }
    public List<EyeGenes> EyesGene { get; set; }
    public List<MouthGenes> MouthsGene { get; set; }

    public DNA()
    {
        EyesGene = new List<EyeGenes>();
        MouthsGene = new List<MouthGenes>();
    }

    public DNA(
        EntityType entityType,
        BodyGenes bodyData,
        BrainGenes brainData,
        List<EyeGenes> eyesData,
        List<MouthGenes> mouthsData)
    {
        EntityTypeGene = entityType;
        BodyGene = bodyData;
        BrainGene = brainData;
        EyesGene = eyesData;
        MouthsGene = mouthsData;
    }

    public string GetDNAString()
    {
        StringBuilder builder = new();

        switch (EntityTypeGene)
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


