using System.Collections.Generic;

namespace JSharp
{
    public class TagsList
    {
        public List<string> values = new List<string>();

        public TagsList() { }

        public TagsList(List<string> values)
        {
            this.values = values;
        }

        public void SetList(List<string> values)
        {
            this.values = values;
        }

        public TagsList ToFunctions(string project)
        {
            TagsList tl = new TagsList(new List<string>());
            foreach (string value in values)
            {
                tl.values.Add(project + ":" + value);
            }
            return tl;
        }
    }
}
