// Copyright © Christian Holm Christensen
// 10/09/2023

using System;
using System.Collections.Generic;
using UnityEngine;
using static P307.Runtime.Hands.HandUtils;

namespace P307.Runtime.Hands
{
	[DisallowMultipleComponent]
	public sealed class HandTracking : MonoBehaviour
	{
		public static Action returnToStart = delegate {  }; 
		
		[SerializeField] Hand hand;
		[SerializeField] UDPReceive udpReceive;
		[SerializeField] List<HandLandmark> landmarks = new();
		[SerializeField, Range(1f, 10f)] float landmarkLerpTime = 2f;
		
		HandLandmark wristLandmark;
		HandLandmark thumbStart;
		HandLandmark indexStart;
		HandLandmark middleStart;
		HandLandmark ringStart;
		HandLandmark pinkyStart;

		Camera mainCam;
		
		void Awake()
		{
			mainCam = Camera.main;
			hand = FindFirstObjectByType<Hand>(FindObjectsInactive.Include);
			udpReceive = FindFirstObjectByType<UDPReceive>(FindObjectsInactive.Include);
		}

		void Start()
		{
			if (hand != null)
			{
				landmarks = hand.GetComponentInChildren<HandLandmarkStateHandler>().LandmarkComponents;
				wristLandmark = landmarks[0];
				thumbStart = landmarks[1];
				indexStart = landmarks[5];
				middleStart = landmarks[9];
				ringStart = landmarks[13];
				pinkyStart = landmarks[17];
			}
		}
		
		Vector3 TranslateDataToCoordinates(int index, IReadOnlyList<string> coordsData)
		{
			float x = float.Parse(coordsData[index * 3]) / 100;
			float y = float.Parse(coordsData[index * 3 + 1]) / 100;
			float z = float.Parse(coordsData[index * 3 + 2]) / 100;
			return new Vector3(x, y, z);
		}

		void Update()
		{
			if (Application.isPlaying is false)
				return;
			
			string[] landmarkCoords = get_coordinates_from(udpReceive.DataStream);
			if (landmarkCoords.Length is 0)
			{
				returnToStart?.Invoke();
				return;
			}
			for (int i = 0; i < HAND_POINT_COUNT; i++)
			{
				Vector3 localPos = landmarks[i].transform.localPosition;
				Vector3 newPos = TranslateDataToCoordinates(i, landmarkCoords);
				landmarks[i].transform.localPosition = Vector3.Lerp(localPos, newPos, Time.deltaTime * landmarkLerpTime);
				HandLandmarkStateHandler.UpdateHand(landmarks);
			}
			
			return;

			string[] get_coordinates_from(string dataIn)
			{
				if (string.IsNullOrEmpty(dataIn))
					return new string[] { };
				
				remove_square_brackets(ref dataIn);
				const char separator = ',';
				string[] coordinates = dataIn.Split(separator);
				return coordinates;
			}

			void remove_square_brackets(ref string dataIn)
			{
				const int first_symbol = 0;
				const int single_symbol = 1;
				int last_symbol = dataIn.Length - 1;

				dataIn = dataIn.Remove(last_symbol, single_symbol);
				dataIn = dataIn.Remove(first_symbol, single_symbol);
			}
		}
	} 
}