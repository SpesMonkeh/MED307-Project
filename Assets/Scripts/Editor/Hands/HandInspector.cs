// Copyright © Christian Holm Christensen
// 10/09/2023

using System;
using System.Collections.Generic;
using System.Linq;
using P307.Runtime.Hands;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace P307Editor.Hands
{
	[CustomEditor(typeof(Hand))]
	public class HandInspector : Editor
	{
		[SerializeField] HandLandmarkStateHandler handLandmarkStateHandler;
		
		const string HAND_LANDMARK_STATE_HANDLER_PATH = "Hand/HandLandmarkStateHandler";

		HandLandmarkStateHandler HandLandmarkStateHandler =>
			handLandmarkStateHandler ??= Resources.Load<HandLandmarkStateHandler>(HAND_LANDMARK_STATE_HANDLER_PATH);
		
		public override VisualElement CreateInspectorGUI()
		{
			return HandInspectorGUI.DrawGUI(this);
		}

		internal void InstantiateLandmarksButtonClicked()
		{
		}
	}
}