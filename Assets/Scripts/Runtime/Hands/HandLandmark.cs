// Copyright © Christian Holm Christensen
// 10/09/2023

using P307.Runtime.Hands.ScriptableObjects;
using UnityEngine;

namespace P307.Runtime.Hands
{
	[DisallowMultipleComponent]
	public sealed class HandLandmark : MonoBehaviour
	{
		[SerializeField] int index;
		[SerializeField] string landmarkTag;
		[SerializeField] MeshFilter meshFilter;
		[SerializeField] LineRenderer lineRenderer;
		[SerializeField] MeshRenderer meshRenderer;
		[SerializeField] HandLandmarkSO landmarkValues;
		//[SerializeField] HandLandmarkSO[] connections = { };
		[SerializeField, Range(0f, 1f)] AnimationCurve lineCurve = new();
		[SerializeField] PrimitiveType primitiveType = PrimitiveType.Sphere;
		
		public int Index => index;
		public Vector3 WorldPosition => transform.position;
		public LineRenderer LineRenderer => lineRenderer ??= gameObject.AddComponent<LineRenderer>();
		public MeshRenderer MeshRenderer => meshRenderer ??= GetComponentInChildren<MeshRenderer>();
		public MeshFilter MeshFilter => meshFilter ??= GetComponentInChildren<MeshFilter>();
		public HandLandmarkSO Values { get => landmarkValues; set => landmarkValues = value; }
		public PrimitiveType PrimitiveType { get => primitiveType; set => primitiveType = value; }
		
		public void Init(int i, string tagText, Material material)
		{
			index = i;
			landmarkTag = tagText;
			lineRenderer ??= gameObject.AddComponent<LineRenderer>();
			lineRenderer.materials = new[] { material };
			lineRenderer.widthCurve = lineCurve;
			meshRenderer = GetComponentInChildren<MeshRenderer>();
			meshFilter = GetComponentInChildren<MeshFilter>();
			
			landmarkValues = ScriptableObject.CreateInstance<HandLandmarkSO>();
		}

		public void UpdatePosition(Vector3 newPos)
		{
			transform.position = newPos;
			lineRenderer.SetPosition(0, newPos);
		}
		
		public void UpdateMesh(PrimitiveType primitive)
		{
			Mesh mesh = HandUtils.GetLandmarkMesh(primitive);
			meshFilter.mesh = mesh;
		}
	}
}