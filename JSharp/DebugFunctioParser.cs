using JSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BluePhoenix
{
    public static class DebugFunctioParser
    {
        public static string[] color = { "green", "yellow", "gold", "orange", "red" };
        public static string Parse(string text)
        {
            text = text.Replace("\r", "");
            string output = "digraph g{";
            Regex rg = new Regex(@" *\[F\] ([\w:/]+) size=(\d+)");
            Stack<string> func = new Stack<string>();
            Stack<int> ind = new Stack<int>();
            Dictionary<string, int> hotmap = new Dictionary<string, int>();
            Dictionary<string, int> sizes = new Dictionary<string, int>();
            HashSet<string> calls = new HashSet<string>();

            void AddHotMap(string name, string size)
            {
                if (!hotmap.ContainsKey(name))
                {
                    hotmap[name] = 0;
                    sizes.Add(name, int.Parse(size));
                }
                hotmap[name]++;
            }
            void Add(string add)
            {
                if (!calls.Contains(add)) {
                    calls.Add(add);
                    output += add;
                }
            }

            func.Push(text.Split('\n')[0]);
            ind.Push(0);
            string pfunc = text.Split('\n')[0];
            int pind = 0;

            foreach (Match m in rg.Matches(text))
            {
                string value = m.Value;
                string name = m.Groups[1].Value;
                string size = m.Groups[2].Value;
                int nind = GetIndentNb(value);
                
                if (ind.Peek() == nind)
                {
                    func.Pop();
                    ind.Pop();
                    Add($"\"{func.Peek()}\"->\"{name}\"\n");
                    AddHotMap(name, size);
                    func.Push(name);
                    ind.Push(nind);
                }
                else if (ind.Peek() + 4 < nind)
                {
                    func.Push(pfunc);
                    ind.Push(pind);
                    Add($"\"{func.Peek()}\"->\"{name}\"\n");
                    AddHotMap(name, size);
                }
                else
                {
                    Add($"\"{func.Peek()}\"->\"{name}\"\n");
                    AddHotMap(name, size);
                }
                pfunc = name;
                pind = nind;
            }
            foreach(string k in hotmap.Keys)
            {
                int n = color.Length;
                string c1 = color[sizes[k] / 20 > n ? n : sizes[k] / 20];
                string c2 = color[hotmap[k] / 20 > n ? n : hotmap[k] / 20];
                Add($"\"{k}\" [style=\"filled\" fillcolor=\"{c1}\" color=\"{c2}\"]\n");
            }
            return output+"}";
        }
        public static int GetIndentNb(string text)
        {
            int i = 0;
            while (text[i] == ' ' && i < text.Length)
                i++;
            return i;
        }
    }
}
