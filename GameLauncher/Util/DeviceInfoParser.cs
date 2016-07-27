using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GameLauncher.Util
{
    class DeviceInfoParser
    {
        public List<string>[] Parse(string deviceInfo)
        {
            var deviceLists = new List<string>[2];

            // ==============================================================================================
            // for tests:
            //
            // deviceInfo = "[dshow @ 02457a60] DirectShow video devices\n[dshow @ 02457a60]  \"Blackmagic WDM Capture\"\n[dshow @ 02457a60]  \"Decklink Video Capture\"\n[dshow @ 02457a60] DirectShow audio devices\n[dshow @ 02457a60]  \"Decklink Audio Capture\"";
            //
            // ==============================================================================================

            var posVideoSection = deviceInfo.IndexOf("DirectShow video devices", StringComparison.Ordinal);
            var posAudioSection = deviceInfo.IndexOf("DirectShow audio devices", StringComparison.Ordinal);

            // if no "DirectShow video devices" or "DirectShow audio devices" section found:
            // return immediately
            if (posVideoSection < 0 || posAudioSection < 0)
            {
                return null;
            }

            var videoDeviceList = new List<string>();
            var audioDeviceList = new List<string>();

            // look for quoted text parts after '[dshow @ xxxxxx]'
            var re = new Regex(@"([\w\s]*)("".+"")");
            
            // more complicated version:
            //var re = new Regex(@"\[dshow @ \w+\]([\w\s]*)("".+"")");

            foreach (Match device in re.Matches(deviceInfo))
            {
                // there may be rows with alternative names of the same device
                if (device.Groups[1].Value.Contains("Alternative name"))
                {
                    continue;
                }
                if (device.Index > posAudioSection)
                {
                    audioDeviceList.Add(device.Groups[2].Value);
                }
                else if (device.Index > posVideoSection)
                {
                    videoDeviceList.Add(device.Groups[2].Value);
                }
            }

            deviceLists[0] = videoDeviceList;
            deviceLists[1] = audioDeviceList;

            return deviceLists;
        }
    }
}