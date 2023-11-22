using System;
using System.Collections;
using System.Collections.Generic;
using P307.Shared;
using UnityEngine;
using static P307.Shared.Const307;

namespace P307.Runtime.Hands
{
	[DisallowMultipleComponent][AddComponentMenu("307/Hands/Tracking")]
	public sealed class HandTracking : MonoBehaviour
	{
		public static Action<bool> resetHand = delegate {  };
		
		[SerializeField] Hand hand;
		[SerializeField] UDPReceive udpReceive;
		[SerializeField] List<HandLandmark> landmarks = new();
		
		[Space(5), Header("Movement")]
		[SerializeField] bool lerpMovement;
		[SerializeField, Range(1f, 10f)] float landmarkLerpTime = 8f;
		[SerializeField, Range(1f, 10f)] float resetHandLerpTime = 4f;
		
		[Space(5), Header("Positioning")]
		[SerializeField, Range(-10f, 10f)] float xCorrection = -5f;
		[SerializeField, Range(-10f, 10f)] float yCorrection;
		[SerializeField, Range(-10f, 10f)] float zCorrection;

		bool isResettingHand;
		Camera mainCam;
		WaitForSecondsRealtime waitForSecondsRealtime;
		
		HandLandmark wristLandmark;
		HandLandmark thumbStart;
		HandLandmark indexStart;
		HandLandmark middleStart;
		HandLandmark ringStart;
		HandLandmark pinkyStart;

		readonly Vector3[] startPositions =
		{
			new(1.08f, -0.75f, 0),
			new(.36f, -.73f, -.26f),
			new(-.34f, -.55f, -.48f),
			new(-.92f, -.51f, -.7f),
			new(-1.45f, -.39f, -.92f),
			new(-.235f, .75f, -.29f),
			new(-.74f, 1.34f, -.59f),
			new(-1.1f, 1.6f, -.85f),
			new(-1.44f, 1.8f, -1.04f),
			new(.115f, .9f, -.38f),
			new(-.27f, 1.54f, -.65f),
			new(-.62f, 1.84f, -.87f),
			new(-1f, 2f, -1.04f),
			new(.54f, .84f, -.5f),
			new(.3f, 1.45f, -.8f),
			new(-.02f, 1.6f, -1f),
			new(.42f, 1.7f, -1.15f),
			new(1.07f, .65f, -.65f),
			new(1.08f, 1.06f, -.97f),
			new(.88f, 1.17f, -1.13f),
			new(.58f, 1.2f, -1.24f)
		};

		int screenWidth;
		int screenHeight;
		
		//void Awake()
		//{
		//	waitForSecondsRealtime = new WaitForSecondsRealtime(waitStartTime);
		//	mainCam = Camera.main;
		//	hand = FindFirstObjectByType<Hand>(FindObjectsInactive.Include);
		//	udpReceive = FindFirstObjectByType<UDPReceive>(FindObjectsInactive.Include);
		//	resetHand = OnResetHand;
		//}

		void OnDisable()
		{
			resetHand -= OnResetHand;
		}

		void OnResetHand(bool doReset)
		{
			isResettingHand = doReset;
		}

		//void Start()
		//{
		//	if (hand == null)
		//		throw new NullReferenceException("Hand was null!");
		//	
		//	landmarks = hand.GetComponentInChildren<HandLandmarkPositionController>().LandmarkComponents;
		//	wristLandmark = landmarks[0];
		//	thumbStart = landmarks[1];
		//	indexStart = landmarks[5];
		//	middleStart = landmarks[9];
		//	ringStart = landmarks[13];
		//	pinkyStart = landmarks[17];
		//}
		

		Vector3 TranslateDataToCoordinates(int i, IReadOnlyList<string> coordsData)
		{
			const float one_percent = .01f;

			int xCoordIndex = i * THREE;
			int yCoordIndex = i * THREE + ONE;
			int zCoordIndex = i * THREE + TWO;
			int count = coordsData.Count;
			bool allIndicesAreValid = xCoordIndex < count
			                          && yCoordIndex < count
			                          && zCoordIndex < count;

			if (allIndicesAreValid is false)
				return landmarks[i].transform.localPosition;
			
			float x = float.Parse(coordsData[xCoordIndex]) * one_percent + xCorrection;
			float y = float.Parse(coordsData[yCoordIndex]) * one_percent + yCorrection;
			float z = float.Parse(coordsData[zCoordIndex]) * one_percent + zCorrection;
			return new Vector3(x, y, z);
		}

		/*
		void Update()
		{
			if (Application.isPlaying is false)
				return;
			
			string[] landmarkCoords = read_coordinates_from_data_stream(udpReceive.DataStream);
			
			if (isResettingHand)
			{
				ReturnToStartPosition();
				return;
			}
			
			for (int i = 0; i < HAND_POINT_COUNT; i++)
			{
				Vector3 newPos = TranslateDataToCoordinates(i, landmarkCoords);
				UpdateHandPosition(i, newPos, landmarkLerpTime);
			}
			return;

			string[] read_coordinates_from_data_stream(string dataIn)
			{
				if (string.IsNullOrEmpty(dataIn))
					return new string[] { };

				const char separator = ',';
				
				remove_square_brackets(ref dataIn);
				string[] coordinates = dataIn.Split(separator);
				return coordinates;
			}

			void remove_square_brackets(ref string dataIn)
			{
				const int first_symbol_index = 0;
				const int number_of_symbols_to_remove = 1;
				int last_symbol = dataIn.Length - 1;

				dataIn = dataIn.Remove(last_symbol, number_of_symbols_to_remove);
				dataIn = dataIn.Remove(first_symbol_index, number_of_symbols_to_remove);
			}
		}
		*/

		Vector3 previousPosition;
		
		void UpdateHandPosition(int i, Vector3 newPos, float lerpTime)
		{
			bool allLandmarksUpdated = i == landmarks.Indices();
			Vector3 currentPos = landmarks[i].transform.localPosition;
			if (currentPos != newPos)
			{
				landmarks[i].transform.localPosition = lerpMovement && lerpTime is not ZERO
					? Vector3.Lerp(currentPos, newPos, Time.deltaTime * Mathf.Abs(lerpTime))
					: newPos;
			}
			if (allLandmarksUpdated is false)
				return;
			
			//HandLandmarkPositionController.UpdateHandLines(landmarks);
		}
		
		void ReturnToStartPosition()
		{
			for (int i = 0; i < startPositions.Length; i++)
				UpdateHandPosition(i, startPositions[i], resetHandLerpTime);
		}

		IEnumerator WaitForPythonLoading()
		{
			string data;
			const string true_string = "True";
			
			do
			{
				data = udpReceive.DataStream;
				yield return null;
			} while (data.Equals(true_string) is false);

			yield return waitForSecondsRealtime;
			
		}
		
	} 
}