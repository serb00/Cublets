using System.Collections.Generic;

public class EnergyManager
{
    public float CurrentEnergy { get; private set; }
    public float MaxEnergy { get; private set; }

    public EnergyManager(float maxEnergy)
    {
        MaxEnergy = maxEnergy;
        CurrentEnergy = maxEnergy;
    }

    public void AddEnergy(float energy)
    {
        CurrentEnergy += energy;
        if (CurrentEnergy > MaxEnergy)
        {
            CurrentEnergy = MaxEnergy;
        }
    }

    public void RemoveEnergy(float energy)
    {
        CurrentEnergy -= energy;
        if (CurrentEnergy < 0)
        {
            CurrentEnergy = 0;
        }
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


}
