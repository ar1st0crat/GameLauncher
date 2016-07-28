using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GameLauncher.Util
{
    /// <summary>
    /// WebCam video and audio recorder based on FFmpeg
    /// </summary>
    class WebCamRecorder
    {
        private const string FFMPEG_PATH = "ffmpeg.exe";
        
        // device names (can be enumerated by FFmpeg)
        private List<string> _videoDeviceList;
        public List<string> VideoDeviceList 
        {
            get
            {
                Initialize();               // refresh list each time
                return _videoDeviceList;
            }
        }

        private List<string> _audioDeviceList;
        public List<string> AudioDeviceList
        {
            get
            {
                Initialize();               // refresh list each time
                return _audioDeviceList;
            }
        }

        public int SelectedAudioDevice { get; set; }
        public int SelectedVideoDevice { get; set; }

        // recording
        private Process _recordProcess;
        private bool _isEnabled;

        /// <summary>
        /// Read and parse multimedia device configuration using ffmpeg and setup WebCamRecorder
        /// </summary>
        /// <returns>
        /// true, if device configuration was read and parsed succesfully
        /// false, if ffmpeg path is not valid
        ///           or ffmpeg process could not be started
        ///           or device configuration could not be parsed.
        /// Note: if the list of video devices is empty, function disables recording, but returns true
        /// </returns>
        public bool Initialize()
        {
            _isEnabled = false;

            if (!File.Exists(FFMPEG_PATH))
            {
                return false;
            }

            Directory.CreateDirectory("video");

            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = FFMPEG_PATH,
                StandardErrorEncoding = Encoding.GetEncoding(65001),
                Arguments = " -list_devices true -f dshow -i dummy",
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var infoProcess = new Process { StartInfo = startInfo };
            try
            {
                infoProcess.Start();
            }
            catch (Exception)
            {
                infoProcess.Dispose();
                return false;
            }

            var deviceInfo = infoProcess.StandardError.ReadToEnd();

            infoProcess.WaitForExit();
            infoProcess.Close();
            
            var parser = new DeviceInfoParser();
            var deviceLists = parser.Parse(deviceInfo);

            if (deviceLists == null)
            {
                return false;
            }

            _videoDeviceList = deviceLists[0];
            _audioDeviceList = deviceLists[1];

            _isEnabled = true;

            if (_videoDeviceList.Count == 0)
            {
                _isEnabled = false;
            }

            return true;
        }

        /// <summary>
        /// Combine command line arguments for ffmpeg recording process
        /// 
        /// TBD: Add more audio settings: 
        /// String.Format(" -f dshow -i audio={0} -ac 1 -ar 8000 \"{1}\"", _audioDevice, filename)
        /// 
        /// more examples (perhaps, will be needed in the future):  
        /// ffmpeg -f dshow -r 25 -i 0  -ab 64 -ar 22050 -ac 1 output.avi
        /// ffmpeg -f dshow -i video="Integrated Camera" -c copy raw.avi -c:v libx264 -preset veryfast -crf 25 encoded.mp4
        /// </summary>
        /// <param name="filename">The full name of an output file</param>
        /// <returns>Command line arguments for ffmpeg recording process</returns>
        private string BuildCmdArguments(string filename)
        {
            var audioDevice = _audioDeviceList[SelectedAudioDevice];
            var videoDevice = _videoDeviceList[SelectedVideoDevice];

            // record only video:
            if (String.IsNullOrEmpty(audioDevice))
            {
                return String.Format(" -f dshow -i video={0} -preset ultrafast -framerate 25 \"{1}\"", videoDevice, filename);
            }

            // record both video and audio:
            return String.Format(" -f dshow -i video={0}:audio={1} -preset ultrafast -framerate 25 \"{2}\"", videoDevice, audioDevice, filename);
        }

        /// <summary>
        /// Start recording video (with or maybe without audio) using ffmpeg
        /// </summary>
        /// <param name="outputFilename">Fullname of the video file to record</param>
        /// <returns>
        /// true, if recording started succesfully
        /// false, if recorder is disabled 
        ///        or recorder is currently busy recording other file
        ///        or ffmpeg process could not be started
        /// </returns>
        public bool Start(string outputFilename)
        {
            if (!_isEnabled || _recordProcess != null)
            {
                return false;
            }
            
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = FFMPEG_PATH,
                Arguments = BuildCmdArguments(outputFilename),
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                _recordProcess = new Process
                {
                    EnableRaisingEvents = true,
                    StartInfo = startInfo
                };
                _recordProcess.Start();
                _recordProcess.Exited += RecordProcess_Exited;
            }
            catch (Exception)
            {
                if (_recordProcess != null)
                {
                    _recordProcess.Dispose();
                    _recordProcess = null;
                }
                return false;
            }

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