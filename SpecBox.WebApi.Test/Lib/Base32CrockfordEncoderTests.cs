using SpecBox.WebApi.Lib;

namespace SpecBox.WebApi.Test.Lib;

public class Base32CrockfordEncoderTests
{
    [Theory]
    [InlineData(new byte[] { 0x11, 0x11, 0x11, 0x11 }, "248H248")]
    [InlineData(new byte[] { 0xba, 0xda, 0x55 }, "QBD5A")]
    [InlineData(new byte[] { 0x3a, 0xeb, 0xca }, "7BNWM")]
    public void FromBase32Crockford_ByteArray_ReturnsCorrectBase32(byte[] input, string expect)
    {
        string result = Base32CrockfordEncoder.Encode(input);

        Assert.Equal(expect, result);
    }

    [Theory]
    [InlineData(new byte[] { 0x11, 0x11, 0x11, 0x11 }, "248H248Y")]
    [InlineData(new byte[] { 0xba, 0xda, 0x55 }, "QBD5A*")]
    [InlineData(new byte[] { 0x3a, 0xeb, 0xca }, "7BNWMK")]
    public void FromBase32Crockford_ByteArray_ReturnsCorrectBase32WithChecksum(byte[] input, string expect)
    {
        string result = Base32CrockfordEncoder.Encode(input, true);

        Assert.Equal(expect, result);
    }
}
