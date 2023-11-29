// Copyright © Christian Holm Christensen
// 29/11/2023

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mediapipe.Unity;
using P307.Runtime.Hands;
using P307.Runtime.Hands.ScriptableObjects;
using UnityEngine;
using static P307.Shared.Const307;

namespace P307.Runtime.ComputerVision.MediaPipe
{
	[DefaultExecutionOrder(50)] // Skal helst køre efter Mediapipe.Unity.MultiHandListAnnotation (0)
	public sealed class PointListAnnotationVoyeur : MonoBehaviour
	{
		[Header("Indstillinger")]
		[SerializeField] PointListAnnotation pointListAnnotation;
		[SerializeField] List<HandLandmarkSO> landmarkData = new();
		[SerializeField, Range(ZERO, HAND_LANDMARK_COUNT)] int pointCount = ZERO;
		[SerializeField, Range(ZERO, HAND_LANDMARK_COUNT)] int landmarkCount = ZERO;

		[Space(5), Header("MediaPipe Kommunikation")]
		[SerializeField, Range(ZERO, 500)] int retrievalAttempts = 50;
		[SerializeField, Range(1, 10)] int retrievalDelaySeconds = 2;

		int delayInMilliseconds;
		Task retrievalDelay;
		PointAnnotationSpace pointAnnotationSpace;

		Action runLandmarkUpdateLoop;

		readonly Dictionary<PointAnnotation, HandLandmarkSO> points2Landmarks = new();

		public static Action<List<PointAnnotation>> pointAnnotationsSet;

		Action RunLandmarkUpdateLoop => runLandmarkUpdateLoop ??= points2Landmarks.Count is HAND_LANDMARK_COUNT 
			? UpdateLandmarkData 
			: null;


		void OnEnable()
		{
			Hand.landmarkDataCreated += OnHandLandmarkDataCreated;
		}

		void OnDisable()
		{
			Hand.landmarkDataCreated -= OnHandLandmarkDataCreated;
			runLandmarkUpdateLoop -= UpdateLandmarkData;
		}

		void Start()
		{
			delayInMilliseconds = retrievalDelaySeconds * ONE_THOUSAND;
			pointAnnotationSpace = HandLandmarkSettingsSO.PointAnnotationSpace;
			BeginPointListAnnotationRetrieval();
		}

		void Update()
		{
			RunLandmarkUpdateLoop?.Invoke();
		}

		void UpdateLandmarkData()
		{
			if (points2Landmarks.Count is not HAND_LANDMARK_COUNT)
				return;

			foreach ((PointAnnotation point, HandLandmarkSO landmarkSO) in points2Landmarks)
			{
				Transform pointTf = point.transform;
				Quaternion pointRot = pointTf.rotation;
				Matrix4x4 localToWorldMatrix = pointTf.localToWorldMatrix;
				Matrix4x4 worldToLocalMatrix = pointTf.worldToLocalMatrix;
				Vector3 pointPosL2W = localToWorldMatrix.GetPosition();
				Vector3 pointPosW2L = worldToLocalMatrix.GetPosition();

				Vector3 newPos = pointAnnotationSpace switch
				{
					PointAnnotationSpace.LocalPositionVector => pointTf.localPosition,
					PointAnnotationSpace.WorldPositionVector => pointTf.position,
					PointAnnotationSpace.Local2WorldPositionMatrix => pointPosL2W,
					PointAnnotationSpace.World2LocalPositionMatrix => pointPosW2L,
					_ => throw new ArgumentOutOfRangeException()
				};
				//landmarkSO.SetPositionRotation(newPos, pointRot);
			}
		}

		void OnHandLandmarkDataCreated(List<HandLandmarkSO> data)
		{
			if (data == null || data.Count is ZERO || landmarkData == data)
				return;
			landmarkData = data;
			landmarkCount = landmarkData.Count;

			if (Mathf.Abs(pointCount - landmarkCount) is ZERO)
			{
				List<PointAnnotation> points = pointListAnnotation.children;

				for (int i = ZERO; i < landmarkCount; i++)
				{
					points2Landmarks.Add(points[i], landmarkData[i]);
				}
			}

			Hand.landmarkDataCreated -= OnHandLandmarkDataCreated;
		}

		async void BeginPointListAnnotationRetrieval()
		{
			(bool isTrue, PointListAnnotation value) foundAnnotation = await LookForPointListAnnotationActivation();
			pointListAnnotation = foundAnnotation.isTrue ? foundAnnotation.value : null;
			if (foundAnnotation.isTrue is false)
				return;

			pointCount = pointListAnnotation.children.Count;
			pointAnnotationsSet?.Invoke(pointListAnnotation.children);
		}

		async Task<(bool isTrue, PointListAnnotation value)> LookForPointListAnnotationActivation()
		{
			int attempts = ONE;
			bool wasFound;
			PointListAnnotation annotation;

			do
			{
				Debug.Log($"AwaitPointListAnnotationActivation() :: Attempt: {attempts}/{retrievalAttempts}");
				annotation = GetComponentInChildren<PointListAnnotation>();
				wasFound = annotation != null;

				if (wasFound)
				{
					Debug.Log("AwaitPointListAnnotationActivation() :: PointListAnnotation-komponent blev fundet!");
					break;
				}

				await Task.Delay(delayInMilliseconds);
				attempts++;

			} while (attempts <= retrievalAttempts);

			return (wasFound, annotation);
		}
	}
}