using Mediapipe.Unity.HandTracking;
using UnityEngine;
using UnityEngine.Windows.WebCam;
using static P307.Shared.Const307;

namespace P307.Runtime.ComputerVision
{
	[DisallowMultipleComponent]
	public sealed class WebcamHandler : MonoBehaviour
	{
		/*// A wide video capture is important, as it gives the user more horizontal freedom of movement.
		[Header("Hands")]
		[SerializeField] HandDetector handDetector;
		[SerializeField, Range(ONE, TWO)] int maxHands = ONE;
		[SerializeField, Range(ZERO, ONE)] float minDetectionConfidence = .8f;
		[SerializeField, Range(ZERO, ONE)] float minTrackingConfidence = HALF;
		[SerializeField] HandTrackingGraph.ModelComplexity handModelComplexity = HandTrackingGraph.ModelComplexity.Full;
		
		[Header("Settings")]
		[SerializeField, Min(ZERO)] int frameWidth = 1920;
		[SerializeField, Min(ZERO)] int frameHeight = 1080;
		[SerializeField] string windowTitle = "Image";
		
		[Space(5), Header("Video Capture")]
		[SerializeField] int cameraIndex = ZERO;
		[SerializeField] bool runCapture = true;
		
		VideoCapture videoCapture;
		
		const int ESC_KEY = 27;

		void Awake()
		{
			if (handDetector == null)
				handDetector = FindFirstObjectByType<HandDetector>();
			handDetector.MaxHands = maxHands;
			handDetector.ModelComplexity = handModelComplexity;
			handDetector.MinDetectionConfidence = minDetectionConfidence;
			handDetector.MinTrackingConfidence = minTrackingConfidence;
		}

		void Start()
		{
			videoCapture = new VideoCapture(cameraIndex);
			videoCapture.Set(CapProp.FrameWidth, frameWidth);
			videoCapture.Set(CapProp.FrameHeight, frameHeight);
		}

		bool DisplayVideoCapture(string windowName, int delay, Mat mat)
		{
			CvInvoke.Imshow(windowName, mat);
			var key = CvInvoke.WaitKey(delay);

			if (key is not ESC_KEY)
				return true;
			
			CvInvoke.DestroyWindow(windowName);
			Debug.LogWarning("INFO: User pressed ESC to stop the video capture.");
			return false;
		}

		void Update()
		{
			if (runCapture is false)
				return;

		}*/

		/*
		 *
		 * import typing
           from typing import Any

           import cv2
           import socket
           from cvzone.HandTrackingModule import HandDetector
           from numpy import ndarray, dtype, generic

           def convert_to_unity_values(
                   data_list: list,
                   hand: dict[str, typing.Union[list[list[int]], tuple[int, int, int, int], tuple[int, int]]]
           ) -> list:
               list_tag = 'lmList'
               landmarks = hand[list_tag]

               # Landmark count: 21
               # Landmark values: (x,y,z) * count
               for landmark in landmarks:
                   x_in_unity = landmark[0]
                   y_in_unity = height - landmark[1]  # In a 1x1 square, y=0: top-left, y=1: bottom-left. Unity is opposite.
                   z_in_unity = landmark[2]
                   data_list.extend([x_in_unity, y_in_unity, z_in_unity])
               return data_list


           # COMMUNICATION
           soc = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
           port = 30707
           local_ip = "127.0.0.1"
           server_address_port = (local_ip, port)

           run_capture = True

           while run_capture:
               # Get the frame form the webcam
               success, img = capture.read()
               # Hands
               hands, img = detector.findHands(img)
               # Reset data before loop
               data = []

               if hands:
                   first_hand_detected = hands[0]
                   data = convert_to_unity_values(data, first_hand_detected)
                   soc.sendto(str.encode(str(data)), server_address_port)

               img = cv2.resize(img, (0, 0), None, .5, .5)
               run_capture = display_video_capture(window_title, 1, img)

		 *
		 */
	}
}