using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    public static class Formatter
    {
        private static string[] funKeyword = { "debug", "print", "warning", "exception" };
        private static string[] typKeyword = { "int", "float", "entity", "string", "bool", "function", "selector" };
        private static string[] compKeyword = { "json", "params", "implicite", "define" };
        private static string[] blueWord = { "true", "false" };
        private static string[] importWord = { "import", "package", "using", "as" };
        private static string[] selector = { "@a", "@e", "@p", "@s", "@r" };
        private static string[] defWord = { "def", "class", "initer", "object", "if", "ifs", "then", "else", "elseif", "elif", "elsif",
            "while", "for", "with", "forgenerate", "interface", "foreach" , "return", "&&", "||", "at",
            "switch", "case", "const", "final", "override", "virtual",
            "struct", "extends", "static", "positioned", "lazy", "abstract", "this", "align", "alias",
            "ticking", "loading", "helper", "void", "null", "enum", "blocktags","entitytags","itemtags", "public", "private",
            "new", "external", "jsonfile", "require", "indexed", "predicate", "let", "var", "val", "extension", "implements",
            "invisible","stacksafe", "infix","prefix"};

        private static string[] autoCompleteTools ={
            "public void ^(){\n\n}",
            "public class ^{\n\n}",
            "public struct ^{\n\n}",
            "public enum ^{\n\n}",
            "private void ^(){\n\n}",
            "private class ^{\n\n}",
            "private struct ^{\n\n}",
            "private enum ^{\n\n}",
            "for(int i=0;i < ^;i++){\n\n}",
            "def ticking main(){\n\t^\n}",
            "def __init__(){\n\t^\n}",
            "minecraft:",
            "switch(^){\n\n}",
            "switch(^){\nforgenerate(){\n\n}\n}",
            "blocktags ^{\n\n}",
            "itemtags ^{\n\n}",
            "entitytags ^{\n\n}",
            "jsonfile ^{\n\n}"
        };

        private static List<string> enums = new List<string>();
        private static List<string> structs = new List<string>();
        private static List<string> enumsValue = new List<string>();
        private static List<string> package = new List<string>();
        private static List<string> tags = new List<string>();
        public static List<string> defWordMore1 = new List<string>();
        public static List<string> defWordMore1F = new List<string>();
        public static List<string> defWordMore1L = new List<string>();
        public static List<string> defWordMore1M = new List<string>();
        public static List<string> defWordMore2 = new List<string>();
        public static Dictionary<string, List<string>> varWord = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> objectFunc = new Dictionary<string, List<string>>();

        private static Regex numberRegex = new Regex(@"(-?\b)(\d+\.\d+|\d+)[bldsf]?\b");
        private static Regex wordRegex = new Regex("\"[^\"\n]*\"");//= new Regex("\"(([^\\n\"]+)*(\\\\\")*)*\"");
        private static Regex commentRegex = new Regex(@"(?s)(//[^\n]*|/\*[^*]*\*/)");
        private static Regex funcDocRegex = new Regex("(?s)\"\"\"[^\"\"\"]*\"\"\"");
        private static List<ColorCoding> colorCodings = new List<ColorCoding>();
        public static bool reformating = false;
        public static bool showName = true;
        public static bool showFunc = true;
        public static bool showEnumValue = true;
        public static Image IMG_Class;
        public static Image IMG_Enum;
        public static Image IMG_Method;
        public static Image IMG_Lazy_Method;
        public static Image IMG_Event;
        public static Image IMG_Word;
        public static Image IMG_String;
        public static Image IMG_Package; 
        public static Image IMG_Entity;
        public static Image IMG_Sound;
        public static Image IMG_Variable;
        public static Image IMG_Object_Method;

        public static void loadDict()
        {
            Color cClass = Color.FromArgb(68, 201, 162);
            Color cFunction = Color.FromArgb(124, 220, 240);
            Color cString = Color.FromArgb(218, 105, 26);
            Color cKeyword = Color.FromArgb(255, 255, 200);

            colorCodings = new List<ColorCoding>();
            colorCodings.Add(ColorCoding.GetSelector(selector, Color.LightBlue, ""));
            colorCodings.Add(ColorCoding.Get(blueWord, Color.Aqua, "Bold"));

            colorCodings.Add(ColorCoding.Get(CommandParser.funcName, Color.FromArgb(0, 185, 255), ""));
            colorCodings.Add(ColorCoding.Get(defWord.Concat(importWord).Concat(defWordMore1).Concat(defWordMore1F).Concat(defWordMore1M).Concat(defWordMore1L).Distinct().ToArray(),
                Color.FromArgb(74, 156, 199), "Bold"));

            colorCodings.Add(ColorCoding.Get(funKeyword
                                            .Concat(compKeyword)
                                            .Concat(tags.ToArray()).Distinct()
                                            .Concat(CommandParser.dataattribute.Select(x => "\\[" + x + "\\]"))
                                            .ToArray(), Color.Magenta, "Bold"));

            colorCodings.Add(ColorCoding.Get(typKeyword.Distinct().ToArray(), Color.Orange, "Bold"));

            if (showName)
            {
                colorCodings.Add(ColorCoding.Get(CommandParser.names.Distinct()
                            .Concat(CommandParser.scoreboards.Distinct())
                            .Concat(CommandParser.effects)
                            .Concat(CommandParser.gamerules.Distinct())
                            .Concat(CommandParser.sounds.Distinct())
                            .ToArray(), cKeyword, ""));
            }


            colorCodings.Add(new ColorCoding(Color.Magenta, numberRegex, @"(-?\b)(\d+\.\d+|\d+)[bldsf]?\b", ""));
            colorCodings.Add(ColorCoding.Get(structs.Concat(enums).Distinct().ToArray(), cClass, "Bold"));
            colorCodings.Add(ColorCoding.Get(defWordMore2.Distinct().ToArray(), cFunction, ""));
            colorCodings.Add(ColorCoding.GetPackage(package.Distinct().ToArray(), Color.FromArgb(74, 156, 199), ""));

            if (showEnumValue)
            {
                colorCodings.Add(ColorCoding.Get(enumsValue.Distinct().ToArray(), Color.LightGreen, ""));
            }

            colorCodings.Add(new ColorCoding(Color.Gray, commentRegex, @"(?s)(//[^\n]*|/\*[^*]*\*/)", "Italic"));
            colorCodings.Add(new ColorCoding(cString, wordRegex, "\"[^\"]*\"", "Italic"));
            colorCodings.Add(new ColorCoding(Color.LightYellow, funcDocRegex, "(?s)\"\"\"[^\"\"\"]*\"\"\"", "Italic"));
            generateXML();

            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/assets/";
            IMG_Class = Image.FromFile(path + "class.png");
            IMG_Enum = Image.FromFile(path + "enum.png");
            IMG_Method = Image.FromFile(path + "method.png");
            IMG_Lazy_Method = Image.FromFile(path + "lazy_method.png");
            IMG_Event = Image.FromFile(path + "event.png");
            IMG_Word = Image.FromFile(path + "word.png");
            IMG_String = Image.FromFile(path + "string.png");
            IMG_Package = Image.FromFile(path + "package.png");
            IMG_Entity = Image.FromFile(path + "entity.png");
            IMG_Sound = Image.FromFile(path + "sound.png");
            IMG_Variable = Image.FromFile(path + "variable.png");
            IMG_Object_Method = Image.FromFile(path + "object_method.png");
        }
        private static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
        public static void generateXML()
        {
            string doc = "<doc>\n";
            doc += "<brackets left=\"\\{\" right=\"\\}\"/>\n";
            doc += "<brackets left=\"\\[\" right=\"\\]\"/>\n";
            doc += "<brackets left=\"\\(\" right=\"\\)\"/>\n";

            int index = 0;
            colorCodings.Reverse();
            foreach (var c in colorCodings)
            {
                string pattern = c.pattern.Replace("&", "&amp;")
                                          .Replace(">", "&gt;")
                                          .Replace("<", "&lt;")
                                          .Replace("'", "&apos;")
                                          .Replace("\"", "&quot;");
                if (pattern != null && pattern != "§§§§§§§§§§§")
                {
                    if (c.fontStyle == "")
                        doc += $"<style name=\"s_{index}\" color=\"{HexConverter(c.c)}\" />\n";
                    else
                        doc += $"<style name=\"s_{index}\" color=\"{HexConverter(c.c)}\" fontStyle=\"{c.fontStyle}\" />\n";
                    doc += $"<rule style=\"s_{index}\" options=\"Singleline,IgnoreCase\">{pattern}</rule>\n";
                }
                index++;
            }
            colorCodings.Reverse();
            doc += "<folding start=\"\\{\" finish=\"\\}\" options=\"IgnoreCase\"/></doc>\n";
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            File.WriteAllText(path + "formating.xml", doc);
        }
        public static List<(string,Image)> getAutoCompleteFull(string file, string text)
        {
            var vars = varWord.ContainsKey(file) ? varWord[file] : new List<string>();
            var func = objectFunc.ContainsKey(file) ? objectFunc[file] : new List<string>();
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/assets/mob_effect/";
            var effect = CommandParser.effects.Where(x => File.Exists($"{path}{x}.png"));

            return autoCompleteTools.Select(x => (x, IMG_String))
                      .Concat(funKeyword.Select(x => $"{x}(^)").Select(x => (x, IMG_Method)))
                      .Concat(typKeyword.Select(x => (x, IMG_String)))
                      .Concat(compKeyword.Select(x => (x, IMG_Word)))
                      .Concat(blueWord.Select(x => (x, IMG_Word)))
                      .Concat(importWord.Select(x => (x, IMG_Word)))
                      .Concat(selector.Select(x => $"{x}[^]").Select(x => (x, IMG_Entity)))
                      .Concat(defWord.Select(x => (x, IMG_Word)))
                      .Concat(enums.Select(x => (x, IMG_Enum)))
                      .Concat(structs.Select(x => $"{x} ^ = {x}()").Select(x => (x, IMG_Class)))
                      .Concat(package.Select(x => (x, IMG_Package)))
                      .Concat(CommandParser.names.Select(x => $"minecraft:{x}").Select(x => (x, IMG_String)))
                      .Concat(CommandParser.sounds.Select(x => $"minecraft:{x}").Select(x => (x, IMG_Sound)))
                      .Concat(defWordMore1.Select(x => $"{x}(^)").Select(x => (x, IMG_Method)))
                      .Concat(defWordMore1F.Select(x => $"{x}" + "{\n^\n} ").Select(x => (x, IMG_Event)))
                      .Concat(defWordMore1M.Select(x => $"{x}()" + "{\n^\n} ").Select(x => (x, IMG_Event)))
                      .Concat(defWordMore1L.Select(x => $"{x}(^)").Select(x => (x, IMG_Lazy_Method)))
                      .Concat(defWordMore2.Select(x => $"{x}(^)").Select(x => (x, IMG_Method)))
                      .Concat(vars.Select(x => (x, IMG_Variable)))
                      .Concat(func.Select(x => $"{x}(^)").Select(x => (x, IMG_Object_Method)))
                      .Concat(effect.Select(x => (x, Image.FromFile($"{path}{x}.png"))))
                      .Concat(CommandParser.funcName.Select(x => (x, IMG_Lazy_Method)))
                      .Distinct().ToList();
        }
        public static void getAutoComplete(AutocompleteMenuNS.AutocompleteMenu menu, string file, string text)
        {
            var lst = getAutoCompleteFull(file, text);
            lst.Sort((x,y)=>x.Item1.CompareTo(y.Item1));
            menu.Items = new string[0];
            menu.ImageList = new ImageList();
            Image[] imgs = lst.Select(x => x.Item2).Distinct().ToArray();
            menu.ImageList.Images.AddRange(imgs);
            lst.ForEach(x => {
                menu.AddItem(new AutocompleteMenuNS.AutocompleteItem(x.Item1, imgs.ToList().IndexOf(x.Item2)));
                }
            );
        }

        public static void setEnum(List<string> keys)
        {
            enums = keys;
        }
        public static void setEnumValue(List<string> values)
        {
            enumsValue = values;
        }
        public static void setStructs(List<string> keys)
        {
            structs = keys;
        }
        public static void setpackage(List<string> keys)
        {
            package = keys;
        }
        public static void setTags(List<string> keys)
        {
            tags = new List<string>();
            foreach (string tag in keys)
            {
                tags.Add("@" + tag);
            }
        }
        public static void setDefWord(List<string> keys)
        {
            defWordMore1 = new List<string>();
            defWordMore2 = new List<string>();
            foreach (string tag in keys)
            {
                if (tag.Contains("."))
                    defWordMore2.Add(tag);
                else
                    defWordMore1.Add(tag);
            }
        }

        public static void reformat(RichTextBox CodeBox, Form f, bool partial)
        {
            if (!reformating)
            {
                reformating = true;

                int selectStart = CodeBox.SelectionStart;

                int lineIndex = CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart);
                int lineCharIndex = CodeBox.GetFirstCharIndexFromLine(lineIndex);

                bool hadFocus = false;
                //partial = false;
                if (CodeBox.Focused)
                {
                    f.ActiveControl = null;
                    hadFocus = true;
                }

                int start = partial ? Math.Max(lineCharIndex, 0) : 0;
                if (lineIndex < CodeBox.Lines.Length)
                {
                    int end = start + CodeBox.Lines[lineIndex].Length + 1;
                    if (!partial)
                    {
                        end = CodeBox.TextLength;
                    }

                    CodeBox.Select(start, end - start);
                    CodeBox.SelectionColor = Color.White;
                    CodeBox.SelectionTabs = new int[] { 25, 50, 75, 100, 125, 150, 175, 200 };
                    CodeBox.Select(selectStart, 0);

                    CheckWords(CodeBox, start, end);
                }

                CodeBox.SelectionStart = selectStart;

                if (hadFocus)
                    CodeBox.Focus();

                reformating = false;
            }
        }

        private static List<List<Word>> smartReplace(string t, int start, int length)
        {

            List<List<Word>> words = new List<List<Word>>();
            words.Add(new List<Word>(2000));
            foreach (ColorCoding colorCoding in colorCodings)
            {
                foreach (Match match in colorCoding.r.Matches(t.Substring(start, length)))
                {
                    words[0].Add(new Word(colorCoding.c, start + match.Index, match.Length));
                }
            }

            return words;
        }
        private static List<List<Word>> smartReplaceThreaded(string t, int start, int length)
        {
            List<Task<List<Word>>> tasks = new List<Task<List<Word>>>(colorCodings.Count);
            List<List<Word>> words = new List<List<Word>>();

            foreach (ColorCoding colorCoding in colorCodings)
            {
                tasks.Add(Task<List<Word>>.Factory.StartNew(() =>
                {
                    List<Word> words2 = new List<Word>(2000);
                    foreach (Match match in colorCoding.r.Matches(t.Substring(start, length)))
                    {
                        words2.Add(new Word(colorCoding.c, start + match.Index, match.Length));
                    }
                    return words2;
                }
                ));
            }
            foreach (Task<List<Word>> task in tasks)
            {
                words.Add(task.Result);
            }

            return words;
        }


        private static void CheckWords(RichTextBox CodeBox, int startIndex, int endIndex)
        {
            try
            {
                List<List<Word>> words = endIndex - startIndex > 1000 ?
                    smartReplaceThreaded(CodeBox.Text + "\n", startIndex, endIndex - startIndex) :
                    smartReplace(CodeBox.Text + "\n", startIndex, endIndex - startIndex);

                int selectStart = CodeBox.SelectionStart;
                foreach (List<Word> lst in words)
                {
                    foreach (Word w in lst)
                    {
                        if (w.start >= startIndex && w.end <= endIndex)
                        {
                            CodeBox.Select(w.start, w.end);
                            CodeBox.SelectionColor = w.color;
                        }
                        else if (w.end > endIndex)
                            break;
                    }
                }

                CodeBox.Select(selectStart, 0);
                CodeBox.SelectionColor = Color.White;
            }
            catch { }
        }
        private static void CheckWords(RichTextBox CodeBox)
        {
            List<List<Word>> words = CodeBox.Text.Length > 1000 ?
                    smartReplaceThreaded(CodeBox.Text + "\n", 0, CodeBox.Text.Length) :
                    smartReplace(CodeBox.Text + "\n", 0, CodeBox.Text.Length);

            int selectStart = CodeBox.SelectionStart;

            foreach (List<Word> lst in words)
            {
                foreach (Word w in lst)
                {
                    CodeBox.Select(w.start, w.end);
                    CodeBox.SelectionColor = w.color;
                }
            }

            CodeBox.Select(selectStart, 0);
            CodeBox.SelectionColor = Color.White;
        }

        private struct Word
        {
            public int start;
            public int end;
            public Color color;
            public Word(Color c, int s, int e)
            {
                color = c;
                start = s;
                end = e;
            }
        }
        private class ColorCoding
        {
            public Color c;
            public Regex r;
            public string pattern;
            public string fontStyle;

            public ColorCoding(Color c, Regex r, string pattern, string fontStyle)
            {
                this.c = c;
                this.r = r;
                this.pattern = pattern;
                this.fontStyle = fontStyle;
            }

            public static ColorCoding Get(string[] lst, Color c, string fontStyle)
            {
                string s = "(?i)(";
                foreach (string p in lst)
                {
                    if (p.StartsWith("\\"))
                        s += p + "|";
                    else
                        s += @"\b" + p.Replace("|", "\\|").Replace(".", "\\.") + @"\b|";
                }
                s = s.Substring(0, s.Length - 1) + ")";
                if (s == "(?i))")
                {
                    s = "§§§§§§§§§§§";
                }
                return new ColorCoding(c, new Regex(s), s, fontStyle);
            }
            public static ColorCoding GetPackage(string[] lst, Color c, string fontStyle)
            {
                string s = "(?i)(";
                foreach (string p in lst)
                {
                    s += @"\b" + p.Replace("|", "\\|") + @"\.|";
                }
                s = s.Substring(0, s.Length - 1) + ")";
                if (s == "(?i))")
                {
                    s = "§§§§§§§§§§§";
                }
                return new ColorCoding(c, new Regex(s), s, fontStyle);
            }
            public static ColorCoding GetSelector(string[] lst, Color c, string fontStyle)
            {
                string s = "(?i)(";
                foreach (string p in lst)
                {
                    s += p.Replace("|", "\\|").Replace("@", "\\@") + @"\[[^\n]*\]" + @"|";
                    s += p.Replace("|", "\\|").Replace("@", "\\@") + @"\b|";
                }
                s = s.Substring(0, s.Length - 1) + ")";
                if (s == "(?i))")
                {
                    s = "§§§§§§§§§§§";
                }
                return new ColorCoding(c, new Regex(s), s, fontStyle);
            }
        }
    }
}
