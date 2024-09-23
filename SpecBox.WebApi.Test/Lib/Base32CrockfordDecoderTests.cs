using SpecBox.WebApi.Lib;

namespace SpecBox.WebApi.Test.Lib;

public class Base32CrockfordDecoderTests
{
    [Theory]
    [InlineData("248H248", new byte[] { 0x11, 0x11, 0x11, 0x11 })]
    [InlineData("QBD5A", new byte[] { 0xba, 0xda, 0x55 })]
    [InlineData("7BNWM", new byte[] { 0x3a, 0xeb, 0xca })]
    [InlineData("K1WG", new byte[] { 0x98, 0x79 })]
    [InlineData("KLWG", new byte[] { 0x98, 0x79 })]
    [InlineData("kiwg", new byte[] { 0x98, 0x79 })]
    public void FromBase32Crockford_ValidString_ReturnsCorrectGuid(string input, byte[] expect)
    {
        byte[] result = Base32CrockfordDecoder.FromBase32Crockford(input);

        Assert.Equal(expect, result);
    }

    [Theory]
    [InlineData("248H248Y", new byte[] { 0x11, 0x11, 0x11, 0x11 })]
    [InlineData("QBD5A*", new byte[] { 0xba, 0xda, 0x55 })]
    [InlineData("7BNWMK", new byte[] { 0x3a, 0xeb, 0xca })]
    [InlineData("K1WG=", new byte[] { 0x98, 0x79 })]
    [InlineData("KLWG=", new byte[] { 0x98, 0x79 })]
    [InlineData("kiwg=", new byte[] { 0x98, 0x79 })]
    public void FromBase32Crockford_ValidStringWithChecksum_ReturnsCorrectGuid(string input, byte[] expect)
    {
        byte[] result = Base32CrockfordDecoder.FromBase32Crockford(input, true);

        Assert.Equal(expect, result);
    }
}
