using System;
using System.Collections.Generic;
using Mediapipe.Unity;
using P307.Runtime.ComputerVision.MediaPipe;
using P307.Runtime.Hands.ScriptableObjects;
using UnityEngine;
using static P307.Shared.Const307;

namespace P307.Runtime.Hands
{
	[DisallowMultipleComponent][AddComponentMenu("307/Hands/Hand")]
	public sealed class Hand : MonoBehaviour
	{
		public static Action<List<HandLandmarkSO>> landmarkDataCreated;
		
		[SerializeField] HandLandmark handLandmarkPrefab;
		[SerializeField] GameObject landmarkContainer;
		[SerializeField] PrimitiveType landmarkPrimitive;
		[SerializeField] List<HandLandmarkSO> landmarkData = new();
		
		
		void OnEnable()
		{
			
			PointListAnnotationVoyeur.pointAnnotationsSet += OnPointAnnotationsSet;
		}

		void OnDisable()
		{
			PointListAnnotationVoyeur.pointAnnotationsSet -= OnPointAnnotationsSet;
		}
		
		void OnPointAnnotationsSet(List<PointAnnotation> obj)
		{
			if (obj.Count is not HAND_LANDMARK_COUNT || landmarkData.Count is HAND_LANDMARK_COUNT)
				return;
			landmarkData = CreateHandLandmarkData(obj.Count);
		}

		static List<HandLandmarkSO> CreateHandLandmarkData(int count)
		{
			List<HandLandmarkSO> data = new();
			for (int i = ZERO; i < count; i++)
			{
				var landmarkSO = HandLandmarkSO.Create(i);
				data.Add(landmarkSO);
			}
			Debug.Log($"Instantiated {count} HandLandmarkSO(s)");
			landmarkDataCreated?.Invoke(data);
			return data;
		}
	}
}