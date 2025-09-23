import sys, os, time, shutil, signal, cv2
import mediapipe as mp
from fer import FER
from mediapipe.tasks import python
from mediapipe.tasks.python.vision import FaceLandmarkerOptions, RunningMode, FaceLandmarker
from mediapipe.tasks.python.vision import GestureRecognizer, GestureRecognizerOptions, RunningMode as GestureRunningMode

TEMP_DIR_PATH = r"C:\ProgramData\HardCodeDev\FlFiles"
DETECTION_RESULT = None
old_blink_data = 0

os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'
os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'
sys.stdout.reconfigure(line_buffering=True)

camera_id = 0
if len(sys.argv) > 1:
    try:
        camera_id = int(sys.argv[1])
    except ValueError:
        print("Invalid camera id, using default 0")

cap = cv2.VideoCapture(camera_id)
if not cap.isOpened():
    print(f"ERROR: Cannot open camera {camera_id}")
    sys.exit(1)
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 1280)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 960)

emotion_detector = FER()

BaseOptions = mp.tasks.BaseOptions
gesture_options = GestureRecognizerOptions(
    base_options=BaseOptions(model_asset_path="gesture_recognizer.task"),
    running_mode=GestureRunningMode.VIDEO
)
gesture_recognizer = GestureRecognizer.create_from_options(gesture_options)

os.makedirs(TEMP_DIR_PATH, exist_ok=True)
temp_model_path = os.path.join(TEMP_DIR_PATH, 'FL.task')
if not os.path.exists(temp_model_path):
    shutil.copy("FL.task", temp_model_path)

def save_result(result, unused_output_image=None, timestamp_ms=None):
    global DETECTION_RESULT
    DETECTION_RESULT = result

face_landmarker_options = FaceLandmarkerOptions(
    base_options=python.BaseOptions(model_asset_path=temp_model_path),
    running_mode=RunningMode.LIVE_STREAM,
    num_faces=1,
    min_face_detection_confidence=0.5,
    min_face_presence_confidence=0.5,
    min_tracking_confidence=0.5,
    output_face_blendshapes=True,
    result_callback=save_result
)
face_landmarker = FaceLandmarker.create_from_options(face_landmarker_options)

def cleanup(signum=None, frame=None):
    face_landmarker.close()
    cap.release()
    cv2.destroyAllWindows()
    sys.exit(0)

signal.signal(signal.SIGTERM, cleanup)
signal.signal(signal.SIGINT, cleanup)

while True:
    ret, frame = cap.read()
    if not ret:
        print("WARNING: No frame captured, retrying...")
        time.sleep(0.1)
        continue

    try:
        emotions = emotion_detector.detect_emotions(frame)
        for emotion in emotions:
            dominant_emotion = emotion["emotions"]
            emotion_name = max(dominant_emotion, key=dominant_emotion.get)
            print(f"{emotion_name}")

        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb_frame)
        gesture_result = gesture_recognizer.recognize_for_video(mp_image, int(cap.get(cv2.CAP_PROP_POS_MSEC)))
        if gesture_result.gestures and len(gesture_result.gestures[0]) > 0:
            top_gesture = gesture_result.gestures[0][0]
            print(f"{top_gesture.category_name}")
        else:
            print("None")

        face_landmarker.detect_async(mp_image, timestamp_ms=int(time.time() * 1000))
        if DETECTION_RESULT:
            blendshapes = DETECTION_RESULT.face_blendshapes
            if blendshapes and len(blendshapes) > 0:
                blink_score = round((blendshapes[0][9].score + blendshapes[0][10].score) / 2, 2)
                if blink_score != old_blink_data:
                    print(f"{blink_score}")
                    old_blink_data = blink_score
            else:
                if old_blink_data != -1:
                    print("-1")
                    old_blink_data = -1

    except Exception as e:
        print(f"ERROR in loop: {e}")
    time.sleep(0.1)