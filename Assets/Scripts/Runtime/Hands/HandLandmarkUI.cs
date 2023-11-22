// Copyright © Christian Holm Christensen
// 10/09/2023

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace P307.Runtime.Hands
{
	[RequireComponent(typeof(RectTransform))][DisallowMultipleComponent][AddComponentMenu("307/Hands/Landmark UI")]
	public sealed class HandLandmarkUI : MonoBehaviour
	{
		[SerializeField] Camera cam;
		[SerializeField] Canvas canvas;
		
#if UNITY_EDITOR

		void OnValidate()
		{
			canvas ??= GetComponentInParent<Canvas>();
			
			if (EditorSceneManager.IsPreviewScene(gameObject.scene) is false)
			{
				cam ??= Camera.main;
				canvas.worldCamera = cam;
			}
		}

#endif

	}
}