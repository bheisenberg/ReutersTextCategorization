using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;

    public class XmlParser
    {
        string inputFile = "Resources/reut2-000.sgm";

        public XmlParser()
        {
            Parse(inputFile);
        }

        private void Parse(string input)
        {
            var text = File.ReadAllText(input);
            var rootedXML = "<root>" + text + "</root>";
            XDocument xdoc = null;
            XmlReaderSettings settings = new XmlReaderSettings
            {
                CheckCharacters = false,
                XmlResolver = null
            };
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(rootedXML), settings))
            {
                xdoc = XDocument.Load(xmlReader);
            }

            foreach (var parent in xdoc.Elements())
            {
                foreach (var childElement in parent.Elements())
                {
                    Debug.WriteLine("ELEMENT: " + childElement.Name);
                    var data = from element in childElement.Elements()
                               where element.Name == "TEXT"
                               select element;
                    Debug.WriteLine(data.First().Value);
                }
            }



            /*var data = from element in xdoc.Elements()
                       select element;

            foreach (var item in data)
            {
                Debug.WriteLine("ITEM: "+item.Value);
            }*/
            /*XmlReader xmlReader = XmlTextReader.Create(inputFile);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CheckCharacters = false;
            
            settings.XmlResolver = null;

            XmlReader reader = XmlReader.Create(inputFile, settings);

            //reader.MoveToContent();
            int count = 100;
            while (reader.Read() && --count > 0)
            {
                if(reader.NodeType == XmlNodeType.Element)
                {

                }
                //if (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.)
                //{
                Debug.WriteLine(reader.NodeType);
                //}
            }*/


        }
    }
}