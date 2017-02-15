using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeuralNetwork.Readers
{
    public static class ImageDataReader
    {
        public static List<byte[]> ReadImageFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            using (var reader = new BigEndianReader(stream))
            {
                stream.Seek(0, SeekOrigin.Begin);

                var magicNum = reader.ReadInt32();
                var numberOfItems = reader.ReadInt32();
                var numberOfRows = reader.ReadInt32();
                var numberOfColumns = reader.ReadInt32();

                var imageSize = numberOfRows * numberOfColumns;

                var images = new List<byte[]>(numberOfItems);
                for (int i = 0; i < numberOfItems; i++)
                {
                    var image = Enumerable.Range(0, imageSize).Select(_ => reader.ReadByte()).ToArray();
                    images.Add(image);
                }
                return images;
            }
        }


        public static byte[] ReadLabels(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            using (var reader = new BigEndianReader(stream))
            {
                var magicNum = reader.ReadInt32();
                var numberOfItems = reader.ReadInt32();

                var labels = Enumerable.Range(0, numberOfItems).Select(_ => reader.ReadByte()).ToArray();
                return labels;
            }
        }

    }
}
