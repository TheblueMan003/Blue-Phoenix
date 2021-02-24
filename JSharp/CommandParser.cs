using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    public static class CommandParser
    {
        public static string[] funcName = new string[] { "tellraw", "title", "say", "clear", "effect", "difficulty", "gamemode", "gamerule",
            "kill", "spawnpoint", "setblock", "fill", "stopsound", "weather", "tp", "execute", "tag", "untag", "unitag", "structure","magictitle"};
        public static string[] difficulties;
        public static string[] effects;
        public static string[] gamemodes;
        public static string[] names;
        public static string[] scoreboards;
        public static string[] sounds;
        public static string[] cmds;
        public static string[] grp2;
        public static List<string> gamerules = new List<string>();
        public static List<Gamerule> gamerulesObj = new List<Gamerule>();
        private static bool loaded;
        
        public static void loadDict()
        {
            if (!loaded)
            {
                loaded = true;
                difficulties = File.ReadAllLines("cmd_data/difficulty.txt");
                effects = File.ReadAllLines("cmd_data/effect.txt");
                gamemodes = File.ReadAllLines("cmd_data/gamemode.txt");
                names = File.ReadAllLines("cmd_data/names.txt");
                scoreboards = File.ReadAllLines("cmd_data/scoreboard.txt");
                sounds = File.ReadAllLines("cmd_data/sounds.txt");
                cmds = File.ReadAllLines("cmd_data/colors/grp_1.txt");
                grp2 = File.ReadAllLines("cmd_data/colors/grp_2.txt");
                foreach (string gr in File.ReadAllLines("cmd_data/gamerule.txt"))
                {
                    string[] c = gr.Split('	');
                    if (c.Length > 3 && c[4] == "Yes")
                    {
                        gamerulesObj.Add(new Gamerule(c[0], c[2], c[3], c[1]));
                        gamerules.Add(c[0]);
                    }
                }
            }
        }
        public static bool canBeParse(string text)
        {
            while(text.StartsWith(" "))text = text.Substring(1, text.Length - 1);
            foreach (string fun in funcName)
            {
                if (text.StartsWith(fun+"("))
                {
                    return true;
                }
            }
            return false;
        }
        public static string parse(string text, Compiler.Context context)
        {
            while (text.StartsWith(" ")) text = text.Substring(1, text.Length - 1);
            string arg = text.Substring(text.IndexOf('(') + 1, text.LastIndexOf(')') - text.IndexOf('(') - 1);
            string[] args = Compiler.smartSplit(arg, ',');

            if (text.ToLower().StartsWith("structure"))
                return parseStructure(args, context, text);

            if (text.ToLower().StartsWith("tag"))
                return parseTag(args, context, text);

            if (text.ToLower().StartsWith("unitag"))
                return parseUniTag(args, context, text);

            if (text.ToLower().StartsWith("untag"))
                return parseUntag(args, context, text);

            if (text.ToLower().StartsWith("title"))
                return parseTitle(args, context, text);

            if (text.ToLower().StartsWith("magictitle"))
                return parseMagicTitle(args, context, text);

            if (text.ToLower().StartsWith("tellraw"))
                return parseTellraw(args, context, text);

            if (text.ToLower().StartsWith("clear"))
                return parseClear(args, context, text);

            if (text.ToLower().StartsWith("say"))
                return parseSay(args, context, text);

            if (text.ToLower().StartsWith("effect"))
                return parseEffect(args, context, text);

            if (text.ToLower().StartsWith("difficulty"))
                return parseDifficulty(args, context, text);

            if (text.ToLower().StartsWith("gamemode"))
                return parseGamemode(args, context, text);

            if (text.ToLower().StartsWith("gamerule"))
                return parseGamerule(args, context, text);

            if (text.ToLower().StartsWith("kill"))
                return parseKill(args, context, text);

            if (text.ToLower().StartsWith("spawnpoint"))
                return parseSpawnpoint(args, context, text);

            if (text.ToLower().StartsWith("fill"))
                return parseFill(args, context, text);

            if (text.ToLower().StartsWith("setblock"))
                return parseSetblock(args, context, text);

            if (text.ToLower().StartsWith("stopsound"))
                return parseStopsound(args, context, text);

            if (text.ToLower().StartsWith("weather"))
                return parseWeather(args, context, text);

            if (text.ToLower().StartsWith("tp"))
                return parseTP(args, context, text);

            throw new NotImplementedException(text.Split(' ')[0] + " is not implemented");
        }
        public static string parseTag(string[] args, Compiler.Context context, string text)
        {
            if (args.Length > 0)
            {
                string tmp = "";
                string output = "";
                if (args.Length == 1)
                {
                    output = "tag @s add " + args[0];
                }
                else
                {
                    tmp += "tag " + context.GetEntitySelector(args[0]) + " add ";
                }
                for (int i = 1; i < args.Length; i++)
                {
                    output += tmp + Compiler.smartEmpty(args[i]) + '\n';
                }

                return output;
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseUniTag(string[] args, Compiler.Context context, string text)
        {
            if (args.Length > 0)
            {
                string tmp1 = "tag @e remove ";
                string tmp2 = "";
                string output = "";
                if (args.Length == 1)
                {
                    output = "tag @e remove " + args[0] + "\ntag @s add " + args[0];
                }
                else
                {
                    tmp2 += "tag " + context.GetEntitySelector(args[0]) + " add ";
                }

                for (int i = 1; i < args.Length; i++)
                {
                    output += tmp1 + Compiler.smartEmpty(args[i]) + '\n';
                }
                for (int i = 1; i < args.Length; i++)
                {
                    output += tmp2 + Compiler.smartEmpty(args[i]) + '\n';
                }

                return output;
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseUntag(string[] args, Compiler.Context context, string text)
        {
            if (args.Length > 0)
            {
                string tmp = "";
                string output = "";
                if (args.Length == 1)
                {
                    output = "tag @s remove " + args[0];
                }
                else
                {
                    tmp += "tag " + context.GetEntitySelector(args[0]) + " remove ";
                }
                for (int i = 1; i < args.Length; i++)
                {
                    output += tmp + Compiler.smartEmpty(args[i]) + '\n';
                }

                return output;
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseTellraw(string[] args, Compiler.Context context, string text)
        {
            string output = "tellraw " + context.GetEntitySelector(args[0]) + " ";
            string[] json = jsonFormat(args, context, 1);
            output = json[1]+ output+ json[0]+"\n"+ json[2];
 
            return output + '\n';
        }
        public static string parseTitle(string[] args, Compiler.Context context, string text)
        {
            string titleType = Compiler.smartEmpty(args[1]);
            if (titleType != "actionbar" && titleType != "title" && titleType != "subtitle")
                throw new Exception("Invalid Title type: " + titleType);
            
            string output = "title " + context.GetEntitySelector(args[0]) + " " + titleType + " ";
            string[] json = jsonFormat(args, context, 2);
            output = json[1] + output + json[0] + "\n" + json[2];
            return output + '\n';
        }
        public static string parseMagicTitle(string[] args, Compiler.Context context, string text)
        {
            int maxTime;
            int argIndex = 1;
            if (!int.TryParse(Compiler.smartEmpty(args[1]), out maxTime))
            {
                maxTime = -1;
                argIndex = 0;
            }

            string titleType = Compiler.smartEmpty(args[argIndex+2]);
            if (titleType != "actionbar" && titleType != "title" && titleType != "subtitle")
                throw new Exception("Invalid Title type: " + titleType);

            string titleLine = "title " + context.GetEntitySelector(args[argIndex+1]) + " " + titleType + " ";
            string output = "";
            int time = 0;

            for (int i = argIndex+3; i < args.Length; i++)
            {
                string arg = Compiler.smartEmpty(args[i]).StartsWith("(") ? Compiler.smartEmpty(args[i].Substring(args[i].IndexOf('(') + 1, args[i].LastIndexOf(')') - args[i].IndexOf('(') - 1)) : args[i];
                string[] subargs = Compiler.smartSplit(arg, ',');

                if (subargs[0].StartsWith("\""))
                {
                    for (int j = 1; j < subargs[0].Length - 1; j++)
                    {
                        if (subargs[0][j] == '\\'){
                            if (subargs[0][j+1] == 'u')
                            {
                                while(Char.IsHighSurrogate((char)Convert.ToInt32(subargs[0].Substring(j, 6).ToUpper().Replace("\\U", "0x"), 16)))
                                {
                                    j += 6;
                                }
                                j += 5;
                            }
                            else
                            {
                                j++;
                            }
                        }
                        
                        string[] json = new string[i - 2 - argIndex];
                        for (int k = argIndex+3; k < i; k++)
                        {
                            json[k - 3 - argIndex] = args[k];
                        }
                        json[i - 3 - argIndex] = "(\"" + subargs[0].Substring(1, j) + "\"";
                        for (int k = 1; k < subargs.Length; k++)
                        {
                            json[i - 3 - argIndex] += "," + subargs[k];
                        }
                        json[i - 3 - argIndex] += ")";
                        string[] jsonParsed = jsonFormat(json, context, 0);
                        output += "execute if score " + context.GetVariableName(args[0]) + " matches " + time.ToString() + " run " + titleLine + jsonParsed[0] + "\n";
                        time++;
                    }
                }
                else
                {
                    string[] json = new string[i - 2];
                    for (int k = argIndex+3; k <= i; k++)
                    {
                        json[k - 3 - argIndex] = args[k];
                    }
                    
                    string[] jsonParsed = jsonFormat(json, context, 0);
                    output += "execute if score " + context.GetVariableName(args[0]) + " matches " + time.ToString() + " run " + titleLine + jsonParsed[0] + "\n";
                    time++;
                }
            }
            string[] jsonParsedGlobal = jsonFormat(args, context, 3 +argIndex);
            output += "execute if score " + context.GetVariableName(args[0]) + " matches " + time.ToString() + ".. run " +titleLine + jsonParsedGlobal[0] + "\n";
            if (maxTime > -1)
            {
                output += "execute if score " + context.GetVariableName(args[0]) + " matches " + (time+ maxTime).ToString() + ".. run scoreboard players set " + context.GetVariableName(args[0]) + " -100000\n";
            }
            return jsonParsedGlobal[1]+output+ jsonParsedGlobal[2] + '\n';
        }
        public static string parseSay(string[] args, Compiler.Context context, string text)
        {
            if (args.Length == 1) {
                string output = "say ";

                for (int i = 0; i < args.Length; i++)
                {
                    output += args[i] + " ";
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseClear(string[] args, Compiler.Context context, string text)
        {
            if (args.Length > 0 && args.Length < 3) {
                string output = "clear ";
                if (context.isEntity(args[0]))
                    output += context.GetEntitySelector(args[0]);
                else
                    output += "@s " + args[0];

                for (int i = 1; i < Math.Min(2, args.Length); i++)
                {
                    output += " " + Compiler.smartEmpty(args[i]);
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseEffect(string[] args, Compiler.Context context, string text)
        {
            if (args.Length < 7)
            {
                string output;
                if (args[0] != "clear")
                {
                    if (context.isEntity(args[1]))
                        output = "effect give " + context.GetEntitySelector(args[1]) + " " + Compiler.smartEmpty(args[2]) + " " + Compiler.smartEmpty(args[3]) + " " + Compiler.smartEmpty(args[4]) + " " + Compiler.smartEmpty(args[5]);
                    else
                        output = "effect give @s " + Compiler.smartEmpty(args[1]) + " " + Compiler.smartEmpty(args[2]) + " " + Compiler.smartEmpty(args[3]) + " " + Compiler.smartEmpty(args[4]);
                }
                else
                {
                    if (context.isEntity(args[1]))
                        output = "effect clear " + context.GetEntitySelector(args[1]) + " " + Compiler.smartEmpty(args[2]);
                    else
                        output = "effect clear @s " + Compiler.smartEmpty(args[1]);
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseDifficulty(string[] args, Compiler.Context context, string text)
        {
            if (args.Length == 1)
            {
                int tmpI;
                string output;
                if (int.TryParse(args[0], out tmpI))
                {
                    output = "difficulty " + difficulties[tmpI];
                }
                else
                {
                    output = "difficulty " + Compiler.smartEmpty(args[0]);
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseGamemode(string[] args, Compiler.Context context, string text)
        {
            if (args.Length == 2 && context.isEntity(args[0]))
            {
                int tmpI;
                string output;
                if (int.TryParse(args[1], out tmpI))
                {
                    output = "gamemode " + difficulties[tmpI] + " " + context.GetEntitySelector(args[0]);
                }
                else
                {
                    output = "gamemode " + Compiler.smartEmpty(args[1]) + " " + context.GetEntitySelector(args[0]);
                }

                return output + '\n';
            }
            else if (args.Length == 2 && context.isEntity(args[1]))
            {
                int tmpI;
                string output;
                if (int.TryParse(args[0], out tmpI))
                {
                    output = "gamemode " + difficulties[tmpI] + " " + context.GetEntitySelector(args[1]);
                }
                else
                {
                    output = "gamemode " + Compiler.smartEmpty(args[0]) + " " + context.GetEntitySelector(args[1]);
                }

                return output + '\n';
            }
            else if (args.Length == 1)
            {
                int tmpI;
                string output;
                if (int.TryParse(args[0], out tmpI))
                {
                    output = "gamemode " + difficulties[tmpI] + " @s";
                }
                else
                {
                    output = "gamemode " + Compiler.smartEmpty(args[0]) + " @s";
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseGamerule(string[] args, Compiler.Context context, string text)
        {
            if (args.Length == 2) {
                string output = "gamerule " + Compiler.smartEmpty(args[0]) + " " + Compiler.smartEmpty(args[1]);

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
}
        public static string parseKill(string[] args, Compiler.Context context, string text)
        {
            if (args.Length == 0)
            {
                return "kill @s\n";
            }
            else if (context.isEntity(args[0]))
            {
                string output = "kill " + context.GetEntitySelector(args[0]);

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseSpawnpoint(string[] args, Compiler.Context context, string text)
        {
            if (args.Length >= 0)
            {
                string output = "spawnpoint ";
                if (context.isEntity(args[0]))
                    output += context.GetEntitySelector(args[0]);
                else
                    output += "@s " + args[0];

                for (int i = 1; i < args.Length; i++)
                {
                    output += " " + args[i];
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseSetblock(string[] args, Compiler.Context context, string text)
        {
            if (args.Length == 1)
            {
                string output = "setblock";

                if (args.Length == 1)
                {
                    if (args[0].Contains("~") || (args[0].Contains(" ") || int.TryParse(args[0].Split(' ')[0], out int t)))
                    {
                        output += " " + args[0];
                    }
                    else
                    {
                        output += " ~ ~ ~ " + args[0];
                    }
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        output += " " + args[i];
                    }
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseFill(string[] args, Compiler.Context context, string text)
        {
            if (args.Length == 1)
            {
                string output = "fill";

                for (int i = 0; i < args.Length; i++)
                {
                    output += " " + args[i].Replace("replace #", "replace #" + Compiler.Project.ToLower() + ":");
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseStopsound(string[] args, Compiler.Context context, string text)
        {
            if (args.Length > 0)
            {
                string output = "stopsound ";
                if (context.isEntity(args[0]))
                    output += context.GetEntitySelector(args[0]);
                else
                    output += "@s " + args[0];

                for (int i = 1; i < args.Length; i++)
                {
                    output += " " + Compiler.smartEmpty(args[i]);
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseWeather(string[] args, Compiler.Context context, string text)
        {
            if (args.Length == 1)
            {
                string output = "weather";

                for (int i = 0; i < args.Length; i++)
                {
                    output += " " + Compiler.smartEmpty(args[i]);
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseStructure(string[] args, Compiler.Context context, string text)
        {
            if (args.Length == 1)
            {
                string output = "setblock ~ ~ ~ minecraft:structure_block[mode = load]{ignoreEntities: 0b,rotation: \"NONE\",mode: \"LOAD\",integrity: 1.0f,name: \"" + Compiler.Project.ToLower() + ":" + args[0].Replace("\"", "") + "\",showboundingbox: 1b}";
                output += "\nsetblock ~ ~1 ~ redstone_block";
                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
        }
        public static string parseTP(string[] args, Compiler.Context context, string text)
        {
            if (args.Length == 1 || (args.Length == 2 && context.isEntity(args[0])))
            {
                string output = "tp";

                if (args.Length == 1)
                {
                    output += "@s " + args[0];
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        output += " " + args[i];
                    }
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text);
            }
}
        private static string getParameter(string subargs)
        {
            string link = subargs;
            link = link.Substring(link.IndexOf("=")+1, link.Length - link.IndexOf("=")-1);
            while (link.StartsWith(" "))
            {
                link = link.Substring(1, link.Length - 1);
            }
            if (link.StartsWith("\""))
                link = link.Substring(1, link.Length - 1);
            if (link.EndsWith("\""))
                link = link.Substring(0, link.Length - 1);

            return link;
        }

        public static string[] jsonFormat(string[] args, Compiler.Context context, int start = 0)
        {
            string output = "[";
            string output2 = "";
            string output3 = "";
            int strTag = 0;

            for (int i = start; i < args.Length; i++)
            {
                string arg = Compiler.smartEmpty(args[i]).StartsWith("(") ? Compiler.smartEmpty(args[i].Substring(args[i].IndexOf('(') + 1, args[i].LastIndexOf(')') - args[i].IndexOf('(') - 1)) : args[i];
                string[] subargs = Compiler.smartSplit(arg, ',');
                bool array = false;
                if (subargs[0].Contains("\""))
                {
                    output += ",{\"text\":" + subargs[0];
                }
                else if (subargs[0].Contains("@"))
                {
                    output += ",{\"selector\":\"" + subargs[0] + "\"";
                }
                else if (Compiler.isStringVar(subargs[0]))
                {
                    string tmp = Compiler.getString(subargs[0]);
                    tmp += "tag @e[tag=__str__,tag=!__str__tag__] add __str_" + strTag.ToString() + "\n";
                    tmp += "tag @e[tag=__str__,tag=!__str__tag__] add __str__tag__" + "\n";
                    output += ",{\"selector\":\"" + "@e[tag=__str_" + strTag.ToString() + "]" + "\"";
                    strTag++;
                    output2 += tmp;
                }
                else if (context.GetVarType(subargs[0]) == Compiler.Type.ENTITY)
                {
                    output += ",{\"selector\":\"" + context.GetEntitySelector(subargs[0]) + "\"";
                }
                else if (context.GetVarType(subargs[0]) == Compiler.Type.ARRAY)
                {
                    int nb = Compiler.GetVariable(context.GetVariable(subargs[0])).arraySize;
                    array = true;
                    for (int j = 0; j < Compiler.GetVariable(context.GetVariable(subargs[0])).arraySize; j++)
                    {
                        var va = Compiler.GetVariable(context.GetVariable(subargs[0] + "." + j.ToString()));
                        if (va.type == Compiler.Type.STRING)
                        {
                            string tmp = Compiler.getString(subargs[0]);
                            tmp += "tag @e[tag=__str__,tag=!__str__tag__] add __str_" + strTag.ToString() + "\n";
                            tmp += "tag @e[tag=__str__,tag=!__str__tag__] add __str__tag__" +  "\n";
                            output += ",{\"selector\":\"" + "@e[tag=__str_" + strTag.ToString() + "]" + "\"";
                            output += jsonSubArg(subargs, context);
                            output += "}";
                            strTag++;
                            output2 += tmp;
                        }
                        else
                        {
                            if (j == 0)
                            {
                                output += ",{\"text\":" + "\"[\"";
                                output += jsonSubArg(subargs, context);
                                output += "}";
                            }
                            string[] v = context.GetVariableName(subargs[0]+"."+j.ToString()).Split(' ');
                            output += ",{ \"score\":{ \"name\":\"" + v[0] + "\",\"objective\":\"" + v[1] + "\"}";
                            output += jsonSubArg(subargs, context);
                            output += "}";
                            output += ",{\"text\":" + "\", \"";
                            output += jsonSubArg(subargs, context);
                            output += "}";
                            if (j == nb - 1)
                            {
                                output += ",{\"text\":" + "\"]\"";
                                output += jsonSubArg(subargs, context);
                            }
                        }
                    }
                }
                else if (context.GetVarType(subargs[0]) == Compiler.Type.STRUCT)
                {
                    Compiler.Structure s = Compiler.structs[Compiler.GetVariableByName(subargs[0]).enums];
                    output += ",{\"text\":\"" + s.name+"(" + "\"";
                    output += jsonSubArg(subargs, context);
                    output += "}";
                    foreach (var v in s.fields)
                    {
                        output += ",{\"text\":\"" + v.name + "=" + "\"";
                        output += jsonSubArg(subargs, context);
                        output += "}";

                        string tmp = jsonFormat(new string[] { v.gameName }, context, 0)[0];
                        output += "," + tmp.Substring(1, tmp.Length - 2);
                    }
                    output += ",{\"text\":\"" + ")" + "\"";
                }
                else
                {
                    string[] v = context.GetVariableName(subargs[0]).Split(' ');
                    output += ",{ \"score\":{ \"name\":\"" + v[0] + "\",\"objective\":\"" + v[1] + "\"}";
                }
                if (!array)
                    output += jsonSubArg(subargs, context);

                output += "}";
            }

            output += "]";

            if (strTag > 0)
            {
                output3 += "\nkill @e[tag=__str__]";
            }
            output = "["+output.Substring(2, output.Length - 2);
            return new string[] { output, output2, output3};
        }
        public static string jsonSubArg(string[] subargs, Compiler.Context context)
        {
            string output = "";
            for (int j = 1; j < subargs.Length; j++)
            {
                if (context.IsFunction(subargs[j]))
                {
                    output += ",\"clickEvent\":{ \"action\":\"run_command\",\"value\":\"function " +
                        Compiler.GetFunction(context.GetFunctionName(subargs[j]), new string[] { }).gameName + "\"}";
                }
                else if (subargs[j].StartsWith("\"http"))
                {
                    output += ",\"clickEvent\":{ \"action\":\"open_url\",\"value\":" + subargs[j] + "}";
                }
                else if (subargs[j].StartsWith("action="))
                {
                    output += ",\"clickEvent\":{ \"action\":\"run_command\",\"value\":\"/function " +
                        Compiler.GetFunction(context.GetFunctionName(subargs[j].Split('=')[1]), new string[] { }).gameName + "\"}";
                }
                else if (subargs[j].StartsWith("link"))
                {
                    output += ",\"clickEvent\":{ \"action\":\"open_url\",\"value\":" + Compiler.smartSplit(subargs[j], '=', 1)[1] + "}";
                }
                else if (subargs[j] == "bold")
                    output += ",\"bold\":true";

                else if (subargs[j] == "italic")
                    output += ",\"italic\":true";

                else if (subargs[j] == "strikethrough")
                    output += ",\"strikethrough\":true";

                else if (subargs[j] == "obfuscated")
                    output += ",\"obfuscated\":true";

                else if (subargs[j] == "underlined")
                    output += ",\"underlined\":true";

                else
                    output += ",\"color\":\"" + subargs[j] + "\"";
            }
            return output;
        }

        public static bool isValidSelector(string selector)
        {
            selector = Compiler.smartEmpty(selector);
            if (selector.Contains("["))
            {
                if (!selector.EndsWith("]"))
                    return false;
                string args = selector.Substring(selector.IndexOf("[") + 1,
                    selector.LastIndexOf("]") - selector.IndexOf("[") - 1);
                foreach (string arg in Compiler.smartSplitJson(args, ',',-1))
                {
                    if (!isValidSelectorArgument(arg))
                        return false;
                }
                if (selector.StartsWith("@s") || selector.StartsWith("@p") ||
                    selector.StartsWith("@a") || selector.StartsWith("@r") || selector.StartsWith("@e"))
                    return true;
                else
                    return false;
            }
            else
            {
                if (selector == "@s" || selector == "@p" || selector == "@a" || selector == "@r" || selector == "@e")
                    return true;
                else
                    return false;
            }
        }
        public static bool isValidSelectorArgument(string arg)
        {
            arg = Compiler.smartEmpty(arg);
            string[] part  = Compiler.smartSplit(arg, '=');
            if (part.Length != 2)
                return false;
            
            if (part[0] == "x" || part[0] == "y" || part[0] == "z" || part[0] == "dx" || part[0] == "dy" || part[0] == "dz" ||
                part[0] == "limit")
            {
                return float.TryParse(part[1], out float t);
            }
            if (part[0] == "distance" || part[0] == "level" || part[0] == "x_rotation" || part[0] == "y_rotation")
            {
                if (part[1].Contains(".."))
                {
                    string[] part2 = part[1].Replace("..", ",").Split(',');

                    return (part2[0] == "" || float.TryParse(part2[0], out float t)) && (part2[1] == "" || float.TryParse(part2[1], out float u));
                }
                else
                {
                    return float.TryParse(part[1], out float t);
                }
            }
            if (part[0] == "gamemode")
            {
                part[1] = part[1].Replace("!", "");
                return part[1] == "adventure" || part[1] == "survival" || part[1] == "creative" || part[1] == "spectator";
            }
            if (part[0] == "sort")
            {
                return part[1] == "nearest" || part[1] == "furthest" || part[1] == "random" || part[1] == "arbitrary";
            }
            if (part[0] == "tag" || part[0] == "name" || part[0] == "team" || part[0] == "type")
            {
                return !part[1].Contains("\"");
            }
            if ("nbt".StartsWith(part[0]) || part[0] == "advancements‌" || part[0] == "predicate‌" || part[0] == "scores")
            {
                return true;
            }

            return false;
        }

        public class Gamerule
        {
            public string name;
            public string val;
            public string type;
            public string desc;

            public Gamerule(string name, string val, string type, string desc)
            {
                this.name = name;
                this.val = val;
                this.type = type;
                this.desc = desc;
            }

            public override string ToString()
            {
                return name;
            }
        }
    }
}
