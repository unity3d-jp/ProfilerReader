
namespace UTJ.ProfilerReader.RawData.Debug
{

    public class DebugLogWrite
    {
        static System.Text.StringBuilder sb = new System.Text.StringBuilder(1024 * 1024);
        static string filename = "";

        public static void SetFile(string file)
        {
#if UTJ_CHECK
            if ( !string.IsNullOrEmpty(file) && System.IO.File.Exists(file) )
            {
                System.IO.File.Delete(file);
            }
#endif
            filename = file;
        }
        public static void Log(string str)
        {
#if UTJ_CHECK
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }
            sb.Append(str).Append("\n");
#endif
            if( sb.Length > 512 * 1024)
            {
                Flush();
            }
        }
        public static void Flush()
        {
#if UTJ_CHECK
            if (string.IsNullOrEmpty(filename))
            {
                sb.Length = 0;
                return;
            }
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.AppendAllText(filename, sb.ToString());
            }
            else
            {
                System.IO.File.WriteAllText(filename, sb.ToString());
            }
            sb.Length = 0;
#endif
        }
    }
}