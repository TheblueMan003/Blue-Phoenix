using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    public static class FormatterCommand
    {
        private static string[] selector = { "@a", "@e", "@p", "@s", "@r" };


        private static List<string> enums = new List<string>();
        private static List<string> structs = new List<string>();
        private static List<string> enumsValue = new List<string>();
        private static List<string> package = new List<string>();
        private static List<string> tags = new List<string>();

        private static Regex numberRegex = new Regex(@"(-?\b)(\d+\.\d+|\d+)\b");
        private static Regex wordRegex = new Regex("(\"[^\\n\"]+\"|\"\")");
        private static Regex commentRegex = new Regex(@"(?s)^#\w+$");
        private static Regex funcDocRegex = new Regex("(?s)\"\"\"[^\"\"\"]*\"\"\"");
        private static List<ColorCoding> colorCodings = new List<ColorCoding>();
        public static bool reformating = false;
        public static bool showName = false;

        public static void loadDict()
        {
            colorCodings = new List<ColorCoding>();
            
            colorCodings.Add(ColorCoding.Get(CommandParser.cmds, Color.FromArgb(0, 185, 255)));
            colorCodings.Add(ColorCoding.Get(CommandParser.grp2, Color.MediumAquamarine));
            colorCodings.Add(ColorCoding.Get(CommandParser.names, Color.OrangeRed));
            colorCodings.Add(ColorCoding.Get(CommandParser.scoreboards, Color.OrangeRed));
            colorCodings.Add(ColorCoding.Get(CommandParser.effects, Color.OrangeRed));
            colorCodings.Add(ColorCoding.Get(CommandParser.gamerules.ToArray(), Color.OrangeRed));
            colorCodings.Add(ColorCoding.Get(CommandParser.sounds, Color.OrangeRed));
            
            colorCodings.Add(ColorCoding.GetPackage(package.ToArray(), Color.LightSteelBlue));

            colorCodings.Add(ColorCoding.GetSelector(selector, Color.LightBlue));
            colorCodings.Add(new ColorCoding(Color.Magenta, numberRegex));
            colorCodings.Add(new ColorCoding(Color.Gray, commentRegex));
            colorCodings.Add(new ColorCoding(Color.FromArgb(0, 128, 14), wordRegex));
        }

        public static void reformat(RichTextBox CodeBox, Form f, bool partial)
        {
            if (!reformating)
            {
                loadDict();
                reformating = true;

                int selectStart = CodeBox.SelectionStart;

                int l = CodeBox.GetLineFromCharIndex(CodeBox.SelectionStart);
                int v = CodeBox.GetFirstCharIndexFromLine(l);

                bool hadFocus = false;
                //partial = false;
                if (CodeBox.Focused)
                {
                    f.ActiveControl = null;
                    hadFocus = true;
                }

                int start = partial ? Math.Max(v, 0) : 0;
                if (l < CodeBox.Lines.Length)
                {
                    int end = start + CodeBox.Lines[l].Length;
                    if (!partial)
                    {
                        end = CodeBox.TextLength;
                    }
                    string line = CodeBox.Text.Substring(start, end - start);

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
        private static List<Word> smartReplace(string t)
        {
            List<Word> words = new List<Word>(2000);

            foreach (ColorCoding colorCoding in colorCodings)
            {
                foreach (Match match in colorCoding.r.Matches(t))
                {
                    words.Add(new Word(colorCoding.c, match.Index, match.Length));
                }
            }

            return words;
        }


        private static void CheckWords(RichTextBox CodeBox, int startIndex, int endIndex)
        {
            try
            {
                List<Word> words = smartReplace(CodeBox.Text);

                int selectStart = CodeBox.SelectionStart;

                foreach (Word w in words)
                {
                    if (w.start >= startIndex && w.end <= endIndex)
                    {
                        CodeBox.Select(w.start, w.end);
                        CodeBox.SelectionColor = w.color;
                    }
                    else if (w.end > endIndex)
                        break;
                }

                CodeBox.Select(selectStart, 0);
                CodeBox.SelectionColor = Color.White;
            }
            catch { }
        }
        private static void CheckWords(RichTextBox CodeBox)
        {
            List<Word> words = smartReplace(CodeBox.Text);

            int selectStart = CodeBox.SelectionStart;

            foreach (Word w in words)
            {
                CodeBox.Select(w.start, w.end);
                CodeBox.SelectionColor = w.color;
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
                foreach (string p in lst)
                {
                    s += @"\b" + p.Replace("|", "\\|").Replace(".", "\\.") + @"\b|";
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
                    s += p.Replace("|", "\\|").Replace("@", "\\@") + @"\[.*\]" + @"|";
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
