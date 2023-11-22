// Copyright © Christian Holm Christensen
// 28/09/2023

using System;
using System.Linq;
using UnityEngine;
using static P307.Shared.Const307;

namespace P307.Runtime.Hands.ScriptableObjects
{
	[CreateAssetMenu(fileName = "New Hand Landmark Settings", menuName = "307/Settings/Hand Landmark")]
	public sealed class HandLandmarkSettingsSO : ScriptableObject
	{
		public static Func<bool> getSetColorBasedOnCoordinatesSetting = () => false;
		public static Func<Vector3> getMeshScaleSetting;
		public static Func<int, string> getLandmarkTagOfIndex = _ => string.Empty;
		public static Func<PrimitiveType> getMeshPrimitiveSetting = () => PrimitiveType.Sphere;
		public static Func<Material> getMeshMaterial;
		public static Func<Material> getLineMaterial;
		public static Func<PointAnnotationSpace> getPointAnnotationSpace;
		public static Func<LandmarkPositionSpace> getLandmarkPositionSpace;

		
		[Space(5), Header("Mesh")]
		[SerializeField] bool setMeshMaterialColorBasedOnCoordinates = true;
		[SerializeField] Vector3 meshScale = Vector3.one;
		[SerializeField] Material meshMaterial;
		[SerializeField] PrimitiveType meshPrimitive = PrimitiveType.Sphere;
		
		[Space(5), Header("Other Values")]
		[SerializeField] Material lineMaterial;

		[Space(5), Header("Hand Landmark Tags")]
		[SerializeField] string[] tags = HandUtils.LandmarkTagsOfIndex.Values.ToArray();

		[Space(5), Header("MediaPipe")]
		[SerializeField] LandmarkPositionSpace landmarkPositionSpace = LandmarkPositionSpace.World;
		[SerializeField] LandmarkPositionSpace defaultLandmarkPositionSpace = LandmarkPositionSpace.World;
		[SerializeField] PointAnnotationSpace pointAnnotationSpace = PointAnnotationSpace.Local2WorldPositionMatrix;
		[SerializeField] PointAnnotationSpace defaultPointAnnotationSpace = PointAnnotationSpace.Local2WorldPositionMatrix;

		static LandmarkPositionSpace defaultLmPosSpace = LandmarkPositionSpace.World;
		static PointAnnotationSpace defaultPaSpace = PointAnnotationSpace.WorldPositionVector;
		static LandmarkPositionSpace lmPosSpace = defaultLmPosSpace;
		static PointAnnotationSpace paSpace = defaultPaSpace;

		const string HAND_GREEN_MATERIAL_PATH = "Materials/HandGreen"; // TODO Omdøb
		const string HAND_RED_MATERIAL_PATH = "Materials/HandRed";  // TODO Omdøb

		public bool SetMeshMaterialColorBasedOnCoordinates => setMeshMaterialColorBasedOnCoordinates;
		public Vector3 MeshScale => meshScale;
		public PrimitiveType MeshPrimitive => meshPrimitive;
		
		public static LandmarkPositionSpace LandmarkPositionSpace => lmPosSpace;
		public static PointAnnotationSpace PointAnnotationSpace => paSpace;
		
		Material LineMaterial => lineMaterial ??= Resources.Load<Material>(HAND_GREEN_MATERIAL_PATH);
		Material MeshMaterial => meshMaterial ??= Resources.Load<Material>(HAND_RED_MATERIAL_PATH);
		
#if UNITY_EDITOR
		void OnValidate()
		{
			SetDefaultValues();
		}
#endif
		
		void OnEnable()
		{
			AssignFuncDelegates();
			SetDefaultValues();
		}

		void OnDisable()
		{
			RemoveFuncDelegates();
		}

		void AssignFuncDelegates()
		{
			getPointAnnotationSpace = OnGetPointAnnotationSpace;
			getLandmarkPositionSpace = OnGetLandmarkPositionSpace;
			getSetColorBasedOnCoordinatesSetting = OnGetSetColorBasedOnCoordinatesSetting;
			getLandmarkTagOfIndex = OnGetHandLandmarkTagOfIndex;
			getMeshScaleSetting = OnGetMeshScaleSetting;
			getMeshPrimitiveSetting = OnGetMeshPrimitiveSetting;
			getMeshMaterial = OnGetMeshMaterialSetting;
			getLineMaterial = OnGetLineMaterialSetting;
		}

		void RemoveFuncDelegates()
		{
			getPointAnnotationSpace -= OnGetPointAnnotationSpace;
			getLandmarkPositionSpace -= OnGetLandmarkPositionSpace;
			getSetColorBasedOnCoordinatesSetting -= OnGetSetColorBasedOnCoordinatesSetting;
			getLandmarkTagOfIndex -= OnGetHandLandmarkTagOfIndex;
			getMeshScaleSetting -= OnGetMeshScaleSetting;
			getMeshPrimitiveSetting -= OnGetMeshPrimitiveSetting;
			getMeshMaterial -= OnGetMeshMaterialSetting;
			getLineMaterial -= OnGetLineMaterialSetting;
		}
		
		void SetDefaultValues()
		{
			lmPosSpace = landmarkPositionSpace;
			paSpace = pointAnnotationSpace;
			defaultLmPosSpace = defaultLandmarkPositionSpace;
			defaultPaSpace = defaultPointAnnotationSpace;
		}

		LandmarkPositionSpace OnGetLandmarkPositionSpace() => landmarkPositionSpace;

		bool OnGetSetColorBasedOnCoordinatesSetting() => setMeshMaterialColorBasedOnCoordinates;
		Vector3 OnGetMeshScaleSetting() => meshScale;
		PrimitiveType OnGetMeshPrimitiveSetting() => meshPrimitive;

		string OnGetHandLandmarkTagOfIndex(int index)
		{
			index = Mathf.Clamp(index, ZERO, HAND_LANDMARK_INDICES);
			return tags[index];
		}
		
		PointAnnotationSpace OnGetPointAnnotationSpace() => pointAnnotationSpace;

		Material OnGetMeshMaterialSetting() => MeshMaterial;

		Material OnGetLineMaterialSetting() => LineMaterial;
	}

	public enum LandmarkPositionSpace
	{
		Local,
		World
	}

	public enum PointAnnotationSpace
	{
		LocalPositionVector,
		WorldPositionVector,
		Local2WorldPositionMatrix,
		World2LocalPositionMatrix
	}
}