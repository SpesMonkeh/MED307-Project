// Copyright © Christian Holm Christensen
// 10/09/2023

using System.Collections.Generic;
using UnityEngine;
using static P307.Shared.Const307;
using Object = UnityEngine.Object;

namespace P307.Runtime.Hands
{
	public static class HandUtils
	{
		public const int HAND_POINT_COUNT = 21;
		public const int
			WRIST_INDEX = 0,
			
			THUMB_CMC_INDEX = 1,
			THUMB_MCP_INDEX = 2,
			THUMB_IP_INDEX = 3,
			THUMB_TIP_INDEX = 4,
			
			INDEX_FINGER_MCP_INDEX = 5,
			INDEX_FINGER_PIP_INDEX = 6,
			INDEX_FINGER_DIP_INDEX = 7,
			INDEX_FINGER_TIP_INDEX = 8,
			
			MIDDLE_FINGER_MCP_INDEX = 9,
			MIDDLE_FINGER_PIP_INDEX = 10,
			MIDDLE_FINGER_DIP_INDEX = 11,
			MIDDLE_FINGER_TIP_INDEX = 12,
			
			RING_FINGER_MCP_INDEX = 13,
			RING_FINGER_PIP_INDEX = 14,
			RING_FINGER_DIP_INDEX = 15,
			RING_FINGER_TIP_INDEX = 16,
			
			PINKY_MCP_INDEX = 17,
			PINKY_PIP_INDEX = 18,
			PINKY_DIP_INDEX = 19,
			PINKY_TIP_INDEX = 20;
		
		const string
			WRIST = "WRIST",
			THUMB = "THUMB",
			INDEX_FINGER = "INDEX_FINGER",
			MIDDLE_FINGER = "MIDDLE_FINGER",
			RING_FINGER = "RING_FINGER",
			PINKY = "PINKY";
		
		const string
			CMC = "_CMC",
			MCP = "_MCP",
			IP = "_IP",
			DIP = "_DIP",
			PIP = "_PIP",
			TIP = "_TIP";

		public static readonly int[] PalmLineIndices =
		{
			WRIST_INDEX,
			PINKY_MCP_INDEX,
			RING_FINGER_MCP_INDEX,
			MIDDLE_FINGER_MCP_INDEX,
			INDEX_FINGER_MCP_INDEX,
			WRIST_INDEX,
			THUMB_CMC_INDEX
		};

		public static readonly int[] PalmIndices =
		{
			WRIST_INDEX,
			THUMB_CMC_INDEX,
			INDEX_FINGER_MCP_INDEX,
			MIDDLE_FINGER_MCP_INDEX,
			RING_FINGER_MCP_INDEX,
			PINKY_MCP_INDEX,
		};

		public static readonly int[] ThumbIndices =
		{
			THUMB_CMC_INDEX,
			THUMB_MCP_INDEX,
			THUMB_IP_INDEX,
			THUMB_TIP_INDEX
		};
		
		public static readonly int[] IndexFingerIndices =
		{
			INDEX_FINGER_MCP_INDEX,
			INDEX_FINGER_PIP_INDEX,
			INDEX_FINGER_DIP_INDEX,
			INDEX_FINGER_TIP_INDEX
		};
		
		public static readonly int[] MiddleFingerIndices =
		{
			MIDDLE_FINGER_MCP_INDEX,
			MIDDLE_FINGER_PIP_INDEX,
			MIDDLE_FINGER_DIP_INDEX,
			MIDDLE_FINGER_TIP_INDEX
		};
		
		public static readonly int[] RingFingerIndices =
		{
			RING_FINGER_MCP_INDEX,
			RING_FINGER_PIP_INDEX,
			RING_FINGER_DIP_INDEX,
			RING_FINGER_TIP_INDEX
		};
		
		public static readonly int[] PinkyIndices =
		{
			PINKY_MCP_INDEX,
			PINKY_PIP_INDEX,
			PINKY_DIP_INDEX,
			PINKY_TIP_INDEX
		};
		
