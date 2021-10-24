using SpaceGame.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Persistance
{
    public class Persistance : IFFileManager
    {
        public String SaveGame(SpaceWord spaceWord) {
            return Newtonsoft.Json.JsonConvert.SerializeObject(spaceWord);
        }

        public SpaceWord LoadGame(String path)
        {
            String fileContent = string.Empty;

            using (StreamReader reader = new StreamReader(path))
            {
                fileContent = reader.ReadToEnd();
            }
            
            return Newtonsoft.Json.JsonConvert.DeserializeObject<SpaceWord>(fileContent);
        }
    }
}
