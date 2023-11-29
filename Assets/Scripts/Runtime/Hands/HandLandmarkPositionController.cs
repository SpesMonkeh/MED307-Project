using System;
using System.Collections.Generic;
using System.Linq;
using Mediapipe.Unity;
using P307.Runtime.ComputerVision.MediaPipe;
using P307.Runtime.Hands.ScriptableObjects;
using P307.Shared;
using UnityEngine;
using static P307.Shared.Const307;

namespace P307.Runtime.Hands
{
	[DisallowMultipleComponent][AddComponentMenu("307/Hands/Landmarks/Position Controller")]
	public sealed class HandLandmarkPositionController : MonoBehaviour
	{
		[Space(FIVE), Header("Points and Landmarks")]
		[SerializeField] HandLandmark[] handLandmarks = { };
		[SerializeField] PointAnnotation[] pointAnnotations = { };
		
		[SerializeField] List<HandLandmark> landmarkComponents = new();
		[SerializeField] HandLandmark landmarkPrefab;
		[SerializeField] PrimitiveType meshType = PrimitiveType.Sphere;
		
		Dictionary<PointAnnotation, HandLandmark> pointLandmarkDict = new();

		const string HAND_LANDMARK_PREFAB_RESOURCE_PATH = "Hands/Hand Point";

		public List<HandLandmark> LandmarkComponents => landmarkComponents;
		public PrimitiveType MeshType => meshType;
		
		
		public HandLandmark GetLandmark(int index)
		{
			if (landmarkComponents.Count is not HAND_LANDMARK_COUNT)
				RefreshLandmarkComponents();
			index = Mathf.Abs(Mathf.Clamp(index, ZERO, landmarkComponents.Indices()));
			return landmarkComponents[index];
		}
		
		public List<HandLandmark> RefreshLandmarkComponents()
		{
			List<HandLandmark> foundComponents = GetComponentsInChildren<HandLandmark>().ToList();
			bool componentsChanged = landmarkComponents != foundComponents;

			Debug.Log(componentsChanged, "List<HandLandmark> landmarkComponents blev opdateret.", this);
			
			return landmarkComponents = componentsChanged
				? foundComponents
				: landmarkComponents;
		}
        
		void OnEnable()
		{
			Hand.landmarkDataCreated += OnHandLandmarkDataCreated;
			PointListAnnotationVoyeur.pointAnnotationsSet += OnPointAnnotationsSet;
		}

		void OnDisable()
		{
			Hand.landmarkDataCreated -= OnHandLandmarkDataCreated;
			PointListAnnotationVoyeur.pointAnnotationsSet -= OnPointAnnotationsSet;
		}

		void Awake()
		{
			landmarkPrefab ??= Resources.Load<HandLandmark>(HAND_LANDMARK_PREFAB_RESOURCE_PATH);
		}

		void Start()
		{
			RefreshLandmarkComponents();
		}

		void Update()
		{
			UpdateHandLines();
		}
		
		void OnHandLandmarkDataCreated(List<HandLandmarkSO> dataList)
		{
			if (Mathf.Abs(dataList.Count - landmarkComponents.Count) is not ZERO)
			{
				Debug.LogError(" OnHandLandmarkDataCreated(List<HandLandmarkSO>) :: dataList.Count og landmarkComponents.Count var ikke lig 0.", this);
				return;
			}
			for (int i = ZERO; i < dataList.Count; i++)
			{
				landmarkComponents[i].Data = dataList[i];
			}
		}
		
		void OnPointAnnotationsSet(List<PointAnnotation> points)
		{
			if (PopulatePointLandmarkDictionary(points) is false)
				return;
			pointAnnotations = pointLandmarkDict.Keys.ToArray();
			handLandmarks = pointLandmarkDict.Values.ToArray();
			ToggleLandmarkMeshRendering(true);
		}

		void ToggleLandmarkMeshRendering(bool enable)
		{
			if (handLandmarks == null || handLandmarks.Length is ZERO)
				return;
			for (int j = ZERO; j < handLandmarks.Length; j++)
			{
				handLandmarks[j].MeshRenderer.enabled = enable;
			}
		}

		void UpdateHandLines()
		{
			//for (int i = ZERO; i < pointAnnotations.Length; i++)
			//{
			//	UpdatePositionOfHandLandmark(handLandmarks[i].transform, pointAnnotations[i].transform);
			//}
			
			if (pointLandmarkDict.Count is ZERO)
				return;
			int count = pointLandmarkDict.Count;
			int j = ZERO;
			foreach (var (point, landmark) in pointLandmarkDict)
			{
				Transform lmTf = landmark.transform;
				LineRenderer line = landmark.LineRenderer;
				line.SetPosition(ZERO, lmTf.position);
				
				UpdatePositionOfHandLandmark(lmTf, point.transform);
				
				if (j + ONE >= count)
					break;
				
				if (HandUtils.IndexDict.TryGetValue(j, out int[] indices) is false)
				{
					j++;
					continue;
				}
				UpdateBaseLandmarkLines(indices, pointLandmarkDict);
				j++;
			}
		}
		
		bool PopulatePointLandmarkDictionary(List<PointAnnotation> points)
		{
			if (points is null || points.Count is ZERO)
				return false;
			for (int i = ZERO; i < points.Count; i++)
			{
				HandLandmark value = i <= landmarkComponents.Indices() ? landmarkComponents[i] : null;
				pointLandmarkDict.TryAdd(points[i], value);
			}
			return pointLandmarkDict.Count is not ZERO;
		}

		static void UpdateBaseLandmarkLines(IReadOnlyCollection<int> indices, Dictionary<PointAnnotation, HandLandmark> dictionary)
		{
			PointAnnotation[] points = dictionary.Keys.ToArray();
			var positions = new Vector3[indices.Count];
			
			for (int j = ZERO; j < HandUtils.PalmIndices.Length; j++)
			{
				int index = HandUtils.PalmIndices[j];
				var basePoint = points[index];
				var baseLandmark = dictionary[basePoint];
				UpdateFingerLineRenderer(baseLandmark, positions);
			}
		}

		static void UpdatePositionOfHandLandmark(Transform lmTransform, Transform pointTransform)
		{
			switch (HandLandmarkSettingsSO.LandmarkPositionSpace)
			{
				case LandmarkPositionSpace.Local:
					lmTransform.localPosition = pointTransform.localPosition;
					break;
				case LandmarkPositionSpace.World:
					lmTransform.position = pointTransform.position;
					break;
				
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		static void UpdateFingerLineRenderer(HandLandmark baseLm, IReadOnlyList<Vector3> positions)
		{
			if (positions.Count is ZERO)
				return;
			
			LineRenderer line = baseLm.LineRenderer;
			line.positionCount = positions.Count;
			for (int i = ZERO; i < positions.Count; i++)
			{
				line.SetPosition(i, positions[i]);
			}
		}
	}
}