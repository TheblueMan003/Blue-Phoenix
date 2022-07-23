using Newtonsoft.Json;
using static JSharp.DataPackMeta.Pack.Filter;

namespace JSharp
{
    public class DataPackMeta
    {
        public Pack pack = new Pack();

        public DataPackMeta(string description, int format = 6, params Block[] blocks)
        {
            pack.description = description;
            pack.pack_format = format;
            pack.filter = new Pack.Filter(blocks);
        }

        public class Pack
        {
            public int pack_format = 6;
            public string description;
            public Filter filter;

            public class Filter {
                public Block[] block;

                public Filter(Block[] blocks)
                {
                    this.block = blocks;
                }

                public class Block
                {
                    [JsonProperty("namespace")]
                    public string _namespace;
                    public string path;

                    public Block(string _namespace, string path)
                    {
                        this._namespace = _namespace; 
                        this.path = path;
                    }
                }
            }
        }
    }

    public class ResourcePackMeta
    {
        public Pack pack = new Pack();

        public ResourcePackMeta(string description, int format = 6)
        {
            pack.description = description;
            pack.pack_format = format;
        }

        public class Pack
        {
            public int pack_format = 6;
            public string description;
        }
    }
}
