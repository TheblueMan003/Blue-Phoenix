using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluePhoenix
{
    public static class PathConverter
    {
        private static Dictionary<string, string> Cache = new Dictionary<string, string>();

        public static string Convert(string path)
        {
            if (!Cache.ContainsKey(path))
            {
                Cache[path] = path.Replace("\\","/").Replace("//","/");
            }
            return Cache[path];
        }

        public static string Local(string path, string parent)
        {
            return Convert(path).Replace(Convert(parent+"/"), "");
        }
    }
}
