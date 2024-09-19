namespace SpecBox.WebApi.Lib;

public static class Base32CrockfordDecoder
{
    private static readonly Dictionary<char, int> Base32CrockfordMap = new Dictionary<char, int>()
    {
        {'0', 0}, {'1', 1}, {'I', 1}, {'L', 1}, {'2', 2}, {'3', 3}, {'4', 4},
        {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8}, {'9', 9},
        {'A', 10}, {'B', 11}, {'C', 12}, {'D', 13}, {'E', 14},
        {'F', 15}, {'G', 16}, {'H', 17}, {'J', 18}, {'K', 19},
        {'M', 20}, {'N', 21}, {'P', 22}, {'Q', 23}, {'R', 24},
        {'S', 25}, {'T', 26}, {'V', 27}, {'W', 28}, {'X', 29},
        {'Y', 30}, {'Z', 31}
    };
    public static byte[] Decode(string base32, bool verifyChecksum = false)
    {
        if (base32 == null) throw new ArgumentNullException(nameof(base32));

        if (base32.Length == 0)
            return [];

        base32 = base32.ToUpper();
        
        var checksum = base32.Last();
        if (verifyChecksum)
        {
            base32 = base32.Substring(0, base32.Length - 1);
        }

        var byteCount = base32.Length * 5 / 8;
        var buffer = new byte[byteCount];

        int bitsRemaining = 0, currentByte = 0;
        int index = 0;

        foreach (char c in base32)
        {
            if (!Base32CrockfordMap.ContainsKey(c))
                throw new ArgumentException($"Invalid character '{c}' in Base32 string.");

            int value = Base32CrockfordMap[c];

            currentByte = (currentByte << 5) | value;
            bitsRemaining += 5;

            if (bitsRemaining >= 8)
            {
                buffer[index++] = (byte)(currentByte >> (bitsRemaining - 8));
                bitsRemaining -= 8;
            }
        }

        if (index != byteCount)
            buffer[index] = (byte)(currentByte << (8 - bitsRemaining));

        if (verifyChecksum)
        {
            var computedChecksum = Base32CrockfordEncoder.CheckSum(buffer);
            if (checksum != computedChecksum)
            {
                throw new Exception("Checksum verification error");
            }
        }

        return buffer;
    }

    public static byte[] FromBase32Crockford(this string base32, bool verifyChecksum = false)
    {
        return Decode(base32, verifyChecksum);
    }

    public static Guid FromBase32CrockfordGuid(this string base32, bool verifyChecksum = false)
    {
        var buf = Decode(base32, verifyChecksum);
        if (buf.Length != 16)
        {
            throw new ArgumentException("Invalid Guid size in Base32");
        }
        return new Guid(buf);
    }
}
