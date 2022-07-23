using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace JSharp
{
    public static class CommandParser
    {
        public static string[] funcName = new string[] { "tellraw", "title", "say", "clear", "effect", "difficulty", "gamemode", "gamerule",
            "fill", "stopsound", "weather", "tp", "execute", "structure","magictitle","magicgeneric", "datapack.component.block"};
        public static string[] difficulties;
        public static string[] effects;
        public static string[] gamemodes;
        public static string[] names;
        public static string[] scoreboards;
        public static string[] sounds;
        public static string[] cmds;
        public static string[] grp2;
        public static string[] dataattribute;
        public static string[] particles;
        public static HashSet<string> functionSet;
        public static List<string> gamerules = new List<string>();
        public static List<Gamerule> gamerulesObj = new List<Gamerule>();
        private static bool loaded;

        public static void loadDict()
        {
            if (!loaded)
            {
                functionSet = new HashSet<string>();
                loaded = true;
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
                difficulties = File.ReadAllLines(path + "cmd_data/difficulty.txt");
                effects = File.ReadAllLines(path + "cmd_data/effect.txt");
                gamemodes = File.ReadAllLines(path + "cmd_data/gamemode.txt");
                names = File.ReadAllLines(path + "cmd_data/names.txt");
                scoreboards = File.ReadAllLines(path + "cmd_data/scoreboard.txt");
                sounds = File.ReadAllLines(path + "cmd_data/sounds.txt");
                cmds = File.ReadAllLines(path + "cmd_data/colors/grp_1.txt");
                grp2 = File.ReadAllLines(path + "cmd_data/colors/grp_2.txt");
                dataattribute = File.ReadAllLines(path + "cmd_data/dataattribute.txt");
                particles = File.ReadAllLines(path + "cmd_data/particles.txt");
                foreach (string gr in File.ReadAllLines(path + "cmd_data/gamerule.txt"))
                {
                    string[] c = gr.Split('	');
                    if (c.Length > 3 && c[4] == "Yes")
                    {
                        gamerulesObj.Add(new Gamerule(c[0], c[2], c[3], c[1]));
                        gamerules.Add(c[0]);
                    }
                }
                foreach (string fun in funcName)
                    functionSet.Add(fun);
            }
        }
        public static bool canBeParse(string text)
        {
            return functionSet.Contains(text);
        }
        public static string parse(string text, Compiler.Context context, int rec = 0)
        {
            text = text.Trim();
            string[] args = Compiler.getArgs(text);

            var lower = text.ToLower();

            if (lower.StartsWith("structure"))
                return parseStructure(args, context, text, rec);

            if (lower.StartsWith("title"))
                return parseTitle(args, context, text, rec);

            if (lower.StartsWith("magictitle"))
                return parseMagicTitle(args, context, text, rec);

            if (lower.StartsWith("magicgeneric"))
                return parseMagicGeneric(args, context, text, rec);

            if (lower.StartsWith("tellraw"))
                return parseTellraw(args, context, text, rec);

            if (lower.StartsWith("clear"))
                return parseClear(args, context, text, rec);

            if (lower.StartsWith("say"))
                return parseSay(args, context, text, rec);

            if (lower.StartsWith("effect"))
                return parseEffect(args, context, text, rec);

            if (lower.StartsWith("difficulty"))
                return parseDifficulty(args, context, text, rec);

            if (lower.StartsWith("gamemode"))
                return parseGamemode(args, context, text, rec);

            if (lower.StartsWith("gamerule"))
                return parseGamerule(args, context, text, rec);

            if (lower.StartsWith("fill"))
                return parseFill(args, context, text, rec);

            if (lower.StartsWith("stopsound"))
                return parseStopsound(args, context, text, rec);

            if (lower.StartsWith("weather"))
                return parseWeather(args, context, text, rec);

            if (lower.StartsWith("tp"))
                return parseTP(args, context, text, rec);

            if (lower.StartsWith("datapack.component.block"))
                return parseBlock(args, context, text, rec);


            throw new NotImplementedException(text.Split(' ')[0] + " is not implemented");
        }

        private static string parseBlock(string[] args, Compiler.Context context, string text, int rec)
        {
            Compiler.FilterBlocks.Add(new DataPackMeta.Pack.Filter.Block(Compiler.extractString(args[0]), Compiler.extractString(args[1])));
            return "";
        }

        public static string parseTellraw(string[] args, Compiler.Context context, string text, int rec = 0)
        {
            string output = "tellraw " + context.GetEntitySelector(args[0]) + " ";
            string[] json = Compiler.Core.FormatJson(args, context, 1);
            output = json[1] + output + json[0] + "\n" + json[2];

            return output + '\n';
        }
        public static string parseTitle(string[] args, Compiler.Context context, string text, int rec = 0)
        {
            string titleType = Compiler.smartEmpty(args[1]);
            if (titleType != "actionbar" && titleType != "title" && titleType != "subtitle")
                throw new Exception("Invalid Title type: " + titleType);

            string output = "title " + context.GetEntitySelector(args[0]) + " " + titleType + " ";
            string[] json = Compiler.Core.FormatJson(args, context, 2);
            output = json[1] + output + json[0] + "\n" + json[2];
            return output + '\n';
        }
        public static string parseMagicTitle(string[] args, Compiler.Context context, string text, int rec = 0)
        {
            int maxTime;
            int argIndex = 1;
            if (!int.TryParse(Compiler.smartEmpty(args[1]), out maxTime))
            {
                maxTime = -1;
                argIndex = 0;
            }

            string titleType = Compiler.smartEmpty(args[argIndex + 2]);
            if (titleType != "actionbar" && titleType != "title" && titleType != "subtitle")
                throw new Exception("Invalid Title type: " + titleType);

            string titleLine = "title " + context.GetEntitySelector(args[argIndex + 1]) + " " + titleType + " ";
            string output = "";
            int time = 0;

            for (int i = argIndex + 3; i < args.Length; i++)
            {
                string arg = Compiler.smartEmpty(args[i]).StartsWith("(") ? Compiler.smartEmpty(args[i].Substring(args[i].IndexOf('(') + 1, args[i].LastIndexOf(')') - args[i].IndexOf('(') - 1)) : args[i];
                string[] subargs = Compiler.smartSplit(arg, ',');

                if (subargs[0].StartsWith("\""))
                {
                    for (int j = 1; j < subargs[0].Length - 1; j++)
                    {
                        if (subargs[0][j] == '\\')
                        {
                            if (subargs[0][j + 1] == 'u')
                            {
                                while (Char.IsHighSurrogate((char)Convert.ToInt32(subargs[0].Substring(j, 6).ToUpper().Replace("\\U", "0x"), 16)))
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
                        for (int k = argIndex + 3; k < i; k++)
                        {
                            json[k - 3 - argIndex] = args[k];
                        }
                        json[i - 3 - argIndex] = "(\"" + subargs[0].Substring(1, j) + "\"";
                        for (int k = 1; k < subargs.Length; k++)
                        {
                            json[i - 3 - argIndex] += "," + subargs[k];
                        }
                        json[i - 3 - argIndex] += ")";
                        string[] jsonParsed = Compiler.Core.FormatJson(json, context, 0);
                        output += "execute if score " + Compiler.GetVariableByName(args[0]).scoreboard() + " matches " + time.ToString() + " run " + titleLine + jsonParsed[0] + "\n";
                        time++;
                    }
                }
                else
                {
                    string[] json = new string[i - 2];
                    for (int k = argIndex + 3; k <= i; k++)
                    {
                        json[k - 3 - argIndex] = args[k];
                    }

                    string[] jsonParsed = Compiler.Core.FormatJson(json, context, 0);
                    output += "execute if score " + Compiler.GetVariableByName(args[0]).scoreboard() + " matches " + time.ToString() + " run " + titleLine + jsonParsed[0] + "\n";
                    time++;
                }
            }
            string[] jsonParsedGlobal = Compiler.Core.FormatJson(args, context, 3 + argIndex);
            output += "execute if score " + Compiler.GetVariableByName(args[0]).scoreboard() + " matches " + time.ToString() + ".. run " + titleLine + jsonParsedGlobal[0] + "\n";
            if (maxTime > -1)
            {
                output += "execute if score " + Compiler.GetVariableByName(args[0]).scoreboard() + " matches " + (time + maxTime).ToString() + ".. run scoreboard players set " + Compiler.GetVariableByName(args[0]).scoreboard() + " -100000\n";
            }
            return jsonParsedGlobal[1] + output + jsonParsedGlobal[2] + '\n';
        }
        public static string parseMagicGeneric(string[] args, Compiler.Context context, string text, int rec = 0)
        {
            int maxTime=int.Parse(Compiler.smartEmpty(args[1]));
            int argIndex = 4;
            int padding = int.Parse(Compiler.smartEmpty(args[2]));
            string aligned = args[3].Trim();
            string font = args[4];

            var tmp = args.ToList();
            tmp.Add("(\"\")");
            args = tmp.ToArray();
            
            string cmd = args[argIndex+1];
            argIndex --;

            string titleLine = cmd + " ";
            string output = "";
            int time = 0;

            for (int i = argIndex + 3; i < args.Length; i++)
            {
                string arg = Compiler.smartEmpty(args[i]).StartsWith("(") ? Compiler.smartEmpty(args[i].Substring(args[i].IndexOf('(') + 1, args[i].LastIndexOf(')') - args[i].IndexOf('(') - 1)) : args[i];
                string[] subargs = Compiler.smartSplit(arg, ',');

                if (subargs[0].StartsWith("\""))
                {
                    for (int j = 1; j < subargs[0].Length - 1; j++)
                    {
                        if (subargs[0][j] == '\\')
                        {
                            if (subargs[0][j + 1] == 'u')
                            {
                                while (Char.IsHighSurrogate((char)Convert.ToInt32(subargs[0].Substring(j, 6).ToUpper().Replace("\\U", "0x"), 16)))
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

                        string[] json = new string[i - argIndex];
                        if (padding != -1 && aligned == "right")
                        {
                            json[0] = "(\"" + new String(' ', padding - time - 1) + $"\",font={font})";
                        }
                        else if (padding != -1 && (aligned == "middle" || aligned == "center"))
                        {
                            json[0] = "(\"" + new String(' ', (padding - time - 1) / 2) + $"\",font={font})";
                        }
                        else
                        {
                            json[0] = $"(\"\")";
                        }

                        for (int k = argIndex + 3; k < i; k++)
                        {
                            json[k - 2 - argIndex] = args[k];
                        }
                        json[i - 2 - argIndex] = "(\"" + subargs[0].Substring(1, j) + "\"";
                        for (int k = 1; k < subargs.Length; k++)
                        {
                            json[i - 2 - argIndex] += "," + subargs[k];
                        }
                        json[i - 2 - argIndex] += ", font=" + font;
                        json[i - 2 - argIndex] += ")";

                        if (padding != -1 && aligned == "left")
                        {
                            json[i - argIndex - 1] = "(\"" + new String(' ', padding - time - 1) + $"\",font={font})";
                        }
                        else if (padding != -1 && (aligned == "middle" || aligned == "center"))
                        {
                            json[i - argIndex - 1] = "(\"" + new String(' ', (padding - time - 1) / 2) + $"\",font={font})";
                        }
                        else
                        {
                            json[i - argIndex - 1] = "(\"\")";
                        }

                        string[] jsonParsed = Compiler.Core.FormatJson(json, context, 0);
                        if (i == args.Length-1)
                        {
                            output += "execute if score " + Compiler.GetVariableByName(args[0]).scoreboard() + " matches " + time.ToString() + ".. run " + titleLine + jsonParsed[0] + "\n";
                        }
                        else
                        {
                            output += "execute if score " + Compiler.GetVariableByName(args[0]).scoreboard() + " matches " + time.ToString() + " run " + titleLine + jsonParsed[0] + "\n";
                        }
                        time++;
                    }
                }
                else
                {
                    string[] json = new string[i];

                    if (padding != -1 && aligned == "right")
                    {
                        json[0] = "(\"" + new String(' ', padding - time - 1) + "\")";
                    }
                    else if (padding != -1 && (aligned == "middle" || aligned == "center"))
                    {
                        json[0] = "(\"" + new String(' ', (padding - time - 1)/2) + "\")";
                    }
                    else
                    {
                        json[0] = "(\"\")";
                    }

                    for (int k = argIndex + 3; k <= i; k++)
                    {
                        json[k - 2 - argIndex] = args[k];
                    }
                    if (padding != -1 && aligned == "left")
                    {
                        json[i - 1 - argIndex] = "(\"" + new String(' ', padding - time - 1) + "\")";
                    }

                    if (padding != -1 && aligned == "left")
                    {
                        json[i-1] = "(\"" + new String(' ', padding - time - 1) + "\")";
                    }
                    else if (padding != -1 && (aligned == "middle" || aligned == "center"))
                    {
                        json[i-1] = "(\"" + new String(' ', (padding - time - 1) / 2) + "\")";
                    }
                    else
                    {
                        json[i-1] = "(\"\")";
                    }

                    string[] jsonParsed = Compiler.Core.FormatJson(json, context, 0);
                    if (i == args.Length - 1)
                    {
                        output += "execute if score " + Compiler.GetVariableByName(args[0]).scoreboard() + " matches " + time.ToString() + ".. run " + titleLine + jsonParsed[0] + "\n";
                    }
                    else
                    {
                        output += "execute if score " + Compiler.GetVariableByName(args[0]).scoreboard() + " matches " + time.ToString() + " run " + titleLine + jsonParsed[0] + "\n";
                    }
                    time++;
                }
            }
            string[] jsonParsedGlobal = Compiler.Core.FormatJson(args, context, 3 + argIndex);
            if (maxTime > -1)
            {
                output += "execute if score " + Compiler.GetVariableByName(args[0]).scoreboard() + " matches " + (time + maxTime).ToString() + ".. run scoreboard players set " + Compiler.GetVariableByName(args[0]).scoreboard() + " -100000\n";
            }
            return jsonParsedGlobal[1] + output + jsonParsedGlobal[2] + '\n';
        }
        public static string parseSay(string[] args, Compiler.Context context, string text, int rec = 0)
        {
            if (args.Length == 1)
            {
                string output = "say ";

                for (int i = 0; i < args.Length; i++)
                {
                    output += args[i] + " ";
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text, null, "=", rec + 1);
            }
        }
        public static string parseClear(string[] args, Compiler.Context context, string text, int rec = 0)
        {
            if (args.Length >= 0 && args.Length < 3)
            {
                string output = "clear ";
                if (args.Length == 0)
                    output += "@s";
                else if (context.isEntity(args[0]))
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
                return Compiler.functionEval(text, null,"=", rec+1);
            }
        }
        public static string parseEffect(string[] args, Compiler.Context context, string text, int rec = 0)
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
                return Compiler.functionEval(text, null, "=", rec+1);
            }
        }
        public static string parseDifficulty(string[] args, Compiler.Context context, string text, int rec = 0)
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
                return Compiler.functionEval(text, null, "=", rec+1);
            }
        }
        public static string parseGamemode(string[] args, Compiler.Context context, string text, int rec = 0)
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
                return Compiler.functionEval(text, null, "=", rec + 1);
            }
        }
        public static string parseGamerule(string[] args, Compiler.Context context, string text, int rec = 0)
        {
            if (args.Length == 2)
            {
                string output = "gamerule " + Compiler.smartEmpty(args[0]) + " " + Compiler.smartEmpty(args[1]);

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text, null, "=", rec + 1);
            }
        }
        public static string parseFill(string[] args, Compiler.Context context, string text, int rec = 0)
        {
            if (args.Length == 1)
            {
                string output = "fill";
                Regex r = new Regex(@"#[\w\.]+");
                for (int i = 0; i < args.Length; i++)
                {
                    Match m = r.Match(args[i]);
                    if (m.Success)
                        output += " " + Compiler.regReplace(args[i], m, "#" + Compiler.Core.FormatTagsPath(context.GetBlockTags(m.Value.Replace("#", ""))));
                    else
                        output += " " + args[i];
                }

                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text, null, "=", rec + 1);
            }
        }
        public static string parseStopsound(string[] args, Compiler.Context context, string text, int rec = 0)
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
                return Compiler.functionEval(text, null, "=", rec + 1);
            }
        }
        public static string parseWeather(string[] args, Compiler.Context context, string text, int rec = 0)
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
                return Compiler.functionEval(text, null, "=", rec + 1);
            }
        }
        public static string parseStructure(string[] args, Compiler.Context context, string text, int rec = 0)
        {
            if (args.Length == 1)
            {
                string output = "setblock ~ ~ ~ minecraft:structure_block[mode = load]{ignoreEntities: 0b,rotation: \"NONE\",mode: \"LOAD\",integrity: 1.0f,name: \"" + Compiler.Project.ToLower() + ":" + args[0].Replace("\"", "") + "\",showboundingbox: 1b}";
                output += "\nsetblock ~ ~1 ~ redstone_block";
                return output + '\n';
            }
            else
            {
                return Compiler.functionEval(text, null, "=", rec + 1);
            }
        }
        public static string parseTP(string[] args, Compiler.Context context, string text, int rec = 0)
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
                return Compiler.functionEval(text, null, "=", rec + 1);
            }
        }
        private static string getParameter(string subargs)
        {
            string link = subargs;
            link = link.Substring(link.IndexOf("=") + 1, link.Length - link.IndexOf("=") - 1);
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
