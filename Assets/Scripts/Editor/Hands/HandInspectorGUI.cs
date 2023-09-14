// Copyright © Christian Holm Christensen
// 11/09/2023

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace P307Editor.Hands
{
	internal sealed class HandInspectorGUI
	{
		const string LANDMARKS = "Landmarks";
		const string INSTANTIATE_MISSING_LANDMARKS = "Instantiate " + LANDMARKS;

		internal static VisualElement DrawGUI(HandInspector editor)
		{
			SerializedObject serializedEditor = new SerializedObject(editor);
			List<string> primitiveTypesList = Enum.GetNames(typeof(PrimitiveType)).ToList();
				
			DropdownField landmarkMeshDropdown = new DropdownField("Landmark Mesh", primitiveTypesList, 0);
			SerializedProperty serializedIndex = serializedEditor.FindProperty("primitiveType");
			
			landmarkMeshDropdown.RegisterValueChangedCallback(_ => serializedIndex.enumValueIndex = landmarkMeshDropdown.index);
			var instantiateLandmarksButton = new Button(editor.InstantiateLandmarksButtonClicked)
			{
				text = INSTANTIATE_MISSING_LANDMARKS
			};

			var imguiContainer = new IMGUIContainer(()=> editor.DrawDefaultInspector());
			VisualElement inspectorGUI = new VisualElement()
			{
				style =
				{
					flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column)
				}
			};
			inspectorGUI.Add(landmarkMeshDropdown);
			inspectorGUI.Add(instantiateLandmarksButton);
			inspectorGUI.Add(imguiContainer);
			return inspectorGUI;
		}
	}
}