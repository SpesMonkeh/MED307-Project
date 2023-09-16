// Copyright © Christian Holm Christensen
// 10/09/2023

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace P307.Runtime.Hands
{
/*
 	HAND LANDMARKS:
 	[INDEX]. [ID]
 	0. WRIST
 	1. THUMB_CMC
 	2. THUMB_MCP
 	3. THUMB_IP
 	4. THUMB_TIP
 	5. INDEX_FINGER_MCP
 	6. INDEX_FINGER_PIP
 	7. INDEX_FINGER_DIP
 	8. INDEX_FINGER_TIP
 	9. MIDDLE_FINGER_MCP
 	10. MIDDLE_FINGER_PIP
 	11. MIDDLE_FINGER_DIP
 	12. MIDDLE_FINGER_TIP
 	13. RING_FINGER_MCP
 	14. RING_FINGER_PIP
 	15. RING_FINGER_DIP
 	16. RING_FINGER_TIP
 	17. PINKY_MCP
 	18. PINKY_PIP
 	19. PINKY_DIP
 	20. PINKY_TIP
*/

	[DisallowMultipleComponent]
	public sealed class Hand : MonoBehaviour
	{
		[SerializeField] HandLandmark handLandmarkPrefab;
		[SerializeField] GameObject landmarkContainer;
		[SerializeField] List<HandLandmark> landmarks;
		[SerializeField] PrimitiveType landmarkPrimitive;
		
		public List<HandLandmark> Landmarks => landmarks ??= GetLandmarksInContainer();
		
		List<HandLandmark> GetLandmarksInContainer() => transform.GetChild(0).GetComponentsInChildren<HandLandmark>().ToList();
	}
}