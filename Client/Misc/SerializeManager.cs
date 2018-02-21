using Client.EventClasses;
using Client.EventClasses.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Client.Misc
{
    public static class SerializeEventManager
    {
        public static void SerializeUiEventsToCache(List<UiEvent> events)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<UiEvent>));
            try
            {
                string mydoc = GetMyDocPath();
                CreateAppFolderInMyDoc(mydoc);
                using (FileStream stream = new FileStream(mydoc + Properties.Resources.CACHE_XML_FILE, FileMode.Create))
                {
                    serializer.Serialize(stream, events);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        private static void CreateAppFolderInMyDoc(string mydoc)
        {
            if (!Directory.Exists(mydoc + Properties.Resources.DOWNLOADS_FOLDER))
            {
                Directory.CreateDirectory(mydoc + Properties.Resources.DOWNLOADS_FOLDER);
            }
        }

        public static void SerializeEventsBackground(List<UiEvent> events)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                SerializeUiEventsToCache(events);
            }).Start();
        }

        public static List<UiEvent> DeserializeEventsFromCache()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<UiEvent>));
            try
            {
                string mydoc = GetMyDocPath();
                using (Stream stream = File.Open(mydoc + Properties.Resources.CACHE_XML_FILE, FileMode.Open))
                {
                    return (List<UiEvent>)serializer.Deserialize(stream);
                }
            }
            catch (IOException ex)
            {
                return new List<UiEvent>();
            }
        }

        private static string GetMyDocPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
