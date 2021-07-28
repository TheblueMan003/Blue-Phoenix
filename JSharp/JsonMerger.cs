using JSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BluePhoenix
{
    public static class JsonMerger
    {
        public static JsonElement GetObject(string jsonstring)
        {
            jsonstring = Compiler.smartExtract(jsonstring);
            
            if (jsonstring.StartsWith("{"))
            {
                return new JsonObject(jsonstring);
            }
            if (jsonstring.StartsWith("["))
            {
                return new JsonArray(jsonstring);
            }
            return new JsonUnit(jsonstring);
        }
    }
    public abstract class JsonElement
    {
        public abstract void Merge(JsonElement element, bool keepOld);
    }
    public class JsonObject: JsonElement
    {
        public Dictionary<string, JsonElement> values = new Dictionary<string, JsonElement>();

        public JsonObject(string element)
        {
            string[][] block = Compiler.smartSplit(Compiler.getCodeBlock(element), ',')
                                        .Select(x => Compiler.smartSplitJson(x, ':').Select(y => Compiler.smartExtract(y)).ToArray())
                                        .ToArray();
            foreach(string[] kv in block)
            {
                values[kv[0]] = JsonMerger.GetObject(kv[1]);
            }
        }

        public override void Merge(JsonElement element, bool keepOld)
        {
            JsonObject other = (JsonObject)element;
            foreach(string key in other.values.Keys)
            {
                if (!values.ContainsKey(key))
                {
                    values.Add(key, other.values[key]);
                }
                else
                {
                    values[key].Merge(other.values[key], keepOld);
                }
            }
        }
        public override string ToString()
        {
            if (values.Count > 0)
                return "{" + values.Select(x => x.Key.ToString() + ":" + x.Value.ToString()).Aggregate((x, y) => x + ", " + y) + "}";
            else
                return "{}";
        }
    }
    public class JsonArray: JsonElement
    {
        public List<JsonElement> elements = new List<JsonElement>();

        public JsonArray(string element)
        {
            string st = Compiler.smartExtract(element);
            st = st.Substring(1, st.Length - 2);
            elements.AddRange(Compiler.smartSplitJson(st, ',').Select(x => JsonMerger.GetObject(x)));
        }

        public override void Merge(JsonElement element, bool keepOld)
        {
            elements.AddRange(((JsonArray)element).elements);
        }
        public override string ToString()
        {
            if (elements.Count > 0)
                return "[" + elements.Select(x => x.ToString()).Aggregate((x, y) => x + ", " + y) + "]";
            else
                return "[]";
        }
    }
    public class JsonUnit : JsonElement
    {
        public string value;

        public JsonUnit(string value){
            this.value = value;
        }

        public override void Merge(JsonElement element, bool keepOld)
        {
            if (!keepOld)
            value = ((JsonUnit)element).value;
        }

        public override string ToString()
        {
            return value;
        }
    }
}
