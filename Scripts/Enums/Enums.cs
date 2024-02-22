/// <summary>
/// Types of entities in the simulation
/// </summary>
public enum EntityType
{
    /// <summary>
    /// Creatures are active simulacras.
    /// </summary>
    Creature,
    /// <summary>
    /// Foods are passive simulacras which are for creatures to eat.
    /// </summary>
    Food,
    /// <summary>
    /// Corps are what left from creatures after death
    /// </summary>
    Corpse
}

public enum BodyPartType
{
    Eye,
    Mouth,
    Body,
    Nose,
    Tongue,
    Lip,
    Teeth
}

/// <summary>
/// Represents the different types of diets an entity can have.
/// </summary>
/// <remarks>
/// The diet type determines the type of food an entity can consume.
/// </remarks>
/// <example>
/// <code>
/// DietType.Carnivore // Suitable for meat-based diets
/// DietType.Herbivore // Suitable for plant-based diets
/// DietType.Omnivore  // Suitable for both meat and plant diets
/// DietType.Insectivore // Specialized for eating insects
/// DietType.Piscivore // Specialized for eating fish
/// DietType.Frugivore // Primarily eats fruits
/// DietType.Detritivore // Feeds on dead organic material
/// </code>
/// </example>
/// <seealso cref="EntityType"/>
/// <seealso cref="BodyPartType"/>
public enum DietType
{
    /// <summary>
    /// Suitable for meat-based diets.
    /// </summary>
    Carnivore,
    /// <summary>
    /// Suitable for plant-based diets.
    /// </summary>
    Herbivore,
    /// <summary>
    /// Suitable for both meat and plant diets.
    /// </summary>
    Omnivore,
    /// <summary>
    /// Specialized for eating insects.
    /// </summary>
    Insectivore,
    /// <summary>
    /// Specialized for eating fish.
    /// </summary>
    Piscivore,
    /// <summary>
    /// Primarily eats fruits.
    /// </summary>
    Frugivore,
    /// <summary>
    /// Feeds on dead organic material.
    /// </summary>
    Detritivore
}

/// <summary>
/// Types of neurons, given their position in the neural network.
/// </summary>
public enum NeuronType
{
    /// <summary>
    /// First layer of neurons.
    /// </summary>
    Input,
    /// <summary>
    /// Last layer of neurons.
    /// </summary>
    Output,
    /// <summary>
    /// Middle layer/s of neurons.
    /// </summary>
    Hidden
}

/// <summary>
/// Various Activation functions for neurons.
/// </summary>
public enum NeuronActivationFunction
{
    /// <summary>
    /// This function takes any real value as input and outputs values 0 or 1, given the threshold value of 0.
    /// </summary>
    BinaryStep,

    /// <summary>
    /// This function takes any real value as input and outputs values in the range of 0 to 1.
    /// </summary>
    Sigmoid,
    /// <summary>
    /// This function takes any real value as input and outputs values in the range of -1 to 1.
    /// </summary>
    HyperbolicTangent,
    /// <summary>
    /// This function takes any real value as input and outputs -1 if value < 0, else outputs 1 if value > 0, else outputs 0 if value = 0.
    /// </summary>
    Sign

}

public enum BrainZoneType
{
    Visual,
    Audial,
    Movement,
    Consumption,
    Internal
}

/// <summary>
/// Represents the different methods of connecting neurons in a neural network.
/// </summary>
public enum NNConnectionsMethod
{
    /// <summary>
    /// Connects every neuron to every neuron from the previous layer.
    /// </summary>
    Full,
    /// <summary>
    /// Connects every neuron to some neurons from the previous layer.
    /// </summary>
    Partial,
    /// <summary>
    /// Connects neurons randomly based on 
    /// </summary>
    Random
}