// Copyright © Christian Holm Christensen
// 10/09/2023

using System;
using UnityEngine;

namespace P307.Runtime.Hands.ScriptableObjects
{
	[CreateAssetMenu(fileName = "new HandLandmarkSO", menuName = "P307/Hand/Landmark")]
	public class HandLandmarkSO : ScriptableObject
	{
		[SerializeField] int meshScaleFactor = 1;
		
		[SerializeField] int id;
		[SerializeField] string tag;
		[SerializeField] int[] connectedLandmarks = {};

		[SerializeField] Vector3 startPosition;
		
		[NonSerialized] HandLandmark landmark;
		[NonSerialized] Vector3 currentPosition;

		public Action<Vector3> moveToStartPosition = delegate {  };
		public Action<Vector3> updatePosition = delegate {  };

		public int Id => id;
		public string Tag => tag;
		public int[] ConnectedLandmarks => connectedLandmarks;

		public Vector3 StartPosition
		{
			get => startPosition;
			set
			{
				if (startPosition == value)
					return;
				startPosition = value;
				moveToStartPosition?.Invoke(startPosition);
				moveToStartPosition -= landmark.UpdatePosition;
				updatePosition += landmark.UpdatePosition;
			}
		}

		public Vector3 CurrentPosition
		{
			get => currentPosition;
			set
			{
				if (currentPosition == value)
					return;
				currentPosition = value;
				updatePosition?.Invoke(currentPosition);
			}
		}

		public void Init(int lmIndex, string lmTag, int[] connectedIndices, HandLandmark lm)
		{
			id = lmIndex;
			tag = lmTag;
			connectedLandmarks = connectedIndices;
			landmark = lm;
			updatePosition -= landmark.UpdatePosition;
			moveToStartPosition += landmark.UpdatePosition;
		}

		public void UpdateLandmarkPosition(Vector3 position)
		{
			// TODO S/LERP
			CurrentPosition = position;
		}

		public void MoveToStartPosition() => UpdateLandmarkPosition(StartPosition);

		public static HandLandmarkSO Create(int index, string tag, int[] connectedIndices, HandLandmark lm)
		{
			var lmSO = CreateInstance<HandLandmarkSO>();
			lmSO.Init(index, tag, connectedIndices, lm);
			return lmSO;
		}
	}
}