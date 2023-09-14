// Copyright © Christian Holm Christensen
// 10/09/2023

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace P307.Runtime.Hands
{
	[DisallowMultipleComponent]
	public sealed class HandPointUI : MonoBehaviour
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