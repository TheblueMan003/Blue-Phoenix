using JSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluePhoenix
{
    public abstract class Condition
    {
        public abstract String[] GetSetup();
        public abstract String GetCondition();
        public abstract String[] GetClear();
        public abstract void Invert();
    }
    public class SingleJavaCondition : Condition
    {
        private string setup;
        private string condition;
        private string clear;
        private bool if_;

        public SingleJavaCondition(string condition, string setup="", string clear="", bool if_ = true)
        {
            this.setup = setup;
            this.condition = condition;
            this.clear = clear;
            this.if_ = if_;
        }

        public override string[] GetClear()
        {
            return new string[] { clear };
        }

        public override string GetCondition()
        {
            if (condition == "") return "";
            return if_? $"if {condition} ":$"unless {condition} ";
        }

        public override string[] GetSetup()
        {
            return new string[] { setup };
        }

        public override void Invert()
        {
            if_ = !if_;
        }
    }
    public class JavaConstantCondition : Condition
    {
        private bool if_;

        public JavaConstantCondition(bool if_)
        {
            this.if_ = if_;
        }

        public override string[] GetClear()
        {
            return new string[0];
        }

        public override string GetCondition()
        {
            return if_?"=$=TRUE=$=": "=$=FALSE=$=";
        }

        public override string[] GetSetup()
        {
            return new string[0];
        }

        public override void Invert()
        {
            if_ = !if_;
        }
    }
    public class JavaConditionList : Condition
    {
        private List<Condition> conditions;
        private bool if_;

        public JavaConditionList(List<Condition> conditions)
        {
            this.conditions = conditions;
            this.if_ = false;
        }
        public override string[] GetClear()
        {
            if (conditions.Count == 0) return new string[0];
            return conditions.SelectMany(x => x.GetClear()).ToArray();
        }

        public override string GetCondition()
        {
            var filter = conditions.Select(x => x.GetCondition()).Where(x => x != "").ToList();
            if (filter.Count == 0) return "";
            if (if_)
            {
                return filter.Aggregate((x, y) => $"{x}{y}");
            }
            else
            {
                return filter.Aggregate((x, y) => $"{x}{y}");
            }
        }

        public override string[] GetSetup()
        {
            if (conditions.Count == 0) return new string[0];
            if (if_)
            {
                return conditions.SelectMany(x => x.GetSetup()).ToArray();
            }
            else
            {
                return conditions.SelectMany(x => x.GetSetup()).ToArray();
            }
        }

        public override void Invert()
        {
            if_ = !if_;
            conditions.ForEach(x => x.Invert());
        }
    }

    public static class StringArrayExtension
    {
        public static string LinesToText(this IEnumerable<string> str, string joint, bool atEnd = false)
        {
            if (str.Count() > 0)
            {
                return str.Aggregate((x, y) => $"{x}{joint}{y}" + (atEnd?joint:""));
            }
            else
            {
                return "";
            }
        }
    }
}
