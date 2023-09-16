// Copyright © Christian Holm Christensen
// 13/09/2023

using System;
using System.Collections.Generic;
using System.Linq;
using P307.Runtime.Hands;
using P307.Runtime.Hands.ScriptableObjects;
using P307.Runtime.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace P307Editor.Hands
{
	[CustomEditor(typeof(HandLandmarkStateHandler))]
	public class HandLandmarkStateHandlerInspector : Editor
	{
		[SerializeField] internal int primitiveTypeIndex;
		[SerializeField] internal PrimitiveType primitiveType = PrimitiveType.Sphere;
		[SerializeField] HandLandmark landmarkPrefab;
		[SerializeField] Material landmarkMaterial;
		[SerializeField] Material lineMaterial;
		
		[NonSerialized] HandLandmarkStateHandler targetStateHandler;

		Label tagLabel;
		Label indexLabel;

		VisualElement landmarkMapContainer;
		
		Dictionary<HandLandmarkSO, VisualElement> landmarkDataViews = new();
		
		const string // ASSET PATHS
			HAND_RED_MATERIAL_PATH = "Materials/HandRed",
			HAND_GREEN_MATERIAL_PATH = "Materials/HandGreen",
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

		Material LandmarkMaterial => landmarkMaterial ??= Resources.Load<Material>(HAND_RED_MATERIAL_PATH);
		Material LineMaterial => lineMaterial ??= Resources.Load<Material>(HAND_GREEN_MATERIAL_PATH);
		HandLandmark LandmarkPrefab => landmarkPrefab ??= Resources.Load<HandLandmark>(HAND_LANDMARK_PREFAB_PATH);
		
		public override VisualElement CreateInspectorGUI()
		{
			targetStateHandler = (HandLandmarkStateHandler)target;
			HandLandmark[] landmarks = targetStateHandler.GetComponentsInChildren<HandLandmark>().ToArray();
			
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
			Dictionary<int, string> landmarkTags = HandUtils.LandmarkTags;
			
			for (int i = 0; i < landmarkTags.Keys.Count; i++)
			{
				Transform stateHandlerTf = targetStateHandler.transform;
				int childCount = stateHandlerTf.childCount;
				bool landmarkFound = false;
				string landmarkTag = $"{i}. {landmarkTags[i]}";
				HandLandmark landmarkComponent;
				
				if (childCount is not 0)
				{
					List<HandLandmark> components = targetStateHandler.LandmarkComponents;
					
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
				landmarkComponent = AddLandmarkChildToStateHandler(stateHandlerTf, i);
				landmarkComponent.MeshFilter.mesh = HandUtils.GetLandmarkMesh(targetStateHandler.MeshType);
				
				if (targetStateHandler.LandmarkComponents.Contains(landmarkComponent))
					continue;
				targetStateHandler.LandmarkComponents.Add(landmarkComponent);
			}
			serializedObject.ApplyModifiedProperties();
		}

		void DestroyChildLandmarkGameObjects()
		{
			targetStateHandler.LandmarkComponents.Clear();
			HandLandmark[] landmarkChildren = targetStateHandler.GetComponentsInChildren<HandLandmark>();
			int lastIndex = landmarkChildren.Length - 1;
			if (lastIndex <= 0)
				return;
			
			for (int i = lastIndex; i >= 0; i--)
			{
				DestroyImmediate(landmarkChildren[i].gameObject);
			}
		}
		
		HandLandmark AddLandmarkChildToStateHandler(Transform parentTf, int i)
		{
			HandLandmark landmark = Instantiate(LandmarkPrefab);
			landmark.Init(i, primitiveType, LandmarkMaterial, LineMaterial);
			
			Transform landmarkTf = landmark.transform;
			landmarkTf.SetParent(parentTf, false);
			landmarkTf.SetSiblingIndex(Mathf.Clamp(i, i, parentTf.childCount));
			
			return landmark;
		}
		
		/*VisualElement CreateLandmarkDataView(int landmarkIndex)
		{
			VisualElement view = new VisualElement()
			{
				style =
				{
					alignContent = new StyleEnum<Align>(Align.Stretch),
					flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
					paddingBottom = 5,
				}
			};
			var tagLabel = new Label("Tag: " + HandUtils.LandmarkTags[landmarkIndex]);
			var indexLabel = new Label("Index: " + landmarkIndex);
			
			view.Add(tagLabel);
			view.Add(indexLabel);
			return view;
		}*/
		
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
				button.RegisterCallback<MouseLeaveEvent>(ClearLabels);
			}
			container.Add(landmarkMapContainer);
		}

		void UpdateLabels(MouseOverEvent @event)
		{
			if (@event.target is not Button button)
				return;
			
			const int start_index = 2;
			const int first_substring_length = 1;
			string index = button.name[..first_substring_length];
			string tag = button.name[start_index..];
			
			SetMapValueLabels(index, tag);
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