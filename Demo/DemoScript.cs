using HardCodeDev.EmotionGestureBlinkUnity;
using UnityEngine;
using UnityEngine.UI;

namespace HardCodeDev.Examples
{
    public class DemoScript : MonoBehaviour
    {
        [SerializeField] private Text _emotionText, _gestureText, _blinkText;

        // Optimization is good ash
        private void Update()
        {
            if (ComputerVisionReceiver.IsBlinked) _blinkText.text = "Blink: Blinked";
            else _blinkText.text = "Blink: No blinking detected";

            if (ComputerVisionReceiver.CurrentGesture != GestureType.None) 
            {
                if (ComputerVisionReceiver.CurrentGesture == GestureType.Like) _gestureText.text = "Gesture: Like";
                else if (ComputerVisionReceiver.CurrentGesture == GestureType.Dislike) _gestureText.text = "Gesture: Dislike";
            }
            else _gestureText.text = "Gesture: None";

            if (ComputerVisionReceiver.CurrentEmotion != EmotionType.Neutral)
            {
                if (ComputerVisionReceiver.CurrentEmotion == EmotionType.Fear) _emotionText.text = "Emotion: Fear";
                else if (ComputerVisionReceiver.CurrentEmotion == EmotionType.Angry) _emotionText.text = "Emotion: Angry";
                else if (ComputerVisionReceiver.CurrentEmotion == EmotionType.Happy) _emotionText.text = "Emotion: Happy";
                else if (ComputerVisionReceiver.CurrentEmotion == EmotionType.Sad) _emotionText.text = "Emotion: Sad";
            }
            else _emotionText.text = "Emotion: Neutral or None";
        }
    }
}
