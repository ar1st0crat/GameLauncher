using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GameLauncher.Model;

namespace GameLauncher.Util
{
    class WebCamRecorder
    {
        private Process _recordProcess;
        private bool _isEnabled;
        
        // device names (can be enumerated by FFmpeg)
        private string _audioDevice;
        private string _videoDevice;

        public bool Initialize()
        {
            Directory.CreateDirectory("video");

            var infoProcess = new Process();
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "ffmpeg.exe",
                StandardErrorEncoding = Encoding.GetEncoding(65001),
                Arguments = " -list_devices true -f dshow -i dummy",
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            infoProcess.StartInfo = startInfo;
            infoProcess.Start();

            var info = infoProcess.StandardError.ReadToEnd();

            infoProcess.WaitForExit();
            infoProcess.Close();

            // ==============================================================================================
            // for future tests:
            //
            //info = "[dshow @ 02457a60] DirectShow video devices\n[dshow @ 02457a60]  \"Blackmagic WDM Capture\"\n[dshow @ 02457a60]  \"Decklink Video Capture\"\n[dshow @ 02457a60] DirectShow audio devices\n[dshow @ 02457a60]  \"Decklink Audio Capture\"";
            //
            // TODO: make 2 functions:
            //       string ParseAudioDevice(string info)
            //       string ParseVideoDevice(string info)
            //
            // ==============================================================================================

            var posVideoSection = info.IndexOf("DirectShow video devices", StringComparison.Ordinal);
            var posAudioSection = info.IndexOf("DirectShow audio devices", StringComparison.Ordinal);

            // no "DirectShow video devices" section:
            // return immediately
            if (posVideoSection < 0)
            {
                _isEnabled = false;
                return false;
            }

            var startPosVideoDeviceString = info.IndexOf('"', posVideoSection);
            var endPosVideoDeviceString = info.IndexOf('"', startPosVideoDeviceString + 1);

            // no "DirectShow video devices" section or
            // device name was found in "DirectShow audio devices" section: 
            // return immediately
            if (startPosVideoDeviceString < 0 ||
                posAudioSection <= startPosVideoDeviceString)
            {
                _isEnabled = false;
                return false;
            }
            
            _videoDevice = info.Substring(startPosVideoDeviceString, 
                endPosVideoDeviceString - startPosVideoDeviceString + 1);
            
            var startPosAudioDeviceString = info.IndexOf('"', posAudioSection);
            var endPosAudioDeviceString = info.IndexOf('"', startPosAudioDeviceString + 1);

            if (startPosAudioDeviceString > 0)
            {
                _audioDevice = info.Substring(startPosAudioDeviceString,
                    endPosAudioDeviceString - startPosAudioDeviceString + 1);
            }

            _isEnabled = true;

            return true;
        }

        /// <summary>
        /// Combine command line arguments for ffmpeg recording process
        /// TBD: Add more audio settings: 
        /// String.Format(" -f dshow -i audio={0} -ac 1 -ar 8000 \"{1}\"", _audioDevice, filename)
        /// 
        /// more examples:  
        /// ffmpeg -f dshow -r 25 -i 0  -ab 64 -ar 22050 -ac 1 output.avi
        /// ffmpeg -f dshow -i video="Integrated Camera" -c copy raw.avi -c:v libx264 -preset veryfast -crf 25 encoded.mp4
        /// </summary>
        /// <param name="filename">The full name of an output file</param>
        /// <returns>Command line arguments for ffmpeg recording process</returns>
        private string BuildCmdArguments(string filename)
        {
            // record only video:
            if (String.IsNullOrEmpty(_audioDevice))
            {
                return String.Format(" -f dshow -i video={0} -preset ultrafast -framerate 25 \"{1}\"",
                    _videoDevice, filename);
            }

            // record both video and audio:
            return String.Format(" -f dshow -i video={0}:audio={1} -preset ultrafast -framerate 25 \"{2}\"",
                _videoDevice, _audioDevice, filename);
        }

        public bool Start(Game game)
        {
            if (!_isEnabled || _recordProcess != null)
            {
                return false;
            }
            
            var filename = String.Format("{0}_{1}.avi",
                DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"),
                game.Name);

            // guarantee that the filename is valid
            filename = Path.GetInvalidFileNameChars()
                .Aggregate(filename, (current, c) => current.Replace(c, '_'));

            filename = @"video\" + filename;


            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "ffmpeg.exe",
                Arguments = BuildCmdArguments(filename),
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _recordProcess = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = startInfo
            };
            _recordProcess.Exited += RecordProcess_Exited;
            _recordProcess.Start();

            return true;
        }

        public void Stop()
        {
            if (_recordProcess == null)
            {
                return;
            }

            _recordProcess.StandardInput.WriteLine("q");
            _recordProcess.StandardInput.Close();
        }

        public void RecordProcess_Exited(object sender, EventArgs e)
        {
            _recordProcess.Exited -= RecordProcess_Exited;
            _recordProcess.Close();
            _recordProcess = null;
        }
    }
}