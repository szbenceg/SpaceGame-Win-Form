using SpaceGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Persistance
{
    public interface IFFileManager
    {
        String SaveGame(SpaceWord spaceWord);

        SpaceWord LoadGame(String path);

    }
}
