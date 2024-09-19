using System.Text;

namespace SpecBox.WebApi.Lib;

public static class Base32CrockfordEncoder
{
    private static readonly char[] CrockfordBase37Chars = "0123456789ABCDEFGHJKMNPQRSTVWXYZ*~#=U".ToCharArray();
    public static string Encode(byte[] data, bool appendChecksum = false)
    {
        StringBuilder result = new StringBuilder((data.Length * 8 + 4) / 5);

        int currentByte = 0, bitsRemaining = 0, index = 0;

        while (index < data.Length || bitsRemaining > 5)
        {
            if (bitsRemaining < 5)
            {
                currentByte = (currentByte << 8) | data[index++];
                bitsRemaining += 8;
            }
            bitsRemaining -= 5;
            result.Append(CrockfordBase37Chars[(currentByte >> bitsRemaining) & 0x1f]);
        }

        if (bitsRemaining > 0)
        {
            result.Append(CrockfordBase37Chars[(currentByte << (5 - bitsRemaining)) & 0x1f]);
        }
        if(appendChecksum) {
            result.Append(CheckSum(data));
        }

        return result.ToString();
    }

    public static char CheckSum(byte[] data)
    {
        int checksum = 0;

        foreach (byte b in data)
        {
            checksum = (checksum * 256 + b) % 37;
        }

        return CrockfordBase37Chars[checksum];
    }

    public static string ToBase32Crockford(this Guid guid, bool appendChecksum = false)
    {
        return Encode(guid.ToByteArray(), appendChecksum);
    }
}
