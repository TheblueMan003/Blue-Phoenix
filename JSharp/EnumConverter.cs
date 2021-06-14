using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace JSharp
{
    public static class EnumConverter
    {
        public static Compiler.Enum GetEnum(string enums, string text, string type, bool final, bool isPrivate)
        {
            Compiler.Enum en;
            if (type.ToLower() == "init")
            {
                en = GetEnumINT(enums, text, final);
            }
            else if (type.ToLower() == "csv")
            {
                en = GetEnumCSV(enums, text, final);
            }
            else
            {
                throw new Exception($"Unknown format: {type}");
            }
            en.isPrivate = isPrivate;
            en.privateContext = Compiler.context.GetVar();
            return en;
        }

        private static string GetType(string value, string type)
        {
            if (type == "")
            {
                try
                {
                    type = Compiler.getExprType(Compiler.smartExtract(value)).ToString().ToLower();
                }
                catch
                {
                    type = "json";
                }
            }
            else
            {
                try
                {
                    string nType = Compiler.getExprType(Compiler.smartExtract(value)).ToString().ToLower();
                    if (type != nType)
                    {
                        if (type == "int" && nType == "float")
                        {
                            type = "float";
                        }
                        else
                        {
                            type = "json";
                        }
                    }
                }
                catch
                {
                    type = "json";
                }
            }
            return type;
        }
        private static Compiler.Enum GetEnumINT(string name, string text, bool final)
        {
            List<string> values = new List<string>();
            string type = "";
            foreach (string line in text.Split('\n'))
            {
                if (line.Contains('=') && !line.StartsWith("#") && !line.StartsWith("//"))
                {
                    string[] fields = Compiler.smartSplit(line, '=');
                    type = GetType(fields[1], type);
                    if (type == "json" && !fields[1].Contains("\""))
                    {
                        fields[1] = "\"" + Compiler.smartExtract(fields[1]) + "\"";
                        values.Add(Compiler.smartExtract(fields[0]).ToLower() + "(" + fields[1] + ")");
                    }
                    else
                        values.Add(Compiler.smartExtract(fields[0]).ToLower() + "(" + Compiler.smartExtract(fields[1]) + ")");
                }
            }
            return new Compiler.Enum(name, new string[] { type + " value" }, values.ToArray(), final);
        }
        private static Compiler.Enum GetEnumCSV(string name, string text, bool final)
        {
            List<string> values = new List<string>();
            List<string> fields = new List<string>();
            List<string> types = new List<string>();

            int i = 0;
            foreach (string line in text.Split('\n'))
            {
                string[] cells = Compiler.smartSplitJson(line, ';');
                if (cells.Length > 0)
                {
                    if (i == 0)
                    {
                        for (int j = 1; j < cells.Length; j++)
                        {
                            fields.Add(cells[j].ToLower());
                            types.Add("");
                        }
                    }
                    else
                    {
                        if (cells.Length > 1)
                        {
                            string value = cells[0] + "(";
                            for (int j = 1; j < cells.Length; j++)
                            {
                                if (j > types.Count)
                                {
                                    Compiler.GlobalDebug("CSV Error at line " + i.ToString() + " index: " + j.ToString() + "/" + types.Count.ToString(), Color.Red);
                                }
                                else
                                {
                                    value += cells[j] + ",";
                                    types[j - 1] = GetType(cells[j], types[j - 1]);
                                }
                            }
                            value = value.Substring(0, value.Length - 1) + ")";

                            values.Add(value);
                        }
                        else
                        {
                            values.Add(cells[0]);
                        }
                    }
                    i++;
                }
            }
            for (int j = 0; j < fields.Count; j++)
            {
                fields[j] = types[j] + " " + fields[j];
            }
            if (fields.Count > 0)
            {
                return new Compiler.Enum(name, fields.ToArray(), values.ToArray(), final);
            }
            else
            {
                return new Compiler.Enum(name, values.ToArray(), final);
            }
        }
    }
}
