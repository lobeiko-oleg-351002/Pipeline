using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using static Client.MainForm;

namespace Client.Misc
{
    public static class Signal
    {
        public static void PlaySignalAccordingToEventConfigValue()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_SOUND_EVENT))
            {
                SystemSounds.Beep.Play();
            }
        }

        public static void PlaySignalAccordingToStatusConfigValue()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_SOUND_STATUS))
            {
                SystemSounds.Beep.Play();
            }
        }
    }
}
