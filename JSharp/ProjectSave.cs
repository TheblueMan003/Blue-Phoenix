using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp
{
    public class ProjectSave
    {
        public string projectName;
        public string datapackDirectory;
        public bool offuscate;
        public bool isLibrary;
        public string description = "Made With BluePhoenix";
        
        public FileSave[] files;
        public FileSave[] resources;
        public Dictionary<string, Dictionary<string, TagsList>> TagsList;
        public Dictionary<string, Dictionary<string, TagsList>> mcTagsList;
        public List<string> compileOrder;
        public ProjectVersion version = new ProjectVersion();

        public class FileSave
        {
            public string name;
            public string content;
            public int index;

            public FileSave()
            {

            }
            public FileSave(string name, string content, int index)
            {
                this.name = name;
                this.content = content;
                this.index = index;
            }
        }
    }
    public class ProjectVersion
    {
        public int major = 1;
        public int minor = 0;
        public int patch = 0;
        public int build = 0;

        public void Build() { build++; }
        public override string ToString()
        {
            return major.ToString()+"."+minor.ToString()+"."+patch.ToString()+"."+build.ToString();
        }
    }
}
