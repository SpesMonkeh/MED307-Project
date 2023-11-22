// Copyright © Christian Holm Christensen
// 11/10/2023

using System;
using UnityEngine;

namespace P307.Runtime.Core.Cameras
{
	[DisallowMultipleComponent]
	public sealed class P307Cameras : MonoBehaviour
	{
		public Action<bool> toggleFullWebcamCam = delegate { };
		public Action<bool> toggleMiniWebcamCam = delegate { };
		public Action<bool> toggleHandLandmarksCam = delegate { };
		
		[SerializeField] Camera fullWebcamCam;
		[SerializeField] Camera miniWebcamCam;
		[SerializeField] Camera handLandmarksCam;
		// TODO: [SerializeField] Camera faceLandmarksCam;
		// TODO: [SerializeField] Camera poseLandmarksCam;

		void OnEnable()
		{
			SetupCamera(fullWebcamCam, ref toggleFullWebcamCam, OnToggleFullWebcamCam);
			SetupCamera(miniWebcamCam, ref toggleMiniWebcamCam, OnToggleMiniWebcamCam);
			SetupCamera(handLandmarksCam, ref toggleHandLandmarksCam, OnToggleHandLandmarksCam);
		}

		void OnDisable()
		{
			toggleFullWebcamCam -= OnToggleFullWebcamCam;
			toggleMiniWebcamCam -= OnToggleMiniWebcamCam;
			toggleHandLandmarksCam -= OnToggleHandLandmarksCam;			
		}

		void OnToggleHandLandmarksCam(bool toggle) => ToggleCamera(handLandmarksCam, toggle);
		void OnToggleMiniWebcamCam(bool toggle) => ToggleCamera(miniWebcamCam, toggle);
		void OnToggleFullWebcamCam(bool toggle) => ToggleCamera(fullWebcamCam, toggle);
		
		static void ToggleCamera(Camera cam, bool toggle) => cam.enabled = toggle;

		static void SetupCamera(Camera cam, ref Action<bool> action, Action<bool> onAction)
		{
			if (cam != null)
				action += onAction;
		}
	}
}