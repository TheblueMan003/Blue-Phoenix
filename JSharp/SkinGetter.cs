using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BluePhoenix
{
    class SkinGetter
    {
        public static void GetSkin(string name, string filename)
        {
            var client = new WebClient();
            try
            {
                string response = client.DownloadString($"https://api.mojang.com/users/profiles/minecraft/{name}");
                JObject objres = JObject.Parse(response);
                string user = client.DownloadString($"https://sessionserver.mojang.com/session/minecraft/profile/{objres["id"]}");
                JObject objuser = JObject.Parse(user);
                JObject texture_info = find_texture_info((JArray)objuser["properties"]);
                string skin_url = texture_info["textures"]["SKIN"]["url"].ToString();
                client.DownloadFile(skin_url, filename);
            }
            catch
            { }
        }
        public static JObject find_texture_info(JArray properties) {
            foreach(JObject prop in properties) {
                if (prop["name"].ToString() == "textures"){
                    return JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(prop["value"].ToString())));
                }
            }
            return null;
        }
    }
}
