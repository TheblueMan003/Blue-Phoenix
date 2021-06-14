namespace JSharp
{
    public class DataPackMeta
    {
        public Pack pack = new Pack();

        public DataPackMeta(string description, int format = 6)
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
