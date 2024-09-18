using System.Text;

namespace SpecBox.WebApi.Lib;

public static class Base32CrockfordEncoder
{
    private static readonly char[] CrockfordBase32Chars = "0123456789ABCDEFGHJKMNPQRSTVWXYZ".ToCharArray();
    public static string Encode(byte[] data)
    {
        StringBuilder result = new StringBuilder((data.Length * 8 + 4) / 5);

        int currentByte = 0, bitsRemaining = 8, index = 0;

        while (index < data.Length)
        {
            if (bitsRemaining > 5)
            {
                currentByte = (currentByte << 8) | (data[index++] & 0xff);
                bitsRemaining -= 8;
            }

            result.Append(CrockfordBase32Chars[(currentByte >> bitsRemaining) & 0x1f]);
            bitsRemaining += 5;
        }

        if (bitsRemaining > 0 && bitsRemaining < 8)
        {
            result.Append(CrockfordBase32Chars[(currentByte << (5 - bitsRemaining)) & 0x1f]);
        }

        return result.ToString();
    }



    public static string ToBase32Crockford(this Guid guid)
    {
        return Base32CrockfordEncoder.Encode(guid.ToByteArray());
    }
}
