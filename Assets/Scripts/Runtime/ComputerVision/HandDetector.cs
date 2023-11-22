// Original Python Hand Tracking Module script by Computer Vision Zone
// Website: https://www.computervision.zone

using System;
using System.Collections;
using System.Threading.Tasks;
using Mediapipe.Unity;
using Mediapipe.Unity.HandTracking;
using UnityEngine;
using static P307.Shared.Const307;

namespace P307.Runtime.ComputerVision
{
	/// <summary>
	/// Finds Hands using the MediaPipe library. Exports the landmarks
	/// in pixel format. Adds extra functionalities like finding how
	/// many fingers are up or the distance between two fingers. Also
	/// provides bounding box info of the hand found. (Computer Vision Zone)
	/// </summary>
	[RequireComponent(typeof(HandTrackingSolution))]
	public sealed class HandDetector : MonoBehaviour
	{
		[SerializeField] HandTrackingSolution hands;
		
		[Tooltip("Maximum number of hands to detect")]
		[SerializeField, Range(ONE, TWO)] int maxHands = ONE;
		
		[Tooltip("Complexity of the hand landmark model: 0 or 1")]
		[SerializeField] HandTrackingGraph.ModelComplexity modelComplexity = HandTrackingGraph.ModelComplexity.Full;
		
		[Tooltip("Minimum Detection Confidence Threshold")]
		[SerializeField, Range(ZERO, ONE)] float detectionConfidence = HALF;
		
		[Tooltip("Minimum Tracking Confidence Threshold")]
		[SerializeField, Range(ZERO, ONE)] float minTrackingConfidence = HALF;

		int taskDelayTime = 2000;
		
		public int MaxHands
		{
			get => maxHands;
			set
			{
				maxHands = value;
				if (hands == null)
					return;
				hands.maxNumHands = value;
			}
		}
		
		public HandTrackingGraph.ModelComplexity ModelComplexity
		{
			get => modelComplexity;
			set
			{
				modelComplexity = value;
				if (hands == null)
					return;
				hands.modelComplexity = value;
			}
		}
		
		public float MinDetectionConfidence
		{
			get => detectionConfidence;
			set
			{
				detectionConfidence = value;
				if (hands == null)
					return;
				hands.minDetectionConfidence = value;
			}
		}
		
		
		public float MinTrackingConfidence
		{
			get => minTrackingConfidence;
			set
			{
				minTrackingConfidence = value;
				if (hands == null)
					return;
				hands.minTrackingConfidence = value;
			}
		}
		

		void Awake()
		{
			hands = GetComponent<HandTrackingSolution>();
		}

		void Start()
		{
			SetupHands();
		}

		async void SetupHands()
		{
			if (hands == null && TryGetComponent(out hands) is false)
				return;

			bool foundGraphRunner = await AwaitGraphRunnerActivation();
			if (foundGraphRunner is false)
				return;
			hands.maxNumHands = maxHands;
			hands.modelComplexity = modelComplexity;
			hands.minDetectionConfidence = detectionConfidence;
			hands.minTrackingConfidence = minTrackingConfidence;
		}

		async Task<bool> AwaitGraphRunnerActivation()
		{
			bool runLoop = true;
			int i = ONE;
			while (runLoop)
			{
				Debug.Log($"Leder efter GraphRunner. Forsøg nr. {i}");
				runLoop = hands != null && hands.GraphRunner != null;
				if (runLoop)
					return true;
				await Task.Delay(taskDelayTime);
				i++;
			}
			return false;
		}


		/*
        self.mpDraw = mp.solutions.drawing_utils
        self.tipIds = [4, 8, 12, 16, 20]
        self.fingers = []
        self.lmList = []
		 */

		void FindHands(ImageSource img, bool draw = true, bool flipType = true)
		{
		}
		
		void FingersUp()
		{
			(int thumb, int index, int middle, int ring, int pinky) fingers = (ZERO, ZERO, ZERO, ZERO, ZERO);
			 	
		}
		/*
		 *     def fingersUp(self, myHand):
        """
        Finds how many fingers are open and returns in a list.
        Considers left and right hands separately
        :return: List of which fingers are up
        """
        fingers = []
        myHandType = myHand["type"]
        myLmList = myHand["lmList"]
        if self.results.multi_hand_landmarks:

            # Thumb
            if myHandType == "Right":
                if myLmList[self.tipIds[0]][0] > myLmList[self.tipIds[0] - 1][0]:
                    fingers.append(1)
                else:
                    fingers.append(0)
            else:
                if myLmList[self.tipIds[0]][0] < myLmList[self.tipIds[0] - 1][0]:
                    fingers.append(1)
                else:
                    fingers.append(0)

            # 4 Fingers
            for id in range(1, 5):
                if myLmList[self.tipIds[id]][1] < myLmList[self.tipIds[id] - 2][1]:
                    fingers.append(1)
                else:
                    fingers.append(0)
        return fingers
		 */
	}
}