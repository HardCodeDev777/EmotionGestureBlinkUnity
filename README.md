![Unity](https://img.shields.io/badge/Unity-unity?logo=Unity&color=%23000000)
![C#](https://img.shields.io/badge/C%23-%23512BD4?logo=.NET)
![Python](https://img.shields.io/badge/Python-3776AB?logo=Python&logoColor=%23ffffff&labelColor=%233776AB)
![License](https://img.shields.io/github/license/HardCodeDev777/EmotionGestureBlinkUnity?color=%2305991d)
![Last commit](https://img.shields.io/github/last-commit/HardCodeDev777/EmotionGestureBlinkUnity?color=%2305991d)
![Tag](https://img.shields.io/github/v/tag/HardCodeDev777/EmotionGestureBlinkUnity)
![Top lang](https://img.shields.io/github/languages/top/HardCodeDev777/EmotionGestureBlinkUnity)

# EmotionGestureBlinkUnity 🎭

---

## 🚀 Overview

Want to bring real-time **emotion, gesture and blink detection** into your Unity game?  
Now you can! This tool lets you do it all without installing Python or having opened cmd windows.

&nbsp;

> Yeah, [Vigil](https://store.steampowered.com/app/3817090) had blink detection, but only that.  
> This project takes it much further.

---

## 📦 Installation

1. Download the latest `.unitypackage` from the **Releases** page.  
2. Drag & drop it into your Unity project.  
3. Check out the `Demo` folder for usage examples.

---

## 🛠️ How it works

The computer vision part was written in **Python** (see [cv.py](<src/Computer Vision/cv.py>)), and compiled into `cv.exe`.  
Unity starts it in `OnEnable()` like this:

```csharp
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
```

All Python output is redirected, so you never see console windows.

> [!TIP]  
> Want to add more emotions/gestures or debug logs? Just remove the redirect and `CreateNoWindow`.

&nbsp;

### 🔑 Log mapping

```csharp
private Dictionary<string, EmotionType> _emotions = new()
{
    { "happy", EmotionType.Happy },
    { "sad", EmotionType.Sad },
    { "angry", EmotionType.Angry },
    { "neutral", EmotionType.Neutral },
    { "surprise", EmotionType.Fear },
    { "fear", EmotionType.Fear }
};

private Dictionary<string, GestureType> _gestures = new()
{
    { "Thumb_Up", GestureType.Like },
    { "Thumb_Down", GestureType.Dislike },
    { "None", GestureType.None }
};
```

The strings match what `cv.exe` outputs.  
If you want to expand recognition, update these dictionaries.

&nbsp;

### 🔄 Thread loop

```csharp
if (_emotions.ContainsKey(text)) CurrentEmotion = _emotions[text];
else if (_gestures.ContainsKey(text)) CurrentGesture = _gestures[text];
else if (float.TryParse(text, NumberStyles.Float, provider, out num)) IsBlinked = num > _blinkingThreashold;
else Debug.LogError($"Unknown log: {text}");
```

&nbsp;

### 🔚 Cleanup

```csharp
if (!_pythonProcess.HasExited)
{
    _pythonReader?.Close();
    _pythonProcess?.Kill();
}
```

&nbsp;

### 📊 Public values

```csharp
public static bool IsBlinked { get; private set; }
public static EmotionType CurrentEmotion { get; private set; } = EmotionType.Neutral;
public static GestureType CurrentGesture { get; private set; } = GestureType.None;
```

---

## 💳 Credits

Big thanks to:  
- [ipraveenkmr/python-open-cv-projects](https://github.com/ipraveenkmr/python-open-cv-projects) (base for gestures & emotions)  
- Vigil devs for the blink detection idea  
- AI for some Python help (I dob't like this language, but I had no choice😅)

---

## 📄 License

This project is under the **MIT License**.  
See [LICENSE](LICENSE) for details.

---

## 👨‍💻 Author

**HardCodeDev**  
- [GitHub](https://github.com/HardCodeDev777)  
- [Itch.io](https://hardcodedev.itch.io/)  

---

> 💬 Got feedback, found a bug, or want to contribute? Open an issue or fork the repo!
