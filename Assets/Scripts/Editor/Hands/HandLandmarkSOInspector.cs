// Copyright © Christian Holm Christensen
// 10/09/2023

using P307.Runtime.Hands.ScriptableObjects;
using P307.Shared;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace P307Editor.Hands
{
	[CustomEditor(typeof(HandLandmarkSO))]
	public class HandLandmarkSOInspector : Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			VisualElement container = new VisualElement()
			{
				style =
				{
					flexGrow = 1,
					flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column)
				}
			};
			container.Add(Utils307.P307Label());
			
			SerializedProperty tagProperty = serializedObject.FindProperty("tag");
			SerializedProperty indexProperty = serializedObject.FindProperty("id");

			var tagField = new PropertyField(tagProperty);
			var indexField = new PropertyField(indexProperty);
			
			container.Add(tagField);
			container.Add(indexField);
			return container;
		}
	}
}