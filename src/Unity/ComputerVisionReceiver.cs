using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace HardCodeDev.EmotionGestureBlinkUnity
{
    public class ComputerVisionReceiver : MonoBehaviour
    {
        public static bool IsBlinked { get; private set; }
        public static EmotionType CurrentEmotion { get; private set; } = EmotionType.Neutral;
        public static GestureType CurrentGesture { get; private set; } = GestureType.None;

        [SerializeField] private int _cameraId;
        [SerializeField] private float _blinkingThreashold;

        private Process _pythonProcess;
        private StreamReader _pythonReader;

        private Dictionary<string, EmotionType> _emotions = new Dictionary<string, EmotionType>
        {
            { "happy", EmotionType.Happy },
            { "sad", EmotionType.Sad },
            { "angry", EmotionType.Angry },
            { "neutral", EmotionType.Neutral },
            // Usually cv recognize "fear" as "surprise"
            { "surprise", EmotionType.Fear },
            { "fear", EmotionType.Fear }
        };

        private Dictionary<string, GestureType> _gestures = new Dictionary<string, GestureType>
        {
            { "Thumb_Up", GestureType.Like },
            { "Thumb_Down", GestureType.Dislike },
            { "None", GestureType.None }
        };

        private void OnEnable() => StartPython();

        private void OnDisable() => EndPython();

        private void StartPython()
        {
            var path = Application.dataPath + "\\StreamingAssets";

            _pythonProcess = new Process();
            _pythonProcess.StartInfo.WorkingDirectory = path;
            _pythonProcess.StartInfo.FileName = path + "\\cv.exe";
            _pythonProcess.StartInfo.Arguments = _cameraId.ToString();
            _pythonProcess.StartInfo.UseShellExecute = false;
            _pythonProcess.StartInfo.CreateNoWindow = true;
            _pythonProcess.StartInfo.RedirectStandardOutput = true;
            _pythonProcess.Start();
            _pythonReader = _pythonProcess.StandardOutput;

            // Idk for what it is tbh
            var provider = CultureInfo.CreateSpecificCulture("en-GB");

            new Thread(() =>
            {
                while (!_pythonProcess.HasExited)
                {
                    var text = _pythonReader.ReadLine();
                    if (text != null)
                    {
                        float num;

                        if (_emotions.ContainsKey(text)) CurrentEmotion = _emotions[text];
                        else if (_gestures.ContainsKey(text)) CurrentGesture = _gestures[text];
                        else if (float.TryParse(text, NumberStyles.Float, provider, out num)) IsBlinked = num > _blinkingThreashold ? true : false;
                        else Debug.LogError($"Unknown log: {text}");
                    }
                    Thread.Sleep(1);
                }
            }).Start();
        }

        private void EndPython()
        {
            if (!_pythonProcess.HasExited)
            {
                _pythonReader?.Close();
                _pythonProcess?.Kill();
            }
        }
    }

    public enum EmotionType
    {
        Happy,
        Sad,
        Angry,
        Neutral,
        Fear
    }

    public enum GestureType
    {
        Like,
        Dislike,
        None
    }

}