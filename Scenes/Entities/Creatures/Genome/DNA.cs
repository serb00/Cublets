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

    public DNA VerifyAndFixDNA(BodyPartsCollection bodyPartsCollection)
    {
        bodyPartsCollection.GetBodyPartMinMaxIDs(BodyPartType.Body, out int minID, out int maxID);
        BodyGenes bodyGene = new()
        {
            ID = Mathf.Clamp(BodyGene.ID, minID, maxID),
            Size = Mathf.Clamp(BodyGene.Size, 0.5f, 10f)
        };
        BodyGene = bodyGene;

        BrainGenes brainGene = new()
        {
            NumHiddenLayers = BrainGene.NumHiddenLayers < 0 ? 0 : BrainGene.NumHiddenLayers,
            ConnectionsMethod = BrainGene.ConnectionsMethod
        };
        BrainGene = brainGene;

        bodyPartsCollection.GetBodyPartMinMaxIDs(BodyPartType.Eye, out minID, out maxID);
        for (int i = 0; i < EyesGene.Count; i++)
        {
            EyeGenes eyeGene = new()
            {
                ID = Mathf.Clamp(EyesGene[i].ID, minID, maxID),
                Angle = new(
                    Mathf.Clamp(EyesGene[i].Angle.X, -1, 1),
                    Mathf.Clamp(EyesGene[i].Angle.Y, -1, 1),
                    Mathf.Clamp(EyesGene[i].Angle.Z, -1, 1)
                )
            };
            EyesGene[i] = eyeGene;
        }

        bodyPartsCollection.GetBodyPartMinMaxIDs(BodyPartType.Mouth, out minID, out maxID);
        for (int i = 0; i < MouthsGene.Count; i++)
        {
            MouthGenes mouthGene = new()
            {
                ID = Mathf.Clamp(MouthsGene[i].ID, minID, maxID),
                Angle = new(
                    Mathf.Clamp(MouthsGene[i].Angle.X, -1, 1),
                    Mathf.Clamp(MouthsGene[i].Angle.Y, -1, 1),
                    Mathf.Clamp(MouthsGene[i].Angle.Z, -1, 1)
                )
            };
            MouthsGene[i] = mouthGene;
        }

        return this;
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


