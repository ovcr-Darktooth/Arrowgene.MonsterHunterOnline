using Arrowgene.MonsterHunterOnline.Service.Data;
using Xunit;

namespace Arrowgene.MonsterHunterOnline.Test.Service.Data;

public class Crc32Test
{
    [Fact]
    public void Compute_CheckValue()
    {
        Assert.Equal(0xCBF43926u, Crc32.Compute("123456789"));
    }

    [Fact]
    public void Compute_EmptyString_IsZero()
    {
        Assert.Equal(0u, Crc32.Compute(""));
    }

    [Fact]
    public void ComputeLowercase_MatchesLowerCasedInput()
    {
        uint expected = Crc32.Compute("template_attack_10");
        Assert.Equal(expected, Crc32.ComputeLowercase("Template_Attack_10"));
        Assert.Equal(expected, Crc32.ComputeLowercase("TEMPLATE_ATTACK_10"));
    }

    [Fact]
    public void ComputeLowercase_DiffersFromCaseSensitiveWhenMixedCase()
    {
        Assert.NotEqual(Crc32.Compute("Template_Attack_10"), Crc32.ComputeLowercase("Template_Attack_10"));
    }
}
