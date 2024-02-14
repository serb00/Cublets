using System;
using System.Text.Json;
using System.Linq;
using Godot;

public static class DNASerializer
{


    public static string IntToBinaryString(int value, int lenght = 32)
    {
        string binary = Convert.ToString(value, 2);
        return binary.PadLeft(lenght, '0');
    }

    public static int BinaryStringToInt(string binary)
    {
        return Convert.ToInt32(binary, 2);
    }

    public static string FloatToBinaryString(float value, int lenght)
    {
        // Convert float to bytes, then to binary string
        byte[] bytes = BitConverter.GetBytes(value);
        string binaryString = string.Join(string.Empty, bytes.Select(b => Convert.ToString(b, 2).PadLeft(lenght, '0')));
        return binaryString;
    }
    public static float BinaryStringToFloat(string binaryString)
    {
        if (binaryString.Length != 32)
            throw new ArgumentException("Binary string must be 32 characters long.");

        // Convert binary string to bytes
        int numOfBytes = binaryString.Length / 8;
        byte[] bytes = new byte[numOfBytes];
        for (int i = 0; i < numOfBytes; i++)
        {
            string byteString = binaryString.Substring(i * 8, 8);
            bytes[i] = Convert.ToByte(byteString, 2);
        }

        // Convert bytes to float
        float result = BitConverter.ToSingle(bytes, 0);
        return result;
    }

    public static string EncodeDNA(DNA dna)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(dna, options);
        }
        catch (Exception ex)
        {
            GD.PrintErr("Failed to encode DNA: ", ex.Message);
            return string.Empty;
        }
    }

    public static DNA DecodeDNA(string dnaString)
    {
        try
        {
            return JsonSerializer.Deserialize<DNA>(dnaString);
        }
        catch (Exception ex)
        {
            GD.PrintErr("Failed to decode DNA: ", ex.Message);
            return null;
        }
    }
}
