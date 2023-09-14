// Copyright © Christian Holm Christensen
// 10/09/2023

using System.Collections.Generic;
using UnityEngine;

namespace P307.Runtime.Hands
{
	public static class HandUtils
	{
		public const int HAND_POINT_COUNT = 21;
		
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
		
		public static Dictionary<int, string> LandmarkTags { get; } = new() {
			{ 0, WRIST },
			{ 1, THUMB + CMC },
			{ 2, THUMB + MCP },
			{ 3, THUMB + IP },
			{ 4, THUMB + TIP },
			{ 5, INDEX_FINGER + MCP },
			{ 6, INDEX_FINGER + PIP },
			{ 7, INDEX_FINGER + DIP },
			{ 8, INDEX_FINGER + TIP },
			{ 9, MIDDLE_FINGER + MCP },
			{ 10, MIDDLE_FINGER + PIP },
			{ 11, MIDDLE_FINGER + DIP },
			{ 12, MIDDLE_FINGER + TIP },
			{ 13, RING_FINGER + MCP },
			{ 14, RING_FINGER + PIP },
			{ 15, RING_FINGER + DIP },
			{ 16, RING_FINGER + TIP },
			{ 17, PINKY + MCP },
			{ 18, PINKY + PIP },
			{ 19, PINKY + DIP },
			{ 20, PINKY + TIP },
		};

		public static Dictionary<int, int[]> ConnectionsToIndex { get; } = new()
		{
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
			HandLandmark[] landmarks = Resources.LoadAll<HandLandmark>("Meshes/Primitives");
			
			for (int i = 0; i < landmarks.Length; i++)
			{
				string primitive = primitiveType.ToString().ToLowerInvariant().Replace("_primitive", string.Empty);
				if (landmarks[i].name.Contains(primitive))
				{
					return landmarks[i].MeshFilter.mesh;
				}
			}
			return null;
		}
		
		public static HandLandmark[] LandmarkMeshes()
		{
			HandLandmark[] landmarks = Resources.LoadAll<HandLandmark>("Meshes/Primitives");
			return landmarks;
		}

		public static void UpdateLandmarkMeshes(List<HandLandmark> landmarks, PrimitiveType primitiveType)
		{
			foreach (var landmark in landmarks)
			{
				landmark.PrimitiveType = primitiveType;
			}
		}
	}
}