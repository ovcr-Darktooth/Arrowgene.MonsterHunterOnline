using Arrowgene.Logging;
using Arrowgene.MonsterHunterOnline.Protocol.Old.Structures;
using Arrowgene.MonsterHunterOnline.Service.CsProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Arrowgene.MonsterHunterOnline.Service.System.MonsterAISystem
{
    /// <summary>Pre-placed MHMonsterSpawnPoint entity from the level file.</summary>
    public record SpawnPoint(ulong Guid, float Px, float Py, float Pz, float Qw, float Qx, float Qy, float Qz);
    public class MonsterAIManager
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(MonsterAIManager));

        // NetId must match a pre-placed MHMonsterSpawnPoint EntityId in the level file.
        // level_002 (mission to kill em003) has EntityIds starting at 71.
        private static uint _nextNetId = 71;

        // EntityId → SpawnPoint for all pre-placed MHMonsterSpawnPoint entities in level_002.
        // NetId sent to the client must match one of these EntityIds or the client crashes.
        private static readonly Dictionary<uint, SpawnPoint> Level002SpawnPoints = new()
        {
            [71] = new(0x4F5AFBA2FB639E02UL, 1596.1078f, 1636.907f, 157.47589f, 0.95881969f, 0.28401545f, 0f, 0f),
            [72] = new(0x4D68DBF5752E8500UL, 1466.2253f, 865.42139f, 320.75f, 0.77714592f, 0.62932044f, 0f, 0f),
            [73] = new(0x46D7E4CC1E18B901UL, 1471.1782f, 942.72211f, 320.75f, 0.77714592f, 0.62932044f, 0f, 0f),
            [74] = new(0x400D75E30301FB57UL, 1444.6707f, 945.73865f, 320.41666f, 0.77714592f, 0.62932044f, 0f, 0f),
            [75] = new(0x444F80B71BFB395EUL, 299.83826f, 256.62781f, 358f, 0.77714592f, 0.62932044f, 0f, 0f),
            [76] = new(0x4C847B2AA966AF5FUL, 1619.8164f, 1645.5601f, 142.61633f, 0.95881969f, 0.28401542f, 0f, 0f),
            [77] = new(0x4069D52AE447ED5FUL, 1443.1484f, 943.27551f, 320.75f, 0.77714592f, 0.62932044f, 0f, 0f),
            [78] = new(0x4EE583A0717A6E62UL, 294.73999f, 249.22955f, 358f, 0.77714592f, 0.62932044f, 0f, 0f),
            [79] = new(0x461759BDB5EF6A66UL, 326.37384f, 1015.5868f, 156.01541f, 0.95881969f, 0.28401542f, 0f, 0f),
            [80] = new(0x429F36D833CAA066UL, 305.85663f, 1011.8296f, 156.01541f, 0.95881969f, 0.28401542f, 0f, 0f),
            [81] = new(0x4CBD7F95C1ACF26DUL, 325.62399f, 1025.0155f, 156.14041f, 0.95881969f, 0.28401542f, 0f, 0f),
            [84] = new(0x47E789804FFCFD7BUL, 966.6355f, 987.43756f, 117.5779f, 0.77714592f, 0.62932044f, 0f, 0f),
            [85] = new(0x4ED524B68531F782UL, 1619.9659f, 1627.7957f, 142.86633f, 0.95881969f, 0.28401542f, 0f, 0f),
            [86] = new(0x4A33B4E52B3DC088UL, 1447.2682f, 936.02759f, 320.74911f, 0.77714592f, 0.62932044f, 0f, 0f),
            [87] = new(0x465A334F7E382191UL, 308.81824f, 1024.9025f, 156.01541f, 0.95881969f, 0.28401542f, 0f, 0f),
            [88] = new(0x4691C5F1AD168591UL, 1583.3585f, 1612.5532f, 142.61633f, 0.95881969f, 0.28401542f, 0f, 0f),
            [89] = new(0x4693C2B1519FCF94UL, 1417.3091f, 930.95557f, 320.75f, 0.77714592f, 0.62932044f, 0f, 0f),
            [91] = new(0x43CBE00B56D63296UL, 1619.1138f, 1632.6552f, 157.47589f, 0.95881969f, 0.28401545f, 0f, 0f),
            [92] = new(0x4EF97FB0BC454F9AUL, 1604.9335f, 1647.8029f, 157.47589f, 0.95881969f, 0.28401545f, 0f, 0f),
            [93] = new(0x4CF495EADEBFB69BUL, 1608.1335f, 1625.2286f, 157.47589f, 0.95881969f, 0.28401545f, 0f, 0f),
            [94] = new(0x410442A45C0C599FUL, 296.04138f, 230.63652f, 358f, 0.77714592f, 0.62932044f, 0f, 0f),
            [95] = new(0x43A879014706DCA1UL, 1598.5479f, 1628.6501f, 157.47589f, 0.95881969f, 0.28401545f, 0f, 0f),
            [96] = new(0x49711A53C98B30A7UL, 287.87793f, 240.47337f, 358.51355f, 0.77714592f, 0.62932044f, 0f, 0f),
            [97] = new(0x452B3FB282A78006UL, 1487.4697f, 888.80109f, 320.88257f, 0.77714592f, 0.62932044f, 0f, 0f),
            [98] = new(0x4D2A012A50A26908UL, 970.10052f, 992.98969f, 117.5779f, 0.77714592f, 0.62932044f, 0f, 0f),
            [99] = new(0x46A6053720A21D0CUL, 983.53888f, 987.55316f, 117.5779f, 0.77714592f, 0.62932044f, 0f, 0f),
            [100] = new(0x44FDD6969F73220CUL, 309.57022f, 1018.3703f, 156.01541f, 0.95881969f, 0.28401542f, 0f, 0f),
            [101] = new(0x49ACB44B63FC7311UL, 1604.8059f, 1651.0632f, 142.49133f, 0.95881969f, 0.28401542f, 0f, 0f),
            [102] = new(0x40C2C3300FA3CE11UL, 1469.5397f, 952.03058f, 320.88492f, 0.77714592f, 0.62932044f, 0f, 0f),
            [103] = new(0x42D5D23670B6A213UL, 1429.2393f, 932.73474f, 320.75f, 0.77714592f, 0.62932044f, 0f, 0f),
            [104] = new(0x4CD3E8AC8E3A1D19UL, 977.99664f, 979.52417f, 117.5779f, 0.77714592f, 0.62932044f, 0f, 0f),
            [105] = new(0x4C4761353A66C91EUL, 961.07019f, 995.41937f, 117.5779f, 0.77714592f, 0.62932044f, 0f, 0f),
            [108] = new(0x47D8CB50B0ABD327UL, 977.73926f, 988.18567f, 117.5779f, 0.77714592f, 0.62932044f, 0f, 0f),
            [109] = new(0x47DD65AA5E349A2FUL, 961.48907f, 981.31134f, 117.5779f, 0.77714592f, 0.62932044f, 0f, 0f),
            [110] = new(0x45C077E7ADF0AA32UL, 1456.0122f, 866.72589f, 321f, 0.77714592f, 0.62932044f, 0f, 0f),
            [111] = new(0x481A48F3A5604637UL, 314.05988f, 1005.9223f, 156.01541f, 0.95881969f, 0.28401542f, 0f, 0f),
            [112] = new(0x4A5703A455E41A3EUL, 328.31448f, 1034.9622f, 156.01541f, 0.95881969f, 0.28401542f, 0f, 0f),
            [113] = new(0x42C3FC42BB2A863EUL, 1592.7782f, 1648.1455f, 142.61633f, 0.95881969f, 0.28401542f, 0f, 0f),
            [114] = new(0x4968FC9730FB7441UL, 1596.8767f, 1620.5706f, 142.49133f, 0.95881969f, 0.28401542f, 0f, 0f),
            [115] = new(0x416189204B241A47UL, 965.92249f, 977.70148f, 117.5779f, 0.77714592f, 0.62932044f, 0f, 0f),
            [118] = new(0x41238B277F178A52UL, 288.75735f, 219.95006f, 358f, 0.77714592f, 0.62932044f, 0f, 0f),
            [119] = new(0x4360ED111F16CA53UL, 1590.141f, 1626.6423f, 157.47589f, 0.95881969f, 0.28401545f, 0f, 0f),
            [120] = new(0x43FC5F7AA3D4A256UL, 294.16235f, 238.37566f, 358f, 0.77714592f, 0.62932044f, 0f, 0f),
            [141] = new(0x40F6306240BC994FUL, 1677.79f, 358.81125f, 205.84091f, 0.99984771f, 0.017452318f, 0f, 0f),
            [144] = new(0x47A0111CB67A5CB7UL, 1684.9493f, 351.02673f, 205.82312f, 0.73727745f, 0.67559016f, 0f, 0f),
            [155] = new(0x416D5049B127E886UL, 1420.3149f, 907.20135f, 320.75f, 0.93358046f, 0.35836786f, 0f, 0f),
            [229] = new(0x4F8FA201E840C2F3UL, 323.41443f, 1023.9645f, 156.17993f, 0.90996128f, -0.41469315f, 0f, 0f),
            [231] = new(0x4159DAEC75E8CBF7UL, 1610.1559f, 1608.0281f, 142.625f, 0.98325491f, -0.18223551f, 0f, 0f),
            [235] = new(0x441178D338B99F7AUL, 293.29242f, 235.81094f, 358f, 0.43051106f, 0.90258533f, 0f, 0f),
            [236] = new(0x44C9F7B2D6C4827FUL, 1609.6293f, 1660.5223f, 142.625f, 0.97236991f, -0.23344538f, 0f, 0f),
            [241] = new(0x446163251AF0C3C3UL, 288.33292f, 225.99171f, 358f, -0.06975656f, 0.99756402f, 0f, 0f),
            [245] = new(0x4FA2069512C0313FUL, 1412.7435f, 916.97729f, 320.93301f, 0.7547096f, 0.65605903f, 0f, 0f),
            [246] = new(0x4B771782D4769C93UL, 1613.2863f, 1639.2313f, 142.75f, 0.99254614f, -0.12186933f, 0f, 0f),
            [249] = new(0x40F4F94DC9ECECFAUL, 1454.9812f, 947.95374f, 320.93301f, 0.7547096f, 0.65605903f, 0f, 0f),
            [251] = new(0x4B2B69BEFF3E1D1FUL, 299.57254f, 263.49414f, 358f, 0.309017f, 0.95105654f, 0f, 0f),
            [252] = new(0x423F0F03DC315158UL, 307.27948f, 216.55583f, 358f, 0.309017f, 0.95105654f, 0f, 0f),
            [253] = new(0x42291FE426579868UL, 1608.8868f, 1614.2769f, 142.5f, 0.99254614f, -0.12186933f, 0f, 0f),
            [258] = new(0x46B5A8B3E40C9F6AUL, 310.56674f, 983.3454f, 156.90172f, -0.70090932f, 0.7132504f, 0f, 0f),
            [259] = new(0x4776C5D66B645C77UL, 993.23724f, 328.94232f, 125.0065f, -0.48480961f, 0.87461972f, 0f, 0f),
            [261] = new(0x4B6869C86C4A9892UL, 322.31824f, 1020.381f, 156.90172f, -0.84804815f, 0.52991927f, 0f, 0f),
            [262] = new(0x4A05C9FB47DDB998UL, 999.89203f, 338.05856f, 125.65823f, 0.25038001f, 0.96814764f, 0f, 0f),
            [263] = new(0x41CE93AD4D5D41B8UL, 326.77081f, 1027.3119f, 156.90172f, 0.66913062f, 0.74314481f, 0f, 0f),
            [264] = new(0x47EABB786A8444C9UL, 1478.5997f, 868.37866f, 321.00586f, 0.18223554f, 0.98325491f, 0f, 0f),
            [266] = new(0x4C1BDFFFD380CAD9UL, 954.79388f, 982.81793f, 118.1035f, 0.66913062f, 0.74314481f, 0f, 0f),
            [267] = new(0x4BD4AF64E11E94DBUL, 984.16833f, 349.45755f, 125.17266f, 0.88294756f, -0.4694716f, 0f, 0f),
            [270] = new(0x4D0B4FA0CA5D0018UL, 325.5751f, 1033.4431f, 156.14058f, 0.98325497f, -0.18223533f, 0f, 0f),
            [271] = new(0x47D9502F7F9C587AUL, 337.43262f, 1024.8939f, 156.10577f, 0.68835455f, 0.7253744f, 0f, 0f),
            [273] = new(0x4CB44F93AA0E9DB2UL, 980.33289f, 994.5874f, 117.63036f, 0.87881714f, 0.47715867f, 0f, 0f),
            [274] = new(0x4BF4ABAF2A2C2ACDUL, 955.97174f, 991.03577f, 117.5779f, -0.60181499f, 0.79863554f, 0f, 0f),
            [275] = new(0x49B6541C6D176CECUL, 306.49023f, 1016.1004f, 156.10577f, -0.3826834f, 0.92387956f, 0f, 0f),
            [276] = new(0x40C5829E6A42EC05UL, 315.88235f, 988.9007f, 156.06735f, 0.26723835f, 0.9636305f, 0f, 0f),
            [277] = new(0x4A195B489C41DC10UL, 340.82047f, 1028.3839f, 156.05493f, 0.89493436f, 0.44619784f, 0f, 0f),
            [278] = new(0x44B8F667EA35891CUL, 976.13452f, 994.91901f, 117.61743f, 0.97437006f, 0.22495103f, 0f, 0f),
            [279] = new(0x4EF8ABB19028762BUL, 981.76849f, 977.12701f, 117.5779f, 0.23344536f, 0.97236991f, 0f, 0f),
            [280] = new(0x4DBA0556AC39E44DUL, 954.23846f, 985.26636f, 117.5779f, -0.49242342f, 0.87035578f, 0f, 0f),
            [319] = new(0x45F7761EA5283E44UL, 984.84723f, 342.23312f, 124.62568f, 0.88294756f, -0.4694716f, 0f, 0f),
            [320] = new(0x4F8F5EA89C69324BUL, 1452.5238f, 868.29285f, 321f, 0.898794f, 0.43837118f, 0f, 0f),
            [321] = new(0x457194D673E01A60UL, 1607.015f, 1639.7474f, 142.68748f, 0.38268346f, 0.92387956f, 0f, 0f),
            [322] = new(0x432F6E4956963B64UL, 304.85022f, 236.17789f, 358f, -0.15643443f, 0.98768836f, 0f, 0f),
            [323] = new(0x4C0AA8DA4691417CUL, 296.48676f, 223.65649f, 358f, -0.043619283f, 0.99904823f, 0f, 0f),
            [324] = new(0x43D0DE06DC8B38E3UL, 299.55167f, 232.84996f, 358f, -0.043619283f, 0.99904823f, 0f, 0f),
            [347] = new(0x422F7B4BDB574AA8UL, 1680.5996f, 351.81644f, 205.375f, 0.99996191f, 0.008726499f, 0f, 0f),
            [351] = new(0x4CE74B9CEDA88A1FUL, 955.31488f, 988.40979f, 117.5779f, -0.70710677f, 0.70710677f, 0f, 0f),
            [354] = new(0x42EEB510D30B79A8UL, 1459.0293f, 853.3783f, 320.75f, 0.77714592f, 0.62932044f, 0f, 0f),
            [355] = new(0x4852D02372C7E1B0UL, 1610.6617f, 1622.0055f, 142.49133f, 0.95881969f, 0.28401542f, 0f, 0f),
            [357] = new(0x43D5A7C3D1ACEDBDUL, 288.23166f, 229.05733f, 358f, 0.77714592f, 0.62932044f, 0f, 0f),
            [359] = new(0x4B7419019F0DA0C2UL, 1598.7501f, 1618.9315f, 157.47589f, 0.95881969f, 0.28401545f, 0f, 0f),
            [360] = new(0x4024A58FBE4B33C3UL, 1473.8583f, 862.5733f, 320.89331f, 0.77714592f, 0.62932044f, 0f, 0f),
            [361] = new(0x4F7573BC50D739C4UL, 1585.0708f, 1622.5005f, 142.74133f, 0.95881969f, 0.28401542f, 0f, 0f),
            [362] = new(0x48FAD258B67E8CC5UL, 333.46387f, 1022.6244f, 156.01541f, 0.95881969f, 0.28401542f, 0f, 0f),
            [363] = new(0x4CFB3DB8AB8D20C9UL, 957.21832f, 989.37006f, 117.57791f, 0.77714592f, 0.62932044f, 0f, 0f),
            [364] = new(0x43DD8BE5B83306D4UL, 308.87207f, 211.52629f, 358f, 0.77714592f, 0.62932044f, 0f, 0f),
            [365] = new(0x48339B74B55A2BD5UL, 1467.1578f, 858.08179f, 320.75f, 0.77714592f, 0.62932044f, 0f, 0f),
            [366] = new(0x4FE2D6424C173BE0UL, 297.15472f, 262.44498f, 358f, 0.77714592f, 0.62932044f, 0f, 0f),
            [367] = new(0x4468E32F413158E0UL, 1606.5968f, 1633.7333f, 142.81593f, 0.95881969f, 0.28401542f, 0f, 0f),
            [368] = new(0x4BBA0737E65725E2UL, 1444.5265f, 879.37213f, 320.97467f, 0.77714592f, 0.62932044f, 0f, 0f),
            [369] = new(0x46D12A8388D14FE4UL, 971.93756f, 981.25055f, 117.5779f, 0.77714592f, 0.62932044f, 0f, 0f),
            [370] = new(0x41C1543CB9EEF3E5UL, 317.28168f, 1022.249f, 156.01541f, 0.77714592f, 0.62932044f, 0f, 0f),
            [371] = new(0x49152703DB8A96ECUL, 302.56375f, 242.05362f, 358f, 0.77714592f, 0.62932044f, 0f, 0f),
            [372] = new(0x4C371244184A59EDUL, 1596.6013f, 1635.2333f, 142.61633f, 0.95881969f, 0.28401542f, 0f, 0f),
            [373] = new(0x4330EA2DD91D1CF0UL, 1481.446f, 937.76672f, 320.75f, 0.77714592f, 0.62932044f, 0f, 0f),
            [375] = new(0x49FFD647FD5FD2FFUL, 319.53168f, 1013.1227f, 156.01541f, 0.95881969f, 0.28401542f, 0f, 0f),
            [376] = new(0x41B9C629103F1D75UL, 1657.5682f, 353.16003f, 205.375f, 0.88701081f, -0.46174866f, 0f, 0f),
            [377] = new(0x42433DEF1848C095UL, 1656.4106f, 349.99167f, 205.375f, -0.71325046f, 0.70090926f, 0f, 0f),
            [393] = new(0x413428CABCA0A1BAUL, 302.84616f, 214.41989f, 358f, 0.99254614f, -0.1218694f, 0f, 0f),
            [400] = new(0x442A9ED628821168UL, 1442.9124f, 882.42322f, 321f, -0.85716724f, 0.51503819f, 0f, 0f),
            [410] = new(0x42C826520125FA35UL, 1677.53f, 352.24316f, 205.375f, 0.95630479f, -0.29237157f, 0f, 0f),
            [415] = new(0x4EF80F039B52F72CUL, 318.36359f, 295.57455f, 358.01675f, 0.99904823f, -0.043619405f, 0f, 0f),
            [416] = new(0x4ACF76B958411836UL, 273.65601f, 205.88736f, 357.78708f, -0.63607818f, 0.77162462f, 0f, 0f),
            [417] = new(0x461F963687CBF43AUL, 293.57297f, 255.80121f, 358.05856f, -0.19936799f, 0.97992468f, 0f, 0f),
            [420] = new(0x492EC9046341D412UL, 310.41791f, 274.0603f, 358f, -0.37460637f, 0.92718393f, 0f, 0f),
            [424] = new(0x495E0759231F0EB3UL, 320.60004f, 294.40836f, 357.78708f, 0.99965733f, -0.026176997f, 0f, 0f),
            [426] = new(0x4AAD7D000BF4799EUL, 309.17929f, 186.41556f, 358.0177f, 0.14780964f, 0.98901582f, 0f, 0f),
            [427] = new(0x48FC08EC44A0E894UL, 312.84573f, 186.45784f, 357.78708f, 0.99965733f, -0.026176997f, 0f, 0f),
            [430] = new(0x4C00F2C534075073UL, 276.13388f, 204.20413f, 357.78708f, -0.61566156f, 0.78801072f, 0f, 0f),
        };

        private static readonly uint[] _spawnOrder = Level002SpawnPoints.Keys.OrderBy(k => k).ToArray();
        private int _spawnIndex = 0;

        private readonly ClientManager _clientManager;
        private readonly Dictionary<uint, MonsterAI> _monsters = new();
        private readonly object _lock = new();

        public MonsterAIManager(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }


        public uint NextNetId() => Interlocked.Increment(ref _nextNetId);

        /// <summary>Returns the next available spawn point (cycles through all 162 level_002 entries).</summary>
        public (uint entityId, SpawnPoint point) TakeSpawnPoint()
        {
            int idx = Interlocked.Increment(ref _spawnIndex) - 1;
            uint id = _spawnOrder[idx % _spawnOrder.Length];
            return (id, Level002SpawnPoints[id]);
        }

        /// <summary>Registers a monster and starts its AI loop.</summary>
        public MonsterAI Spawn(uint netId, int monsterInfoId, CSVec3 position)
        {
            var monster = new MonsterAI(netId, monsterInfoId, position, this);
            lock (_lock)
            {
                _monsters[netId] = monster;
            }
            Logger.Info($"MonsterAI spawned: netId={netId} infoId={monsterInfoId} pos=({position.x:F1},{position.y:F1},{position.z:F1})");
            return monster;
        }

        /// <summary>Stops the AI loop and removes the monster.</summary>
        public void Despawn(uint netId)
        {
            MonsterAI monster;
            lock (_lock)
            {
                if (!_monsters.TryGetValue(netId, out monster))
                    return;
                _monsters.Remove(netId);
            }
            monster.Dispose();
            Logger.Info($"MonsterAI despawned: netId={netId}");
        }

        /// <summary>Returns the nearest connected client with a known position, and the distance to it.</summary>
        public (Client client, float dist) FindNearestPlayer(CSVec3 from)
        {
            Client nearest = null;
            float minDist = float.MaxValue;

            foreach (Client c in _clientManager.GetAll())
            {
                if (c.State?.Position == null) continue;
                float dist = Distance(from, c.State.Position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = c;
                }
            }

            return (nearest, minDist);
        }

        /// <summary>Sends a MonsterLocomotion packet to every connected client.</summary>
        public void BroadcastLcm(CSMonsterLocomotion lcm)
        {
            var packet = NewCsPacket.MonsterLCM(lcm);
            foreach (Client c in _clientManager.GetAll())
            {
                try { c.SendCsPacket(packet); }
                catch (Exception ex) { Logger.Error($"BroadcastLcm to {c.Identity}: {ex.Message}"); }
            }
        }

        private static float Distance(CSVec3 a, CSVec3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>Maps MonsterInfoId to its CryEngine behavior tree path.</summary>
        public string GetBTState(int monsterInfoId)
        {
            // TODO: build a full mapping from CSV static data
            return monsterInfoId switch
            {
                39002 => @"Em001\em001.xml",
                39003 => @"Em002\em002.xml",
                60030 => @"Em003\em003.xml",
                _     => string.Empty,
            };
        }
    }
}
