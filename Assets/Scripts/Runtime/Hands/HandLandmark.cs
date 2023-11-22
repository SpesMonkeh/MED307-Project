using P307.Runtime.Hands.ScriptableObjects;
using UnityEngine;
using static P307.Runtime.Hands.HandUtils;
using static P307.Shared.Const307;

namespace P307.Runtime.Hands
{
	[DisallowMultipleComponent] [RequireComponent(typeof(LineRenderer))] [AddComponentMenu("307/Hands/Landmark", order: ZERO)]
	public sealed class HandLandmark : MonoBehaviour
	{
		[SerializeField] int index;
		[SerializeField] string landmarkTag;
		[SerializeField] MeshFilter meshFilter;
		[SerializeField] MeshRenderer meshRenderer;
		[SerializeField] LineRenderer lineRenderer;
		[SerializeField] HandLandmarkSO data;
		[SerializeField, Range(ZERO, ONE)] AnimationCurve lineCurve = new();

		[Space(14), Header("Experimental shite")]
		[SerializeField] bool useCoordinateColor = true;

		public int Index => index;
		public LineRenderer LineRenderer => lineRenderer = GetComponent<LineRenderer>();
		public MeshFilter MeshFilter => meshFilter ??= GetComponent<MeshFilter>();
		public MeshRenderer MeshRenderer => meshRenderer ??= GetComponent<MeshRenderer>();
		public HandLandmarkSO Data { get => data; set => data = value; }
		public PrimitiveType PrimitiveType => HandLandmarkSettingsSO.getMeshPrimitiveSetting?.Invoke() ?? PrimitiveType.Sphere;

		static Material MeshMaterial => HandLandmarkSettingsSO.getMeshMaterial?.Invoke();
		static Material LineMaterial => HandLandmarkSettingsSO.getLineMaterial?.Invoke();
		
		public void Init(int i)
		{
			index = i;
			landmarkTag = LandmarkTagsOfIndex[i];

			name = $"{i}. {landmarkTag}";

			SetupMesh(MeshMaterial);

			lineRenderer = GetComponent<LineRenderer>();
			lineRenderer.positionCount = LineConnectionIndicesOfIndex[i].Length;
			lineRenderer.SetPosition(ZERO, transform.position);
			lineRenderer.materials = new[] { LineMaterial };
			lineRenderer.widthCurve = lineCurve;
		}

		public void UpdateColorBasedOnCoordinates()
		{
			if (useCoordinateColor is false)
				return;
			Vector3 position = transform.position;
			meshRenderer.material.color = new Color(
				r: Mathf.InverseLerp(-ONE_HUNDRED, TEN * FIVE, position.x),
				g: Mathf.InverseLerp(-ONE_HUNDRED, TEN * FIVE, position.y),
				b: Mathf.InverseLerp(-ONE_HUNDRED, TEN * FIVE, position.z));
		}

		void FixedUpdate()
		{
			UpdateColorBasedOnCoordinates();
		}

		void SetupMesh(Material material)
		{
			transform.localScale = Vector3.one + (HandLandmarkSettingsSO.getMeshScaleSetting?.Invoke() ?? Vector3.one);

			meshFilter = GetComponent<MeshFilter>();
			meshFilter.mesh = GetLandmarkMesh(PrimitiveType);

			meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.materials = new[] { material };
		}
	}
}