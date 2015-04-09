using IMVR.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMVR.Indexer
{
    public class AtlasManager
    {
        private TextureAtlas atlas;
        private object internalAtlasLock = new object();
        private static object atlasLock = new object();
        public int TileSize = 128;
        public int XPowerOf2 = 11;
        public int YPowerOf2 = 11;

        public AtlasTicket GetTicket(string url)
        {
            lock (internalAtlasLock)
            {
                CheckAtlas();
                return atlas.Add(url);
            }
        }


        public void Save()
        {
            lock (internalAtlasLock)
            {
                if (atlas != null)
                {
                    var privateAtlas = atlas;
                    atlas = null;
                    AbstractWorker.StartNew(delegate
                    {
                        Console.WriteLine("Start writing atlas");
                        privateAtlas.Generate();
                    });

                    lock (atlasLock)
                    {
                        var db = Options.Instance.DB;
                        db.Atlases.Add(privateAtlas.Atlas);
                    }
                }
            }
        }

        private void CheckAtlas()
        {
            if (atlas == null || atlas.IsFull)
            {
                Save();

                // First time -> create new one
                // 2^11 = 2048
                atlas = new TextureAtlas(Cache.GetPath(), 1 << XPowerOf2, 1 << YPowerOf2, TileSize);
            }
        }

    }
}
