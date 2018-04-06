using Client.EventClasses.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Client.Misc.Serialization
{
    public class Serializer
    {
        public void SerializeList<T>(List<T> events, string outputPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<UiEvent>));
            try
            {
                using (FileStream stream = new FileStream(outputPath, FileMode.Create))
                {
                    serializer.Serialize(stream, events);
                }
            }
            catch (IOException ex)
            {
                //throw ex;
            }
        }

        public List<T> DeserializeList<T>(string inputPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            try
            {
                using (Stream stream = File.Open(inputPath, FileMode.OpenOrCreate))
                {
                    return (List<T>)serializer.Deserialize(stream);
                }
            }
            catch(Exception ex)
            {
                if (File.Exists(inputPath))
                {
                    File.Delete(inputPath);
                }
                return new List<T>();
            }
        }
    }
}
