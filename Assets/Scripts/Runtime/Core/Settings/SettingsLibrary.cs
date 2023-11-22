// Copyright © Christian Holm Christensen
// 28/09/2023

using System;
using P307.Runtime.Hands.ScriptableObjects;
using UnityEngine;

namespace P307.Runtime.Core.Settings
{
	[DisallowMultipleComponent][AddComponentMenu("307/Core/Settings Library")]
	public sealed class SettingsLibrary : MonoBehaviour, IP307CoreComponent
	{
		public static Func<HandLandmarkSettingsSO> getHandLandmarkSettings;
		
		[SerializeField] HandLandmarkSettingsSO handLandmarkSettings;

		[Space(5), Header("Templates")]
		[SerializeField] HandLandmarkSettingsSO handLandmarkSettingsTemplate;
		
		void OnEnable()
		{
			getHandLandmarkSettings = OnGetHandLandmarkSettings;
		}

		void OnDisable()
		{
			getHandLandmarkSettings -= OnGetHandLandmarkSettings;
		}

		HandLandmarkSettingsSO OnGetHandLandmarkSettings()
		{
			if (handLandmarkSettings != null)
				return handLandmarkSettings;
			
			handLandmarkSettingsTemplate ??= Resources.Load<HandLandmarkSettingsSO>("Settings/Templates");
			handLandmarkSettings = Instantiate(handLandmarkSettingsTemplate);
			return handLandmarkSettings;
		}

		void Awake()
		{
			handLandmarkSettings = OnGetHandLandmarkSettings();
		}

		public bool Initialize()
		{
			handLandmarkSettings = OnGetHandLandmarkSettings();
			Debug.Log($"{nameof(GetType)} was initialized.");
			return handLandmarkSettings != null;
		}
	}
}