// Copyright © Christian Holm Christensen
// 13/09/2023

using System;
using System.Collections.Generic;
using System.Linq;
using P307.Runtime.Hands;
using P307.Shared;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static P307.Shared.Const307;

namespace P307Editor.Hands
{
	//[CustomEditor(typeof(HandLandmarkPositionController))]
	public class HandLandmarkPositionControllerInspector : Editor
	{
		[SerializeField] internal int primitiveTypeIndex;
		[SerializeField] internal PrimitiveType primitiveType = PrimitiveType.Sphere;
		[SerializeField] HandLandmark landmarkPrefab;
		
		[NonSerialized] HandLandmarkPositionController targetPositionController;

		Label tagLabel;
		Label indexLabel;

		VisualElement landmarkMapContainer;
		
		const string // ASSET PATHS
			HAND_LANDMARK_PREFAB_PATH = "Hands/Hand Point",
			HAND_MAP_ELEMENT_UXML_PATH = "Assets/UI Toolkit/Hands/Editor/Resources/hand_map_element_editor.uxml";
		
		const string // SERIALIZED PROPERTY NAMES
			MESH_TYPE_PROPERTY = "meshType";
		
		const string // OTHER CONSTANT STRINGS
			REFRESH_LANDMARKS = "Refresh Landmarks",
			LANDMARK_CONTAINER = "LandmarkContainer";
		const string MAP_INDEX_VALUE_LABEL = "Index_value";
		const string MAP_TAG_VALUE_LABEL = "Tag_value";
		const string N_A = "N/A";

		HandLandmark LandmarkPrefab => landmarkPrefab ??= Resources.Load<HandLandmark>(HAND_LANDMARK_PREFAB_PATH);
		
		
		public override VisualElement CreateInspectorGUI()
		{
			targetPositionController = (HandLandmarkPositionController)target;
			HandLandmark[] landmarks = targetPositionController.GetComponentsInChildren<HandLandmark>().ToArray();
			
			CreateInspectorContainer(out VisualElement container);
			AddMeshTypePropertyField(ref container, serializedObject.FindProperty(MESH_TYPE_PROPERTY));
			AddRefreshLandmarksButton(ref container);
			AddLandmarkMapElement(ref container);
			//AddLandmarkDataViews(landmarks, ref container, ref landmarkDataViews);
			
			return container;
		}

		static void AddMeshTypePropertyField(ref VisualElement container, SerializedProperty property)
		{
			PropertyField meshTypeField = new PropertyField(property);
			container.Add(meshTypeField);
		}

		void AddRefreshLandmarksButton(ref VisualElement container)
		{
			Button refreshLandmarksButton = new Button(RefreshLandmarks)
			{
				text = REFRESH_LANDMARKS
			};
			container.Add(refreshLandmarksButton);
		}
        
		void RefreshLandmarks()
		{
			serializedObject.Update();
			DestroyChildLandmarkGameObjects();
			Dictionary<int, string> landmarkTags = HandUtils.LandmarkTagsOfIndex;
			
			for (int i = 0; i < landmarkTags.Keys.Count; i++)
			{
				Transform stateHandlerTf = targetPositionController.transform;
				int childCount = stateHandlerTf.childCount;
				bool landmarkFound = false;
				string landmarkTag = $"{i}. {landmarkTags[i]}";
				HandLandmark landmarkComponent;
				
				if (childCount is not 0)
				{
					List<HandLandmark> components = targetPositionController.LandmarkComponents;
					
					for (int j = 0; j < childCount; j++)
					{
						Transform childTf = stateHandlerTf.GetChild(j);
						landmarkFound = childTf.TryGetComponent(out landmarkComponent) 
						                && landmarkComponent.name.Contains(landmarkTag);
						switch (landmarkFound)
						{
							case false:
								continue;
							case true when components.Contains(landmarkComponent) is false:
								components.Add(landmarkComponent);
								break;
						}
						break;
					}
					if (landmarkFound)
						continue;
				}
				landmarkComponent = AddLandmarkChildToPositionController(stateHandlerTf, i);
				landmarkComponent.MeshFilter.mesh = HandUtils.GetLandmarkMesh(targetPositionController.MeshType);
				
				if (targetPositionController.LandmarkComponents.Contains(landmarkComponent))
					continue;
				targetPositionController.LandmarkComponents.Add(landmarkComponent);
			}
			serializedObject.ApplyModifiedProperties();
		}

		void DestroyChildLandmarkGameObjects()
		{
			targetPositionController.LandmarkComponents.Clear();
			HandLandmark[] landmarkChildren = targetPositionController.GetComponentsInChildren<HandLandmark>();
			int lastIndex = landmarkChildren.Length - 1;
			if (lastIndex <= 0)
				return;
			
			for (int i = lastIndex; i >= 0; i--)
			{
				DestroyImmediate(landmarkChildren[i].gameObject);
			}
		}
		
