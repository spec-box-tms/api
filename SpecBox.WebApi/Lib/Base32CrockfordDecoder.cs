namespace SpecBox.WebApi.Lib;

public static class Base32CrockfordDecoder
{
        private static readonly Dictionary<char, int> Base32CrockfordMap = new Dictionary<char, int>()
    {
        {'0', 0}, {'1', 1}, {'2', 2}, {'3', 3}, {'4', 4},
        {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8}, {'9', 9},
        {'A', 10}, {'B', 11}, {'C', 12}, {'D', 13}, {'E', 14},
        {'F', 15}, {'G', 16}, {'H', 17}, {'J', 18}, {'K', 19},
        {'M', 20}, {'N', 21}, {'P', 22}, {'Q', 23}, {'R', 24},
        {'S', 25}, {'T', 26}, {'V', 27}, {'W', 28}, {'X', 29},
        {'Y', 30}, {'Z', 31}
    };
    public static byte[] Decode(string base32)
    {
        Console.WriteLine(base32);
        base32 = base32.ToUpper();
        base32 = base32.TrimEnd('=');

        var byteCount = base32.Length * 5 / 8;
        var returnArray = new byte[byteCount];

        int bitsRemaining = 0, currentByte = 0;
        int index = 0;

        foreach (char c in base32)
        {
            if (!Base32CrockfordMap.ContainsKey(c))
                throw new ArgumentException($"Invalid character '{c}' in Base32 string.");

            int value = Base32CrockfordMap[c];

            if (bitsRemaining > 3)
            {
                currentByte = (currentByte << 5) | value;
                bitsRemaining -= 5;
            }
            else
            {
                currentByte = (currentByte << bitsRemaining) | (value >> (5 - bitsRemaining));
                returnArray[index++] = (byte)currentByte;
                currentByte = value & ((1 << (5 - bitsRemaining)) - 1);
                bitsRemaining += 3;
            }
        }

        if (index != byteCount)
            returnArray[index] = (byte)(currentByte << (8 - bitsRemaining));

        return returnArray;
    }

    public static byte[] FromBase32Crockford(this string base32)
    {
        return Decode(base32);
    }

    public static Guid FromBase32CrockfordGuid(this string base32)
    {
        var buf = Decode(base32);
        if(buf.Length != 128) 
        {
            throw new ArgumentException("Invalid Guid size in Base32");
        }
        return new Guid(buf);
    }
}
