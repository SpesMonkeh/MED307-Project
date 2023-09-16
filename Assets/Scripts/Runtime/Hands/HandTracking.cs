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
		[SerializeField] Hand hand;
		[SerializeField] UDPReceive udpReceive;
		[SerializeField] List<HandLandmark> landmarks = new();
		
		[Space(5), Header("Movement")]
		[SerializeField] bool lerpMovement;
		[SerializeField, Range(1f, 10f)] float landmarkLerpTime = 8f;

		[Space(5), Header("Positioning")]
		[SerializeField, Range(-10f, 10f)] float xCorrection = -5f;
		[SerializeField, Range(-10f, 10f)] float yCorrection;
		[SerializeField, Range(-10f, 10f)] float zCorrection;
		
		Camera mainCam;
		
		HandLandmark wristLandmark;
		HandLandmark thumbStart;
		HandLandmark indexStart;
		HandLandmark middleStart;
		HandLandmark ringStart;
		HandLandmark pinkyStart;

		readonly Vector3[] startPositions =
		{
			new(7.52f, 5.1f, 0),
			new(6.76f, 5.22f, -.25f),
			new(6.12f, 5.47f, -.55f),
			new(5.51f, 5.52f, -.85f),
			new(4.94f, 5.52f, -1.18f),
			new(6.4f, 6.85f, -.47f),
			new(5.96f, 7.55f, -.86f),
			new(5.6f, 7.92f, -1.17f),
			new(5.22f, 8.18f, -1.39f),
			new(6.76f, 6.92f, -.63f),
			new(6.44f, 7.79f, -1f),
			new(6.07f, 8.24f, -1.27f),
			new(5.69f, 8.57f, -1.47f),
			new(7.15f, 6.77f, -.83f),
			new(6.96f, 7.65f, -1.19f),
			new(6.66f, 8.1f, -1.43f),
			new(6.31f, 8.49f, -1.6f),
			new(7.54f, 6.43f, -1.05f),
			new(7.66f, 7f, -1.39f),
			new(7.61f, 7.42f, -1.57f),
			new(7.49f, 7.83f, -1.69f)
		};

		int screenWidth;
		int screenHeight;
		
		void Awake()
		{
			mainCam = Camera.main;
			hand = FindFirstObjectByType<Hand>(FindObjectsInactive.Include);
			udpReceive = FindFirstObjectByType<UDPReceive>(FindObjectsInactive.Include);
		}

		void Start()
		{
			if (hand == null)
				throw new NullReferenceException("Hand was null!");
			
			landmarks = hand.GetComponentInChildren<HandLandmarkStateHandler>().LandmarkComponents;
			wristLandmark = landmarks[0];
			thumbStart = landmarks[1];
			indexStart = landmarks[5];
			middleStart = landmarks[9];
			ringStart = landmarks[13];
			pinkyStart = landmarks[17];
		}
		

		Vector3 TranslateDataToCoordinates(int index, IReadOnlyList<string> coordsData)
		{
			const int one = 1;
			const int two = 2;
			const int three = 3;
			const float one_percent = .01f;
			
			float x = float.Parse(coordsData[index * three]) * one_percent + xCorrection;
			float y = float.Parse(coordsData[index * three + one]) * one_percent + yCorrection;
			float z = float.Parse(coordsData[index * three + two]) * one_percent + zCorrection;
			return new Vector3(x, y, z);
		}

		void Update()
		{
			if (Application.isPlaying is false)
				return;
			
			string[] landmarkCoords = read_coordinates_from_data_stream(udpReceive.DataStream);
			if (landmarkCoords.Length is 0)
			{
				ReturnToStartPosition();
				return;
			}
			
			for (int i = 0; i < HAND_POINT_COUNT; i++)
			{
				Vector3 newPos = TranslateDataToCoordinates(i, landmarkCoords);
				UpdateHandPosition(i, newPos);
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

		void UpdateHandPosition(int i, Vector3 newPos)
		{
			Vector3 currentPos = landmarks[i].transform.localPosition;
			landmarks[i].transform.localPosition = lerpMovement
				? Vector3.Lerp(currentPos, newPos, Time.deltaTime * landmarkLerpTime)
				: newPos;
			
			HandLandmarkStateHandler.UpdateHand(landmarks);
		}
		
		void ReturnToStartPosition()
		{
			for (int i = 0; i < startPositions.Length; i++)
				UpdateHandPosition(i, startPositions[i]);
		}
	} 
}