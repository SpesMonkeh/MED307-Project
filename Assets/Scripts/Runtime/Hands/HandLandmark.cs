// Copyright © Christian Holm Christensen
// 10/09/2023

using P307.Runtime.Hands.ScriptableObjects;
using UnityEngine;

namespace P307.Runtime.Hands
{
	[DisallowMultipleComponent][RequireComponent(typeof(LineRenderer))]
	public sealed class HandLandmark : MonoBehaviour
	{
		[SerializeField] int index;
		[SerializeField] string landmarkTag;
		[SerializeField] MeshFilter meshFilter;
		[SerializeField] MeshRenderer meshRenderer;
		[SerializeField] LineRenderer lineRenderer;
		[SerializeField] HandLandmarkSO data;
		//[SerializeField] HandLandmarkSO[] connections = { };
		[SerializeField, Range(0f, 1f)] AnimationCurve lineCurve = new();
		[SerializeField] PrimitiveType primitiveType = PrimitiveType.Sphere;
		
		public Vector3 WorldPosition => transform.position;
		public LineRenderer LineRenderer => lineRenderer = GetComponent<LineRenderer>();
		public MeshRenderer MeshRenderer => meshRenderer ??= GetComponentInChildren<MeshRenderer>();
		public MeshFilter MeshFilter => meshFilter ??= GetComponentInChildren<MeshFilter>();
		public HandLandmarkSO Values { get => data; set => data = value; }
		public PrimitiveType PrimitiveType { get => primitiveType; set => primitiveType = value; }
		
		public void Init(int i, PrimitiveType meshType, Material lmMaterial, Material lineMaterial)
		{
			index = i;
			landmarkTag = HandUtils.LandmarkTags[i];
			
			name = $"{i}. {landmarkTag}";
			data = HandLandmarkSO.Create(i, landmarkTag, this);

			SetupMesh(primitiveType, lmMaterial);

			lineRenderer = GetComponent<LineRenderer>();
			lineRenderer.positionCount = HandUtils.ConnectionsToIndex[i].Length;
			lineRenderer.SetPosition(0, transform.position);
			
			lineRenderer.materials = new []{ lineMaterial };
			
			lineRenderer.widthCurve = lineCurve;
			
		}

		void SetupMesh(PrimitiveType meshType, Material material)
		{
			GameObject meshGO = GameObject.CreatePrimitive(meshType);
			meshGO.transform.SetParent(transform);
			meshGO.name = "mesh";
			meshGO.transform.localScale = new Vector3(.5f, .5f, .5f);
			
			meshRenderer = meshGO.GetComponent<MeshRenderer>();
			meshFilter = meshGO.GetComponent<MeshFilter>();
			meshFilter.mesh = HandUtils.GetLandmarkMesh(meshType);
			meshRenderer.materials = new[] { material };
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