using Client.EventClasses.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Client.Misc.Serialization
{
    public static class EventSerializer 
    {
        public static void SerializeUiEventsToCache(this Serializer serializer, List<UiEvent> events)
        {
            string mydoc = GetMyDocPath();
            CreateAppFolderInMyDoc(mydoc);
            serializer.SerializeList(events, mydoc + Properties.Resources.CACHE_XML_FILE);
        }

        private static void CreateAppFolderInMyDoc(string mydoc)
        {
            if (!Directory.Exists(mydoc + Properties.Resources.DOWNLOADS_FOLDER))
            {
                Directory.CreateDirectory(mydoc + Properties.Resources.DOWNLOADS_FOLDER);
            }
        }

        private static string GetMyDocPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public static void SerializeEventsBackground(this Serializer serializer, List<UiEvent> events)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                SerializeUiEventsToCache(serializer, events);
            }).Start();
        }

        public static List<UiEvent> DeserializeEventsFromCache(this Serializer serializer)
        {
            string mydoc = GetMyDocPath();
            string filepath = mydoc + Properties.Resources.CACHE_XML_FILE;
            var events = serializer.DeserializeList<UiEvent>(filepath);
            UpdateCachedEventsToApproved(events);
            return events;
        }

        private static void UpdateCachedEventsToApproved(List<UiEvent> events)
        {
            foreach (var e in events)
            {
                if ((e.EventData.Approver == null) && (e.EventData.IsApproved == null))
                {
                    e.EventData.IsApproved = true;
                }
            }
        }
    }
}
