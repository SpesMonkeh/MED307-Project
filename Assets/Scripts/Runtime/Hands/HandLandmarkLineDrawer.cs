// Copyright © Christian Holm Christensen
// 14/09/2023

using System;
using System.Collections.Generic;
using UnityEngine;
using static P307.Runtime.Hands.HandUtils;

namespace P307.Runtime.Hands
{
	[AddComponentMenu("307/Hands/Line Drawer")]
	public sealed class HandLandmarkLineDrawer : MonoBehaviour
	{
		[Header("General Line Settings")]
		[SerializeField, Range(ZERO, 5)] int lineCornerVertices = 2;
		[SerializeField, Range(ZERO, 5)] int  lineEndVertices = 1;
		
		[Space(5), Header("Components")]
		[SerializeField] LineRenderer palmLine;
		[SerializeField] LineRenderer thumbLine;
		[SerializeField] LineRenderer indexFingerLine;
		[SerializeField] LineRenderer middleFingerLine;
		[SerializeField] LineRenderer ringFingerLine;
		[SerializeField] LineRenderer pinkyLine;
		[SerializeField] HandLandmarkPositionController landmarkPositionController;

		const int ZERO = 0;
		
		void Awake()
		{
			landmarkPositionController = transform.parent.GetComponentInChildren<HandLandmarkPositionController>();
			
			//SetLineValues();
			//PalmUpdate();
			//ThumbUpdate();
			//IndexFingerUpdate();
			//MiddleFingerUpdate();
			//RingFingerUpdate();
			//PinkyUpdate();
		}

		void SetLineValues()
		{
			palmLine.loop = true;
			
			SetLineCornerVertices();
			SetLineEndVertices();
			SetPositionCounts();
		}

		void SetLineCornerVertices()
		{
			palmLine.numCornerVertices = lineCornerVertices;
			thumbLine.numCornerVertices = lineCornerVertices;
			indexFingerLine.numCornerVertices = lineCornerVertices;
			middleFingerLine.numCornerVertices = lineCornerVertices;
			ringFingerLine.numCornerVertices = lineCornerVertices;
			pinkyLine.numCornerVertices = lineCornerVertices;
		}

		void SetLineEndVertices()
		{
			palmLine.numCapVertices = lineEndVertices;
			thumbLine.numCapVertices = lineEndVertices;
			indexFingerLine.numCapVertices = lineEndVertices;
			middleFingerLine.numCapVertices = lineEndVertices;
			ringFingerLine.numCapVertices = lineEndVertices;
			pinkyLine.numCapVertices = lineEndVertices;
		}

		void SetPositionCounts()
		{
			palmLine.positionCount = PalmIndices.Length;
			thumbLine.positionCount = ThumbIndices.Length;
			indexFingerLine.positionCount = IndexFingerIndices.Length;
			middleFingerLine.positionCount = MiddleFingerIndices.Length;
			ringFingerLine.positionCount = RingFingerIndices.Length;
			pinkyLine.positionCount = PinkyIndices.Length;
		}

		//void Update()
		//{
		//	PalmUpdate();
		//	ThumbUpdate();
		//	IndexFingerUpdate();
		//	MiddleFingerUpdate();
		//	RingFingerUpdate();
		//	PinkyUpdate();
		//}
		
		void UpdateFinger(ref LineRenderer line, IReadOnlyList<int> landmarkIndices)
		{
			if (landmarkPositionController == null)
				throw new NullReferenceException($"Tried to update line positions for {line.name}, but {nameof(HandLandmarkPositionController)} was null!");
			if (line == null)
				throw new NullReferenceException($"Tried to update line positions for {nameof(LineRenderer)}, but it was null!");
			
			for (int i = ZERO; i <= line.positionCount; i++)
			{
				if (i >= landmarkIndices.Count)
					break;
				
				Vector3 connectionPos = landmarkPositionController.GetLandmark(landmarkIndices[i]).transform.localPosition;
				line.SetPosition(i, connectionPos);
			}
		}

		void PalmUpdate() => UpdateFinger(ref palmLine, PalmIndices);
		void ThumbUpdate() => UpdateFinger(ref thumbLine, ThumbIndices);
		void IndexFingerUpdate() => UpdateFinger(ref indexFingerLine, IndexFingerIndices);
		void MiddleFingerUpdate() => UpdateFinger(ref middleFingerLine, MiddleFingerIndices);
		void RingFingerUpdate() => UpdateFinger(ref ringFingerLine, RingFingerIndices);
		void PinkyUpdate() => UpdateFinger(ref pinkyLine, PinkyIndices);
	}
}