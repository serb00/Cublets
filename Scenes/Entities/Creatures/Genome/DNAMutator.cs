using System;
using System.Collections.Generic;
using Godot;

public class DNAMutator
{
    private static readonly Random random = new();
    /// <summary>
    /// Mutation rate for DNA genes.
    /// Example: 0.05f means 5% mutation rate.
    /// </summary>
    private const float mutationRate = 0.05f;
    private readonly BodyPartsCollection bodyPartsCollection;

    public static DNA MutateCreatureDNA(DNA parent1, DNA parent2)
    {
        // Combine EntityTypeGene
        var entityType = (random.Next(2) == 0) ? parent1.EntityTypeGene : parent2.EntityTypeGene;

        // Combine and maybe mutate BodyGene
        var bodyGene = MutateBodyGenes(parent1.BodyGene, parent2.BodyGene);

        // Combine and maybe mutate BrainGene
        var brainGene = MutateBrainGenes(parent1.BrainGene, parent2.BrainGene);

        // Combine and maybe mutate EyesGene
        var eyesGene = MutateEyeGenes(parent1.EyesGene, parent2.EyesGene);

        // Combine and maybe mutate MouthsGene
        var mouthsGene = MutateMouthGenes(parent1.MouthsGene, parent2.MouthsGene);

        // Return the new DNA instance
        return new DNA(entityType, bodyGene, brainGene, eyesGene, mouthsGene);
    }

    private static DNA.BodyGenes MutateBodyGenes(DNA.BodyGenes gene1, DNA.BodyGenes gene2)
    {
        // Example combining logic for body genes
        var size = (gene1.Size + gene2.Size) / 2;
        if (ShouldMutate()) size += MutateValue(size);

        // Choose type and ID similarly or based on specific logic
        return new DNA.BodyGenes { ID = gene1.ID, Size = size };
    }

    private static DNA.BrainGenes MutateBrainGenes(DNA.BrainGenes gene1, DNA.BrainGenes gene2)
    {
        // Similar logic for brain genes
        var numHiddenLayers = (gene1.NumHiddenLayers + gene2.NumHiddenLayers) / 2;
        if (ShouldMutate()) numHiddenLayers += MutateValue();
        var connectionsMethod = (random.Next(2) == 0) ? gene1.ConnectionsMethod : gene2.ConnectionsMethod;

        return new DNA.BrainGenes { NumHiddenLayers = numHiddenLayers, ConnectionsMethod = connectionsMethod };
    }

    private static List<DNA.EyeGenes> MutateEyeGenes(List<DNA.EyeGenes> list1, List<DNA.EyeGenes> list2)
    {
        // Simple example for lists. Real logic might involve more complex merging and mutation.
        var resultList = new List<DNA.EyeGenes>();

        int maxLength = Math.Max(list1.Count, list2.Count);
        for (int i = 0; i < maxLength; i++)
        {
            if (i < list1.Count && i < list2.Count)
            {
                // Both lists have this element, choose one and mutate
                DNA.EyeGenes chosenGene = (random.Next(2) == 0) ? list1[i] : list2[i];
                if (ShouldMutate())
                {
                    chosenGene.ID += MutateValue();
                    chosenGene.Angle = new Vector3(
                        chosenGene.Angle.X + MutateValue(chosenGene.Angle.X),
                        chosenGene.Angle.Y + MutateValue(chosenGene.Angle.Y),
                        chosenGene.Angle.Z + MutateValue(chosenGene.Angle.Z)
                    );
                }
                resultList.Add(chosenGene);
            }
            else if (i < list1.Count)
            {
                // Only list1 has this element, mutate and add
                DNA.EyeGenes chosenGene = list1[i];
                if (ShouldMutate())
                {
                    chosenGene.ID += MutateValue();
                    chosenGene.Angle = new Vector3(
                        chosenGene.Angle.X + MutateValue(chosenGene.Angle.X),
                        chosenGene.Angle.Y + MutateValue(chosenGene.Angle.Y),
                        chosenGene.Angle.Z + MutateValue(chosenGene.Angle.Z)
                    );
                }
                resultList.Add(chosenGene);
            }
            else if (i < list2.Count)
            {
                // Only list2 has this element, mutate and add
                DNA.EyeGenes chosenGene = list2[i];
                if (ShouldMutate())
                {
                    chosenGene.ID += MutateValue();
                    chosenGene.Angle = new Vector3(
                        chosenGene.Angle.X + MutateValue(chosenGene.Angle.X),
                        chosenGene.Angle.Y + MutateValue(chosenGene.Angle.Y),
                        chosenGene.Angle.Z + MutateValue(chosenGene.Angle.Z)
                    );
                }
                resultList.Add(chosenGene);
            }
        }

        return resultList;
    }

    private static List<DNA.MouthGenes> MutateMouthGenes(List<DNA.MouthGenes> list1, List<DNA.MouthGenes> list2)
    {
        // Simple example for lists. Real logic might involve more complex merging and mutation.
        var resultList = new List<DNA.MouthGenes>();

        int maxLength = Math.Max(list1.Count, list2.Count);
        for (int i = 0; i < maxLength; i++)
        {
            if (i < list1.Count && i < list2.Count)
            {
                // Both lists have this element, choose one and mutate
                DNA.MouthGenes chosenGene = (random.Next(2) == 0) ? list1[i] : list2[i];
                if (ShouldMutate())
                {
                    chosenGene.ID += MutateValue();
                    chosenGene.Angle = new Vector3(
                        chosenGene.Angle.X + MutateValue(chosenGene.Angle.X),
                        chosenGene.Angle.Y + MutateValue(chosenGene.Angle.Y),
                        chosenGene.Angle.Z + MutateValue(chosenGene.Angle.Z)
                    );
                }
                resultList.Add(chosenGene);
            }
            else if (i < list1.Count)
            {
                // Only list1 has this element, mutate and add
                DNA.MouthGenes chosenGene = list1[i];
                if (ShouldMutate())
                {
                    chosenGene.ID += MutateValue();
                    chosenGene.Angle = new Vector3(
                        chosenGene.Angle.X + MutateValue(chosenGene.Angle.X),
                        chosenGene.Angle.Y + MutateValue(chosenGene.Angle.Y),
                        chosenGene.Angle.Z + MutateValue(chosenGene.Angle.Z)
                    );
                }
                resultList.Add(chosenGene);
            }
            else if (i < list2.Count)
            {
                // Only list2 has this element, mutate and add
                DNA.MouthGenes chosenGene = list2[i];
                if (ShouldMutate())
                {
                    chosenGene.ID += MutateValue();
                    chosenGene.Angle = new Vector3(
                        chosenGene.Angle.X + MutateValue(chosenGene.Angle.X),
                        chosenGene.Angle.Y + MutateValue(chosenGene.Angle.Y),
                        chosenGene.Angle.Z + MutateValue(chosenGene.Angle.Z)
                    );
                }
                resultList.Add(chosenGene);
            }
        }

        return resultList;
    }

    private static bool ShouldMutate()
    {
        return random.NextDouble() < mutationRate;
    }

    private static float MutateValue(float value)
    {
        // Apply a small random change
        return (float)(random.NextDouble() * 2 - 1) * 0.05f * value; // +/- 5% change
    }

    private static int MutateValue()
    {
        // Apply change by 1 or -1
        return (random.Next(2) == 0 ? 1 : -1);
    }
}