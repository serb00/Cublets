using System.Collections.Generic;

public class EnergyManager
{
    public float CurrentEnergy { get; private set; }
    public float MaxEnergy { get; private set; }

    public float TotalEnergySpend { get; private set; } = 0;
    public float TotalEnergyReplenished { get; private set; } = 0;
    private readonly float EnergyCostMovement = 1;
    private readonly float EnergyCostRotation = 0.5f;
    private readonly float EnergyCostBrainProcessing = 0.2f;
    private readonly float EnergyCostEyeProcessing = 0.1f;

    public EnergyManager(float maxEnergy, float size)
    {
        MaxEnergy = maxEnergy;
        CurrentEnergy = maxEnergy;
        EnergyCostMovement = size;
        EnergyCostRotation = size;
    }

    public void AddEnergy(float energy)
    {
        var previousEnergy = CurrentEnergy;
        CurrentEnergy += energy;
        if (CurrentEnergy > MaxEnergy)
        {
            CurrentEnergy = MaxEnergy;
        }
        TotalEnergyReplenished += CurrentEnergy - previousEnergy;
    }

    public void SpendEnergy(float energy)
    {
        var previousEnergy = CurrentEnergy;
        CurrentEnergy -= energy;
        if (CurrentEnergy < 0)
        {
            CurrentEnergy = 0;
        }
        TotalEnergySpend += previousEnergy - CurrentEnergy;
    }

    public void AdjustMaxEnergy(float maxEnergy)
    {
        MaxEnergy += maxEnergy;
        if (CurrentEnergy > MaxEnergy)
        {
            CurrentEnergy = MaxEnergy;
        }
    }

    public void MaximizeEnergy()
    {
        CurrentEnergy = MaxEnergy;
    }

    public float CalculateEnergyConsumptionMovement(float distance)
    {
        float energyConsumed = distance * EnergyCostMovement; // These are arbitrary multipliers
        return energyConsumed;
    }

    public float CalculateEnergyConsumptionRotation(float angle)
    {
        float energyConsumed = angle * EnergyCostRotation; // These are arbitrary multipliers
        return energyConsumed;
    }

    public float CalculateEnergyConsumptionBrainProcessing(int signalPasses, int neuronConnectionsCount, int neuronsCount)
    {
        float energyConsumed = signalPasses * neuronConnectionsCount * neuronsCount * EnergyCostBrainProcessing;
        return energyConsumed;
    }

    public float CalculateEnergyConsumptionEyeProcessing(int EyeComplexity, int ActivatorPerEntity, float distance, float FOVVertical, float FOVHorizontal)
    {
        float energyConsumed = EyeComplexity * ActivatorPerEntity * distance * (FOVVertical / 180) * (FOVHorizontal / 180) * EnergyCostEyeProcessing;
        return energyConsumed;
    }

    public void Reset()
    {
        CurrentEnergy = MaxEnergy;
        TotalEnergySpend = 0;
        TotalEnergyReplenished = 0;
    }
}