		public static Dictionary<int, int[]> IndexDict { get; }= new()
		{
			{ ZERO,			PalmIndices }			,
			{ ONE,			ThumbIndices }			,
			{ FIVE,			IndexFingerIndices }	,
			{ NINE,			MiddleFingerIndices }	,
			{ THIRTEEN,		RingFingerIndices }		,
			{ SEVENTEEN,	PinkyIndices }
		};
		
		public static Dictionary<int, string> LandmarkTagsOfIndex { get; } = new()
		{
			// 0
			{ WRIST_INDEX,				WRIST }					,
			// 1 - 4
			{ THUMB_CMC_INDEX,			THUMB + CMC }			,
			{ THUMB_MCP_INDEX,			THUMB + MCP }			,
			{ THUMB_IP_INDEX,			THUMB + IP }			,			
			{ THUMB_TIP_INDEX,			THUMB + TIP }			,
			// 5 - 8
			{ INDEX_FINGER_MCP_INDEX,	INDEX_FINGER + MCP }	,
			{ INDEX_FINGER_PIP_INDEX,	INDEX_FINGER + PIP }	,
			{ INDEX_FINGER_DIP_INDEX,	INDEX_FINGER + DIP }	,
			{ INDEX_FINGER_TIP_INDEX,	INDEX_FINGER + TIP }	,
			// 9 - 12
			{ MIDDLE_FINGER_MCP_INDEX,	MIDDLE_FINGER + MCP }	,
			{ MIDDLE_FINGER_PIP_INDEX,	MIDDLE_FINGER + PIP }	,
			{ MIDDLE_FINGER_DIP_INDEX,	MIDDLE_FINGER + DIP }	,
			{ MIDDLE_FINGER_TIP_INDEX,	MIDDLE_FINGER + TIP }	,
			// 13 - 16
			{ RING_FINGER_MCP_INDEX,	RING_FINGER + MCP }		,
			{ RING_FINGER_PIP_INDEX,	RING_FINGER + PIP }		,
			{ RING_FINGER_DIP_INDEX,	RING_FINGER + DIP }		,
			{ RING_FINGER_TIP_INDEX,	RING_FINGER + TIP }		,
			// 17 - 20
			{ PINKY_MCP_INDEX,			PINKY + MCP }			,
			{ PINKY_PIP_INDEX,			PINKY + PIP }			,
			{ PINKY_DIP_INDEX,			PINKY + DIP }			,
			{ PINKY_TIP_INDEX,			PINKY + TIP }			,
		};

		public static Dictionary<int, int[]> LineConnectionIndicesOfIndex { get; } = new()
		{
			// int = index
			// int[] = line connection indices
			
			// WRIST
			{ 0, new[] { 1, 5, 7 } },

			// THUMB
			{ 1, new[] { 0, 2 } },
			{ 2, new[] { 1, 3 } },
			{ 3, new[] { 2, 4 } },
			{ 4, new[] { 3 } },

			// INDEX
			{ 5, new[] { 0, 6, 9 } },
			{ 6, new[] { 5, 7 } },
			{ 7, new[] { 6, 8 } },
			{ 8, new[] { 7 } },

			// MIDDLE
			{ 9, new[] { 5, 10, 13 } },
			{ 10, new[] { 9, 11 } },
			{ 11, new[] { 10, 12 } },
			{ 12, new[] { 11 } },

			// RING
			{ 13, new[] { 9, 14, 17 } },
			{ 14, new[] { 13, 15 } },
			{ 15, new[] { 14, 16 } },
			{ 16, new[] { 15 } },

			// PINKY
			{ 17, new[] { 0, 13, 18 } },
			{ 18, new[] { 17, 19 } },
			{ 19, new[] { 18, 20 } },
			{ 20, new[] { 19 } }
		};
		
		public static Mesh GetLandmarkMesh(PrimitiveType primitiveType)
		{
			var go = GameObject.CreatePrimitive(primitiveType);
			var mesh = go.GetComponent<MeshFilter>().sharedMesh;
			UnityEngine.Debug.Log("Get Landmark Mesh");
#if UNITY_EDITOR
			Object.DestroyImmediate(go);
#else
			Object.Destroy(go);
#endif
			return mesh;
		}
	}
}