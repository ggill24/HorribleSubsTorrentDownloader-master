using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;
using System.IO;

namespace HorribleSubsTorrentDownloader.Classes
{
    class JsHandler
    {
        JsonSchemaGenerator generator;
        JsonSchema jsSchema;
        public JsHandler()
        {

            //Generate the JsSchema using the properties from the struct "Anime"
            generator = new JsonSchemaGenerator();
            jsSchema = generator.Generate(typeof(Anime));
        }

        public void writeJsonToFile(string path)
        {
            if (doesJsonFileExist(path)) { File.Delete(path); }

            //Create the json file and write to it
            using (StreamWriter sw = File.CreateText(path))
            using (JsonTextWriter jsTxtWriter = new JsonTextWriter(sw))
            {
                jsSchema.WriteTo(jsTxtWriter);
            }
        }
        public bool doesJsonFileExist(string path)
        {
            if (File.Exists(Path.Combine(Dependencies.AppDirectoryPath + "list.json"))) return true;
            return false;
        }
    }
}
