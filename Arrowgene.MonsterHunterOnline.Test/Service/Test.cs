﻿using Arrowgene.MonsterHunterOnline.Service;
using Arrowgene.MonsterHunterOnline.Service.TqqApi.Crypto;
using Xunit;

namespace Arrowgene.MonsterHunterOnline.Test.Service;

public class Test
{
    [Fact]
    public void TestEncrypt()
    {
        PlayerState ps = new PlayerState(null);
        ps.SendBruteForce();
    }
}