using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IMVR.Commons;
using System.IO;
using ProtoBuf;

namespace IMVR.Commons.Tests
{
    [TestClass]
    public class SerializationTest
    {

        private IMDB db;
        private string tempFile;

        [TestInitialize]
        public void SetUp()
        {
            db = new IMDB();

            db.Folders.Add("My Folder");
            for (int i = 0; i < 1000; i++)
            {
                db.Images.Add(new Image() {
                    Path = "C:\\" + i + ".mp3"
                });

                db.Songs.Add(new Song()
                {
                    Path = "C:\\" + i + ".mp3"
                });
            }

            tempFile = Path.GetTempFileName();
        }

        [TestCleanup]
        public void TearDown()
        {
            System.IO.File.Delete(tempFile);
        }

        [TestMethod]
        public void ShouldSerialize()
        {
            using (var file = System.IO.File.OpenWrite(tempFile))
            {
                Serializer.Serialize<IMDB>(file, db);
            }

            // File size should be at least the size of [number of items]
            Assert.IsTrue(new FileInfo(tempFile).Length > (db.Images.Count + db.Songs.Count), "File length is too small!");
        }

        [TestMethod]
        public void ShouldDeserialize()
        {
            using (var file = new MemoryStream())
            {
                Serializer.Serialize<IMDB>(file, db);
                file.Position = 0;

                var clone = Serializer.Deserialize<IMDB>(file);

                Assert.AreEqual(db.Folders.Count, clone.Folders.Count);
                Assert.AreEqual(db.Images.Count, clone.Images.Count);
                Assert.AreEqual(db.Songs.Count, clone.Songs.Count);

                Assert.AreEqual(db.Images[0].Path, clone.Images[0].Path);
            }
        }

        [TestMethod]
        public void ShouldDeserializeInSuccession()
        {
            // Save a larger file and then a smaller
            db.Save(tempFile);

            db.Images.Clear();
            db.Folders.Clear();

            db.Save(tempFile);

            // Try to deserialize
            IMDB.FromFile(tempFile);
        }

        [TestMethod]
        public void ShouldDeepClone()
        {
            var clone = Serializer.DeepClone<IMDB>(db);

            Assert.AreEqual(db.Images.Count, clone.Images.Count);
            Assert.AreEqual(db.Songs.Count, clone.Songs.Count);
            Assert.AreEqual(db.Images[0].Path, clone.Images[0].Path);
        }
    }
}
