// Copyright © Christian Holm Christensen
// 24/09/2023

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace P307.Runtime.Objects
{
	[CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)]
	public class TransformRecordingSO : ScriptableObject
	{
		[Header("Clip Settings")]
		[SerializeField] bool useLocalSpace = false;
		[SerializeField, Range(1, 10)] int lengthInSeconds = 5;
		[SerializeField] TransformRecordingUpdateMethod updateMethod = TransformRecordingUpdateMethod.FixedUpdate;

		[Space(5), Header("Position")]
		[SerializeField] bool recordX = true;
		[SerializeField] bool recordY = true;
		[SerializeField] bool recordZ = true;
		
		[SerializeField] Vector3[] positionFrames = { };

		Dictionary<int, Vector3> recordedPositions = new();

		WaitForSecondsRealtime WaitForSeconds => new(lengthInSeconds);
		
		public IEnumerator RecordTransform(GameObject g)
		{
			int startSecond = DateTime.Now.Second;
			int currentSecond;
			int frame = 0;
			Transform tf = g.transform;
			
			recordedPositions.Clear();
			
			do
			{
				recordedPositions.TryAdd(frame++, useLocalSpace is false ? tf.position : tf.localPosition);
				currentSecond = DateTime.Now.Second;
				yield return new WaitForEndOfFrame();
			}
			while (lengthInSeconds >= Mathf.Abs(currentSecond - startSecond));
		}
	}

	public enum TransformRecordingUpdateMethod
	{
		Update,
		FixedUpdate,
		LateUpdate
	}
}