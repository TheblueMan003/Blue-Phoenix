﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    public static class Formatter
    {
        private static string[] funKeyword = { "debug" ,"print"};
        private static string[] typKeyword = { "int", "float", "entity", "string", "bool", "function", "selector"};
        private static string[] compKeyword = { "json", "params", "implicite", "define"};
        private static string[] blueWord = { "true", "false" };
        private static string[] importWord = { "import", "package", "using","as" };
        private static string[] selector = { "@a", "@e", "@p", "@s", "@r" };
        private static string[] defWord = { "def", "class", "initer", "object", "if", "ifs", "then", "else", "elseif", "elif", "elsif",
            "while", "for", "with", "forgenerate", "interface", "foreach" , "return", "&&", "||", "at",
            "switch", "case", "const", "final", "override", "virtual",
            "struct", "extends", "static", "positioned", "lazy", "abstract", "this", "align", "alias",
            "ticking", "loading", "helper", "void", "null", "enum", "blocktags", "public", "private",
            "new", "external", "jsonfile", "require", "indexed", "predicate", "let", "var", "val", "extension"};

        private static List<string> enums = new List<string>();
        private static List<string> structs = new List<string>();
        private static List<string> enumsValue = new List<string>();
        private static List<string> package = new List<string>();
        private static List<string> tags = new List<string>();
        private static List<string> defWordMore1 = new List<string>();
        private static List<string> defWordMore2 = new List<string>();

        private static Regex numberRegex = new Regex(@"(-?\b)(\d+\.\d+|\d+)\b");
        private static Regex wordRegex = new Regex("(\"(([^\\n\"]*(\\\")*)*)+\"|\"\")");
        private static Regex commentRegex = new Regex(@"(?s)(//[^\n]*|/\*[^*]*\*/)");
        private static Regex funcDocRegex = new Regex("(?s)\"\"\"[^\"\"\"]*\"\"\"");
        private static List<ColorCoding> colorCodings = new List<ColorCoding>();
        public static bool reformating = false;
        public static bool showName = false;

        public static void loadDict()
        {
            colorCodings = new List<ColorCoding>();
            colorCodings.Add(ColorCoding.Get(blueWord, Color.Aqua));
            colorCodings.Add(ColorCoding.Get(importWord, Color.FromArgb(74, 156, 199)));
            colorCodings.Add(ColorCoding.Get(CommandParser.funcName, Color.FromArgb(0, 185, 255)));
            colorCodings.Add(ColorCoding.Get(defWord, Color.FromArgb(74, 156, 199)));
            colorCodings.Add(ColorCoding.Get(defWordMore1.ToArray(), Color.FromArgb(74, 156, 199)));
            colorCodings.Add(ColorCoding.Get(funKeyword, Color.Magenta));
            colorCodings.Add(ColorCoding.Get(typKeyword, Color.Orange));
            colorCodings.Add(ColorCoding.Get(compKeyword, Color.Magenta));
            colorCodings.Add(ColorCoding.GetSelector(tags.ToArray(), Color.Magenta));

            
            colorCodings.Add(ColorCoding.Get(CommandParser.names, Color.Orange));
            colorCodings.Add(ColorCoding.Get(CommandParser.scoreboards, Color.Orange));
            colorCodings.Add(ColorCoding.Get(CommandParser.effects, Color.Orange));
            colorCodings.Add(ColorCoding.Get(CommandParser.gamerules.ToArray(), Color.Orange));
            colorCodings.Add(ColorCoding.Get(CommandParser.sounds, Color.Orange));
            

            colorCodings.Add(new ColorCoding(Color.Magenta, numberRegex));
            colorCodings.Add(ColorCoding.Get(structs.ToArray(), Color.LimeGreen));
            colorCodings.Add(ColorCoding.Get(defWordMore2.ToArray(), Color.LightSteelBlue));
            colorCodings.Add(ColorCoding.GetPackage(package.ToArray(), Color.LightSteelBlue));
            colorCodings.Add(ColorCoding.Get(enums.ToArray(), Color.LimeGreen));
            colorCodings.Add(ColorCoding.Get(enumsValue.ToArray(), Color.LightGreen));

            colorCodings.Add(ColorCoding.GetSelector(selector, Color.LightBlue));
            colorCodings.Add(new ColorCoding(Color.Gray, commentRegex));
            //colorCodings.Add(new ColorCoding(Color.FromArgb(0, 128, 14), wordRegex));
            colorCodings.Add(new ColorCoding(Color.LightYellow, funcDocRegex));
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
            foreach(string tag in keys)
            {
                tags.Add("@"+tag);
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

        public static void reformat(RichTextBox CodeBox,Form f, bool partial)
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
                    int end = start + CodeBox.Lines[lineIndex].Length+1;
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
            foreach (ColorCoding colorCoding in colorCodings) {
                foreach (Match match in colorCoding.r.Matches(t.Substring(start, length)))
                {
                    words[0].Add(new Word(colorCoding.c, start+match.Index, match.Length));
                }
            }
            
            return words;
        }
        private static List<List<Word>> smartReplaceThreaded(string t, int start, int length)
        {
            List<Task<List< Word>>> tasks = new List<Task<List<Word>>> (colorCodings.Count);
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
                List<List<Word>> words = endIndex - startIndex > 1000?
                    smartReplaceThreaded(CodeBox.Text+"\n", startIndex, endIndex-startIndex):
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

            public ColorCoding(Color c, Regex r)
            {
                this.c = c;
                this.r = r;
            }

            public static ColorCoding Get(string[] lst, Color c)
            {
                string s = "(?i)(";
                foreach(string p in lst)
                {
                    s += @"\b"+p.Replace("|","\\|").Replace(".", "\\.") + @"\b|";
                }
                s = s.Substring(0, s.Length - 1) + ")";
                if (s == "(?i))")
                {
                    s = "§§§§§§§§§§§";
                }
                return new ColorCoding(c, new Regex(s));
            }
            public static ColorCoding GetPackage(string[] lst, Color c)
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
                return new ColorCoding(c, new Regex(s));
            }
            public static ColorCoding GetSelector(string[] lst, Color c)
            {
                string s = "(?i)(";
                foreach (string p in lst)
                {
                    s += p.Replace("|", "\\|").Replace("@", "\\@") +@"\[.*\]" + @"|";
                    s += p.Replace("|", "\\|").Replace("@", "\\@") + @"\b|";
                }
                s = s.Substring(0, s.Length - 1) + ")";
                if (s == "(?i))")
                {
                    s = "§§§§§§§§§§§";
                }
                return new ColorCoding(c, new Regex(s));
            }
        }
    }
}
