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
            return text.Substring(text.LastIndexOf('.') + 1, text.Length - text.LastIndexOf('.') - 1);
        }
        public static bool isSameAs(string v1, string v2)
        {
            string[] field = new string[4];
            field[0] = v1.Substring(0, v1.LastIndexOf('.'));
            field[1] = getField(v1);

            field[2] = v2.Substring(0, v2.LastIndexOf('.'));
            field[3] = getField(v2);

            return field[0] == field[2] && field[1] == field[3];
        }
        public static string parseGet(string text, float scale)
        {
            loadDict();
            string[] field = new string[2];
            field = Compiler.smartSplitJson(text, '.', 1);
            return parseGet(field[0], field[1], scale);
        }

        public static string parseGet(string entity, string value, float scale)
        {
            loadDict();
            string[] field = new string[2];
            field[0] = limitedEntity(entity);
            field[1] = value;

            if (nbt_map.ContainsKey(field[1]))
                return "data get entity " + limitedEntity(Compiler.smartEmpty(field[0])) + " " + nbt_map[field[1]] + " " + scale.ToString();
            else
                return getEntityVar(field[0], field[1]);
        }
        public static string getType(string nbt)
        {
            loadDict();
            if (nbt_map_type.ContainsKey(nbt))
            {
                if (nbt_map_type[nbt] == "double")
                    return "float";
                if (nbt_map_type[nbt] == "short")
                    return "int";
                if (nbt_map_type[nbt] == "long")
                    return "int";
                return nbt_map_type[nbt];
            }
            else
            {
                var var = Compiler.GetVariable(Compiler.context.GetVariable(nbt));
                return var.type.ToString().ToLower();
            }
        }
        public static string parseSet(string entity, string value, float scale)
        {
            loadDict();
            if (entity.Contains("@"))
            {
                entity = limitedEntity(entity);

                if (nbt_map.ContainsKey(value))
                    return "execute store result entity " + entity + " " + nbt_map[value] + " " + nbt_map_type[value] + " " + scale.ToString() + " run ";
                else
                    return setEntityVar(entity, value);
            }
            else
            {
                if (nbt_map.ContainsKey(value))
                    return "execute store result entity @e[tag=" + entity + ", limit=1] " + nbt_map[value] + " " + nbt_map_type[value] + " " + scale.ToString() + " run ";
                else
                    return setEntityVar(entity, value);
            }
        }
        private static string setEntityVar(string entity, string value)
        {
            if (Compiler.context.GetVariable(value, true) != null)
            {
                var var = Compiler.GetVariable(Compiler.context.GetVariable(value));
                if (var.entity)
                    return "execute store result score " + var.scoreboard().Replace("@s", entity) + " run ";
                else
                    throw new Exception(entity + " isn't an entity variable");
            }
            else
            {
                throw new Exception("Unknown " + value);
            }
        }
        private static string getEntityVar(string entity, string value)
        {
            if (Compiler.context.GetVariable(value, true) != null)
            {
                var var = Compiler.GetVariable(Compiler.context.GetVariable(value));
                if (var.entity)
                    return "scoreboard players get " + var.scoreboard().Replace("@s", entity);
                else
                    throw new Exception(entity + " isn't an entity variable");
            }
            else
            {
                throw new Exception("Unknown " + value);
            }
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
