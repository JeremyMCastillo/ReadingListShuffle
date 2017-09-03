using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingListShuffle
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = GetFilePath("Input");
            int idColumnIndex = GetCollectionIDColumn();

            Dictionary<string, Queue<string[]>> collections = LoadCollections(filePath, idColumnIndex);
            if (collections.Count > 0)
            {
                filePath = GetFilePath("Output");
                ShuffleCollections(collections, filePath);
            }
            else
            {
                Console.WriteLine("Nothing to do, bye!");
            }
        }

        static string GetFilePath(string which)
        {
            Console.WriteLine(string.Format("Enter {0} File Path:", which));
            return Console.ReadLine();
        }

        static int GetCollectionIDColumn()
        {
            int result;
            for (string idColumn = string.Empty; !int.TryParse(idColumn, out result); idColumn = Console.ReadLine())
            {
                Console.WriteLine("Which Column Contains the Collection Inforamtion? Enter zero based index:");
            }
            return result;
        }

        static Dictionary<string, Queue<string[]>> LoadCollections(string filePath, int collectionIDIndex)
        {
            Dictionary<string, Queue<string[]>> collections = new Dictionary<string, Queue<string[]>>();
            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields().Select(field => "\"" + field + "\"").ToArray();
                    if (collections.ContainsKey(fields[collectionIDIndex]))
                    {
                        collections[fields[collectionIDIndex]].Enqueue(fields);
                    }
                    else
                    {
                        Queue<string[]> newQueue = new Queue<string[]>();
                        newQueue.Enqueue(fields);
                        collections.Add(fields[collectionIDIndex], newQueue);
                    }
                }
            }

            return collections;
        }

        static void ShuffleCollections(Dictionary<string, Queue<string[]>> collections, string outputFilePath)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            bool pickLarge = true;
            while (collections.Keys.Count > 0)
            {
                KeyValuePair<string, Queue<string[]>> element;
                if (pickLarge)
                {
                    int maxCount = collections.Max(pair => pair.Value.Count);
                    element = collections.First(pair => pair.Value.Count == maxCount);
                    pickLarge = false;
                }
                else
                {
                    // Get the next row from a random collection
                    int selectedIndex = rand.Next(0, collections.Count);
                    element = collections.ElementAt(selectedIndex);
                    pickLarge = true;
                }
                string[] entry = element.Value.Dequeue();
                File.AppendAllText(outputFilePath, string.Join(",", entry) + "\r\n");
                if (element.Value.Count <= 0)
                {
                    collections.Remove(element.Key);
                }
            }
        }
    }
}
