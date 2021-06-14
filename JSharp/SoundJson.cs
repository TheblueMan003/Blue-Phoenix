using System.Collections.Generic;

namespace BluePhoenix
{
    public class SoundJson
    {
        public List<Sound> sounds;
    }

    public class Sound
    {
        public string name = "";
        public float volume = 1;
        public float pitch = 1;
        public int weight = 1;
        public bool stream = false;
        public int attenuation_distance = 16;
        public bool preload = false;
        public string type = "sound";
    }
}
