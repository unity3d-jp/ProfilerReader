using System.Text;


namespace UTJ.ProfilerReader.Analyzer
{
    public class CsvStringGenerator
    {
        private StringBuilder stringBuilder;
        public CsvStringGenerator()
        {
            stringBuilder = new StringBuilder(1024 * 1024);
        }
        public CsvStringGenerator(StringBuilder sb)
        {
            stringBuilder = sb;
        }
        public CsvStringGenerator AppendColumn(string val)
        {
            if( val == null) { val = ""; }
            if (val.Contains(",")) {
                val = val.Replace(',', '.');
            }
            if (val.Contains("\n"))
            {
                val = val.Replace('\n', ' ');
            }
            stringBuilder.Append(val).Append(',');
            return this;
        }
        public CsvStringGenerator AppendColumn(string val,int idx,int length)
        {
            if (val == null)
            {
                stringBuilder.Append(",");
                return this;
            }
            if (val.Contains(",") )
            {
                val = val.Replace(',', '.');
            }
            if (val.Contains("\n"))
            {
                val = val.Replace('\n', ' ');
            }
            else
            {
                stringBuilder.Append(val, idx, length).Append(',');
            }
            return this;
        }
        public CsvStringGenerator AppendColumn(int val)
        {
            stringBuilder.Append(val).Append(',');
            return this;
        }
        public CsvStringGenerator AppendColumn(bool val)
        {
            stringBuilder.Append(val).Append(',');
            return this;
        }
        public CsvStringGenerator AppendColumn(float val)
        {
            stringBuilder.Append(val).Append(',');
            return this;
        }
        public CsvStringGenerator AppendColumn(ulong val)
        {
            stringBuilder.Append(val).Append(',');
            return this;
        }
        public CsvStringGenerator AppendColumnAsAddr(ulong val)
        {
            stringBuilder.Append("0x");
            AppendAddrStr(stringBuilder, val,16).Append(',');
            return this;
        }
        public CsvStringGenerator NextRow()
        {
            stringBuilder.Append("\n");
            return this;
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }

        private static char[] addrChars = new char[] {
            '0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'
        };

        // AppendFormat("0x{0,0:X16}", val) <- allocate a lot of managed memory...
        public static StringBuilder AppendAddrStr(StringBuilder sb,ulong val,int num) {
            for (int i = num - 1; i >= 0; --i )
            {
                ulong masked = (val >> (i*4) )& 0xf;
                sb.Append(addrChars[masked]);
            }
            return sb;
        }
    }
}