		HandLandmark AddLandmarkChildToPositionController(Transform parentTf, int i)
		{
			HandLandmark landmark = Instantiate(LandmarkPrefab);
			landmark.Init(i);
			
			Transform landmarkTf = landmark.transform;
			landmarkTf.SetParent(parentTf, false);
			landmarkTf.SetSiblingIndex(Mathf.Clamp(i, i, parentTf.childCount));
			
			return landmark;
		}
		
		VisualElement CreateLandmarkDataView(int landmarkIndex)
		{
			const string landmark_components = "landmarkComponents";
			
			VisualElement view = new VisualElement()
			{
				style =
				{
					alignContent = new StyleEnum<Align>(Align.Stretch),
					flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
					paddingBottom = 5,
				}
			};
			var tag = new Label("Tag: " + HandUtils.LandmarkTagsOfIndex[landmarkIndex]);
			var index = new Label("Index: " + landmarkIndex);
			
			view.Add(tag);
			view.Add(index);

			SerializedProperty components = serializedObject.FindProperty(landmark_components);
			int componentCount = components.arraySize;

			if (landmarkIndex >= componentCount)
				return view;
			
			var component = components.GetArrayElementAtIndex(landmarkIndex);
			var componentData = component.FindPropertyRelative("data");

			PropertyField dataField = new PropertyField(componentData);
			view.Add(dataField);
			
			return view;
		}
		
		void AddLandmarkMapElement(ref VisualElement container)
		{
			VisualTreeAsset mapTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(HAND_MAP_ELEMENT_UXML_PATH);
			landmarkMapContainer = mapTreeAsset.CloneTree();
			IEnumerable<VisualElement> children = landmarkMapContainer.Q(LANDMARK_CONTAINER).Children();
			
			foreach (VisualElement child in children)
			{
				if (child is not Button button)
					continue;
				button.RegisterCallback<MouseOverEvent>(UpdateLabels);
				//button.RegisterCallback<MouseOverEvent>(UpdateLandmarkDataView);
				button.RegisterCallback<MouseLeaveEvent>(ClearLabels);
				
			}
			container.Add(landmarkMapContainer);
		}

		VisualElement CreateLandmarkListView()
		{
			VisualElement container = new VisualElement();
		// TODO
			

			return container;
		}

		void UpdateLandmarkDataView(MouseOverEvent @event)
		{
			if (@event.target is not Button button)
				return;
			int index = GetIndexFromButton(button.name);
			CreateLandmarkDataView(index);
		}

		void UpdateLabels(MouseOverEvent @event)
		{
			if (@event.target is not Button button)
				return;

			int index = GetIndexFromButton(button.name);
			string tag = GetTagFromButton(button.name);
			SetMapValueLabels(index.ToString(), tag);
			
		}

		static int GetIndexFromButton(string buttonName)
		{ 
			const char separator = '_';
			string indexString = buttonName[ONE] is separator
				? buttonName[..ONE]
				: buttonName[..TWO];
			return int.TryParse(indexString, out int index) ? index : -ONE;
		}

		static string GetTagFromButton(string buttonName)
		{
			const char separator = '_';
			int startIndex =  buttonName[ONE] is separator
				? TWO
				: THREE;
			return buttonName[startIndex..];
		}
		
		void ClearLabels(MouseLeaveEvent evt)
		{
			landmarkMapContainer.Q<Label>(MAP_INDEX_VALUE_LABEL).text = N_A;
			landmarkMapContainer.Q<Label>(MAP_TAG_VALUE_LABEL).text = N_A;
		}

		void SetMapValueLabels(string indexValue, string tagValue)
		{
			landmarkMapContainer.Q<Label>(MAP_INDEX_VALUE_LABEL).text = indexValue;
			landmarkMapContainer.Q<Label>(MAP_TAG_VALUE_LABEL).text = tagValue;
		}

		//void AddLandmarkDataViews(
		//	IReadOnlyList<HandLandmark> landmarks,
		//	ref VisualElement container,
		//	ref Dictionary<HandLandmarkSO,VisualElement> dataViewsDictionary
		//) {
		//	dataViewsDictionary.Clear();
		//	for (int i = 0; i < landmarks.Count; i++)
		//	{
		//		var landmark = landmarks[i];
		//		var viewElement = CreateLandmarkDataView(i);
		//		dataViewsDictionary.Add(landmark.Data, viewElement);
		//		container.Add(viewElement);
		//	}
		//}

		void OnDisable()
		{
			if (landmarkMapContainer is null)
				return;
			IEnumerable<VisualElement> children = landmarkMapContainer.Q(LANDMARK_CONTAINER).Children();
			foreach (VisualElement child in children)
			{
				if (child is not Button button)
					continue;
				button.UnregisterCallback<MouseOverEvent>(UpdateLabels);
				button.UnregisterCallback<MouseLeaveEvent>(ClearLabels);
			}
		}

		static void CreateInspectorContainer(out VisualElement container)
		{
			container = new VisualElement
			{
				style =
				{
					flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column),
					marginTop = 0,
					marginBottom = 0,
					marginLeft = 0,
					marginRight = 0,
					paddingTop = 0,
					paddingBottom = 0,
					paddingLeft = 0,
					paddingRight = 0
				}
			};
			container.Add(Utils307.P307Label());
		}
	}
}