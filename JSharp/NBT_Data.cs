using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    public static class NBT_Data
    {
        private static Dictionary<string, string> nbt_map = new Dictionary<string, string>();
        private static Dictionary<string, string> nbt_map_type = new Dictionary<string, string>();
        private static bool dicLoaded = false;

        private static void loadDict()
        {
            if (!dicLoaded)
            {
                dicLoaded = true;
                foreach (string l in File.ReadAllLines("nbt_map.txt"))
                {
                    if (l.Contains("="))
                    {
                        string[] c = l.Replace(" ", "").Split('=');
                        nbt_map.Add(c[0], c[1]);
                        nbt_map_type.Add(c[0], c[2]);
                    }
                }
            }
        }

        public static string getField(string text)
        {
            return map(text.Substring(text.LastIndexOf('.') + 1, text.Length - text.LastIndexOf('.') - 1));
        }
        public static string map(string text)
        {
            loadDict();
            return nbt_map[text];
        }
        public static string parseGet(string text, float scale)
        {
            loadDict();
            string[] field = new string[2];
            field[0] = text.Substring(0, text.LastIndexOf('.'));
            field[1] = getField(text);
            
            return "data get entity " + limitedEntity(Compiler.smartEmpty(field[0])) + " " + field[1]+" "+scale.ToString();
        }

        public static string parseGet(string entity, string value, float scale)
        {
            loadDict();
            string[] field = new string[2];
            field[0] = limitedEntity(entity);
            field[1] = value;
            
            return "data get entity " + limitedEntity(Compiler.smartEmpty(field[0])) + " " + nbt_map[field[1]] + " " + scale.ToString();
        }

        public static string parseSet(string entity, string value, float scale)
        {
            loadDict();
            if (entity.Contains("@"))
            {
                entity = limitedEntity(entity);
                return "execute store result entity " + entity + " " + nbt_map[value] + " " + nbt_map_type[value] + " " + scale.ToString() + " run ";
            }
            else
                return "execute store result entity @e[tag=" + entity + ", limit=1] " + nbt_map[value] + " " + nbt_map_type[value] + " " + scale.ToString() + " run ";
        }

        public static string limitedEntity(string entity)
        {
            if (!entity.Contains("limit=1") && !entity.Contains("@s") && !entity.Contains("@p") && !entity.Contains("@r"))
                if (entity.Contains("]"))
                    entity = entity.Substring(0, entity.LastIndexOf("]")) + ",limit=1]";
                else
                    entity = entity + "[limit=1]";
            return entity;
        }
    }
}
