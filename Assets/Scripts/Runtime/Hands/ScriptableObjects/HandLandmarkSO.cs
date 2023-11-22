// Copyright © Christian Holm Christensen
// 10/09/2023

using System;
using UnityEngine;
using static P307.Runtime.Hands.HandUtils;
using static P307.Shared.Const307;

namespace P307.Runtime.Hands.ScriptableObjects
{
	[CreateAssetMenu(fileName = "new HandLandmarkSO", menuName = "307/Hands/Landmark")]
	public class HandLandmarkSO : ScriptableObject
	{
		[Header("Colors and Materials")]
		[SerializeField] Color pointLightColor = Color.cyan;
		[SerializeField] Color lineStartColor = Color.blue;
		[SerializeField] Color lineEndColor = Color.black;
		[SerializeField] Color meshColor = Color.cyan;
		[SerializeField] Material lineMaterial;
		[SerializeField] Material meshMaterial;
		
		[Space(5), Header("Identification and Relations")]
		[SerializeField] int index;
		[SerializeField] string tag;
		[SerializeField] int[] connections = {};
		
		[NonSerialized] Vector3 meshLocalScale = new(.25f, .25f, .25f);

		public string Tag => tag;

		public void Init(int lmIndex, string lmTag, int[] connectedIndices)
		{
			index = lmIndex;
			tag = lmTag;
			connections = LineConnectionIndicesOfIndex[lmIndex];
		}

		public static HandLandmarkSO Create(int index)
		{
			index = Mathf.Clamp(value: index, min: ZERO, max: HAND_LANDMARK_INDICES);
			HandLandmarkSO so = CreateInstance<HandLandmarkSO>();
			so.Init(index, LandmarkTagsOfIndex[index], LineConnectionIndicesOfIndex[index]);
			return so;
		}
	}
}