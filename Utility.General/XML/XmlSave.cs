using System;
using System.IO;
using System.Xml.Serialization;

namespace Utility.General.XML
{
    public class XmlSave
    {
        public static void SaveData(object IClass, string filename)
        {
            StreamWriter writer = null;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer((IClass.GetType()));
                writer = new StreamWriter(filename);
                xmlSerializer.Serialize(writer, IClass);
            }
            finally
            {
                if (writer != null)
                    writer.Close();

                writer = null;
            }
        }
    }
}
