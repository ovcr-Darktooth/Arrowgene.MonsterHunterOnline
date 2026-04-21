namespace Arrowgene.MonsterHunterOnline.Service.Data
{
    public static class Crc32
    {
        private static readonly uint[] Table = BuildTable();

        private static uint[] BuildTable()
        {
            const uint poly = 0xEDB88320u;
            var table = new uint[256];
            for (uint i = 0; i < 256; i++)
            {
                uint c = i;
                for (int k = 0; k < 8; k++)
                {
                    c = (c & 1) != 0 ? (poly ^ (c >> 1)) : (c >> 1);
                }
                table[i] = c;
            }
            return table;
        }

        public static uint Compute(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            uint crc = 0xFFFFFFFFu;
            for (int i = 0; i < value.Length; i++)
            {
                byte b = (byte)value[i];
                crc = Table[(crc ^ b) & 0xFF] ^ (crc >> 8);
            }
            return ~crc;
        }

        public static uint ComputeLowercase(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            uint crc = 0xFFFFFFFFu;
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if (ch >= 'A' && ch <= 'Z') ch = (char)(ch + 32);
                byte b = (byte)ch;
                crc = Table[(crc ^ b) & 0xFF] ^ (crc >> 8);
            }
            return ~crc;
        }
    }
}
