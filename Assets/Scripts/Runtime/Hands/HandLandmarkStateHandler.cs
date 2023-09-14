// Copyright © Christian Holm Christensen
// 13/09/2023

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace P307.Runtime.Hands
{
	[DisallowMultipleComponent]
	public sealed class HandLandmarkStateHandler : MonoBehaviour
	{
		[SerializeField] List<HandLandmark> landmarkComponents = new();
		 
		[SerializeField] HandLandmark landmarkPrefab;
		
		public static int[] tripleConnectionLandmarks = { 0 };
		public static int[] doubleConnectionLandmarks = { 5, 7, 9, 13 };
		public static int[] endConnectionLandmarks = { 4, 8, 12, 16 };

		const string HAND_LANDMARK_PREFAB_RESOURCE_PATH = "Hands/Hand Point";

		public List<HandLandmark> LandmarkComponents => landmarkComponents;
		
		void Awake()
		{
			landmarkPrefab ??= Resources.Load<HandLandmark>(HAND_LANDMARK_PREFAB_RESOURCE_PATH);
			landmarkComponents ??= GetComponentsInChildren<HandLandmark>().ToList();
		}

		public static void UpdateHand(List<HandLandmark> landmarks)
		{
			for (int point = 0; point < landmarks.Count; point++)
			{
				var landmark = landmarks[point];

				LineRenderer lineRenderer = landmark.LineRenderer;
				
				lineRenderer.positionCount = 2;
				lineRenderer.SetPosition(0, landmark.WorldPosition);

				if (endConnectionLandmarks.Contains(point)) 
					continue;
				
				if (tripleConnectionLandmarks.Contains(point))
				{
					lineRenderer.positionCount = 4;
					lineRenderer.endColor = Color.red;
				}
				if (doubleConnectionLandmarks.Contains(point))
				{
					lineRenderer.positionCount = 3;
					lineRenderer.endColor = Color.blue;
				}
				else
				{
					lineRenderer.positionCount = 2;
					int nextPoint = point + 1;
					if (nextPoint >= landmarks.Count)
						break;
					
					HandLandmark nextLandmark = landmarks[nextPoint];
					lineRenderer.SetPosition(1, nextLandmark.WorldPosition);
					break;
				}

				UpdateFingerBasedOnIndex(point)?.Invoke(landmarks[point], landmarks);
			}
		}

		static Action<HandLandmark, List<HandLandmark>> UpdateFingerBasedOnIndex(int landmarkIndex) => landmarkIndex switch 
		{ 
			0 => UpdateWrist,
			1 => UpdateThumb,
			5 => UpdateIndexFinger,
			9 => UpdateMiddleFinger,
			13 => UpdateRingFinger,
			17 => UpdatePinky,
			_ => null
		};

		static void UpdateWrist(HandLandmark landmark, List<HandLandmark> landmarks)
		{
			LineRenderer line = landmark.LineRenderer;
			HandLandmark thumbCmc = landmarks[1];
			HandLandmark indexMcp = landmarks[5];
			HandLandmark pinkyMcp = landmarks[17];
					
			line.positionCount = 4;
			line.SetPosition(0, landmark.WorldPosition);
			line.SetPosition(1, thumbCmc.WorldPosition);
			line.SetPosition(2, indexMcp.WorldPosition);
			line.SetPosition(3, pinkyMcp.WorldPosition);
		}

		static void UpdateThumb(HandLandmark landmark, List<HandLandmark> landmarks)
		{
			LineRenderer line = landmark.LineRenderer;
			
			if (line.positionCount is not 4)
				line.positionCount = 4;

			HandLandmark cmc = landmarks[1];
			HandLandmark mcp = landmarks[2];
			HandLandmark ip = landmarks[3];
			HandLandmark tip = landmarks[4];

			line.SetPosition(0, landmark.WorldPosition);
			line.SetPosition(1, mcp.WorldPosition);
			
			cmc.LineRenderer.positionCount = 2;
			cmc.LineRenderer.SetPosition(0, cmc.WorldPosition);
			cmc.LineRenderer.SetPosition(1, mcp.WorldPosition);
				
			mcp.LineRenderer.positionCount = 2;
			mcp.LineRenderer.SetPosition(0, mcp.WorldPosition);
			mcp.LineRenderer.SetPosition(1, ip.WorldPosition);

			ip.LineRenderer.positionCount = 2;
			ip.LineRenderer.SetPosition(0, ip.WorldPosition);
			ip.LineRenderer.SetPosition(1, tip.WorldPosition);

			tip.LineRenderer.positionCount = 0;
		}
		
		static void UpdateIndexFinger(HandLandmark landmark, List<HandLandmark> landmarks)
		{
			LineRenderer line = landmark.LineRenderer;
			
			if (line.positionCount is not 2)
				line.positionCount = 2;

			HandLandmark cmc = landmarks[5];
			HandLandmark mcp = landmarks[6];
			HandLandmark ip = landmarks[7];
			HandLandmark tip = landmarks[8];

			line.SetPosition(0, landmark.WorldPosition);
			line.SetPosition(1, mcp.WorldPosition);
			
			cmc.LineRenderer.positionCount = 2;
			cmc.LineRenderer.SetPosition(0, cmc.WorldPosition);
			cmc.LineRenderer.SetPosition(1, mcp.WorldPosition);
				
			mcp.LineRenderer.positionCount = 2;
			mcp.LineRenderer.SetPosition(0, mcp.WorldPosition);
			mcp.LineRenderer.SetPosition(1, ip.WorldPosition);

			ip.LineRenderer.positionCount = 2;
			ip.LineRenderer.SetPosition(0, ip.WorldPosition);
			ip.LineRenderer.SetPosition(1, tip.WorldPosition);

			tip.LineRenderer.positionCount = 0;
		}
		
		static void UpdateMiddleFinger(HandLandmark landmark, List<HandLandmark> landmarks)
		{
			LineRenderer line = landmark.LineRenderer;
			
			if (line.positionCount is not 2)
				line.positionCount = 2;

			HandLandmark cmc = landmarks[9];
			HandLandmark mcp = landmarks[10];
			HandLandmark ip = landmarks[11];
			HandLandmark tip = landmarks[12];

			line.SetPosition(0, landmark.WorldPosition);
			line.SetPosition(1, mcp.WorldPosition);
			
			cmc.LineRenderer.positionCount = 2;
			cmc.LineRenderer.SetPosition(0, cmc.WorldPosition);
			cmc.LineRenderer.SetPosition(1, mcp.WorldPosition);
				
			mcp.LineRenderer.positionCount = 2;
			mcp.LineRenderer.SetPosition(0, mcp.WorldPosition);
			mcp.LineRenderer.SetPosition(1, ip.WorldPosition);

			ip.LineRenderer.positionCount = 2;
			ip.LineRenderer.SetPosition(0, ip.WorldPosition);
			ip.LineRenderer.SetPosition(1, tip.WorldPosition);

			tip.LineRenderer.positionCount = 0;
		}
		
		static void UpdateRingFinger(HandLandmark landmark, List<HandLandmark> landmarks)
		{
			LineRenderer line = landmark.LineRenderer;
			
			if (line.positionCount is not 2)
				line.positionCount = 2;

			HandLandmark cmc = landmarks[13];
			HandLandmark mcp = landmarks[14];
			HandLandmark ip = landmarks[15];
			HandLandmark tip = landmarks[16];

			line.SetPosition(0, landmark.WorldPosition);
			line.SetPosition(1, mcp.WorldPosition);
			
			cmc.LineRenderer.positionCount = 2;
			cmc.LineRenderer.SetPosition(0, cmc.WorldPosition);
			cmc.LineRenderer.SetPosition(1, mcp.WorldPosition);
				
			mcp.LineRenderer.positionCount = 2;
			mcp.LineRenderer.SetPosition(0, mcp.WorldPosition);
			mcp.LineRenderer.SetPosition(1, ip.WorldPosition);

			ip.LineRenderer.positionCount = 2;
			ip.LineRenderer.SetPosition(0, ip.WorldPosition);
			ip.LineRenderer.SetPosition(1, tip.WorldPosition);

			tip.LineRenderer.positionCount = 0;
		}

		static void UpdatePinky(HandLandmark landmark, List<HandLandmark> landmarks)
		{
			LineRenderer line = landmark.LineRenderer;

			if (line.positionCount is not 2)
				line.positionCount = 2;

			HandLandmark cmc = landmarks[17];
			HandLandmark mcp = landmarks[18];
			HandLandmark ip = landmarks[19];
			HandLandmark tip = landmarks[20];

			line.SetPosition(0, landmark.WorldPosition);
			line.SetPosition(1, mcp.WorldPosition);

			cmc.LineRenderer.positionCount = 2;
			cmc.LineRenderer.SetPosition(0, cmc.WorldPosition);
			cmc.LineRenderer.SetPosition(1, mcp.WorldPosition);

			mcp.LineRenderer.positionCount = 2;
			mcp.LineRenderer.SetPosition(0, mcp.WorldPosition);
			mcp.LineRenderer.SetPosition(1, ip.WorldPosition);

			ip.LineRenderer.positionCount = 2;
			ip.LineRenderer.SetPosition(0, ip.WorldPosition);
			ip.LineRenderer.SetPosition(1, tip.WorldPosition);

			tip.LineRenderer.positionCount = 0;
		}
	}
}