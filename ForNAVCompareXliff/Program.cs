using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ForNAVCompareXliff
{
    class Program
    {
        static void AddTranslation(Dictionary<string, string> translations, XmlDocument doc, string language)
        {
            XmlNodeList nodes = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes;
            foreach (XmlNode node in nodes)
            {
                string id = node.Attributes["id"].Value;
                translations.Add(id, node.ChildNodes[1].InnerText + "@");
            }
        }

        static void AddDiff(Dictionary<string, string> translations, XmlDocument doc, string language)
        {
            XmlNodeList nodes = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes;
            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(doc.NameTable);

            for (int i = 0; i < translations.Count; i++)
            {
                string key = translations.ElementAt(i).Key;
                if (doc.SelectSingleNode($"//*[@id='{key}']") == null)
                    translations[key] += language + ",";
            }
        }
        static void Main(string[] args)
        {
            if (args.Length < 2)
                Console.WriteLine("usage: XliffCompare <xliff directory> <output path>");

            Dictionary<string, XmlDocument> docs = new Dictionary<string, XmlDocument>();
            XmlDocument enus = null;
            foreach (var f in Directory.EnumerateFiles(args[0]))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(f);
                string name = Path.GetFileNameWithoutExtension(f);
                string key = name.Substring(name.Length - 5);
                docs.Add(key, doc);
                if (key == "en-US")
                    enus = doc;
            }
            Dictionary<string, string> translations = new Dictionary<string, string>();
            AddTranslation(translations, enus, "en-US");
            foreach (var t in docs)
                if (t.Value != enus)
                    AddDiff(translations, t.Value, t.Key);
            StreamWriter s = File.CreateText(args[1]);
            foreach (var translation in translations)
                s.WriteLine($"{translation.Key}:{translation.Value}");
            s.Close();
        }
    }

}
