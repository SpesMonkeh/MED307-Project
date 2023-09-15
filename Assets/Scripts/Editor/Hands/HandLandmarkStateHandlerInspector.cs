// Copyright © Christian Holm Christensen
// 13/09/2023

using System.Collections.Generic;
using System.Linq;
using P307.Runtime.Hands;
using P307.Runtime.Hands.ScriptableObjects;
using P307.Runtime.Utils;
using Unity.VisualScripting;
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
		
		Dictionary<HandLandmarkSO, VisualElement> landmarkValuesViews = new();

		const string HAND_RED_MATERIAL_PATH = "Materials/HandRed";
		const string HAND_GREEN_MATERIAL_PATH = "Materials/HandGreen";
		const string HAND_LANDMARK_PREFAB_PATH = "Hands/Hand Point";

		Material LandmarkMaterial => landmarkMaterial ??= Resources.Load<Material>(HAND_RED_MATERIAL_PATH);
		Material LineMaterial => lineMaterial ??= Resources.Load<Material>(HAND_GREEN_MATERIAL_PATH);
		
		HandLandmark LandmarkPrefab => landmarkPrefab ??= Resources.Load<HandLandmark>(HAND_LANDMARK_PREFAB_PATH);
		
		public override VisualElement CreateInspectorGUI()
		{
			VisualElement container = new VisualElement()
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
			
			PropertyField meshTypeField = new PropertyField(serializedObject.FindProperty("meshType"));
			container.Add(meshTypeField);
			
			Button refreshLandmarksButton = new Button(RefreshLandmarks)
			{
				text = "Refresh Landmarks"
			};
			container.Add(refreshLandmarksButton);

			VisualElement landmarkMapContainer = new VisualElement()
			{
				style =
				{
					width = 100,
					height = 100,
					flexGrow = 0,
					flexShrink = 0,
					backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f)),
					alignSelf = new StyleEnum<Align>(Align.Center)
				}
			};
			container.Add(landmarkMapContainer);

			AddHandLandmarkValuesViews(ref container);
			return container;
		}

		void AddHandLandmarkValuesViews(ref VisualElement container)
		{
			landmarkValuesViews.Clear();
			
			HandLandmark[] landmarks = target.GetComponentsInChildren<HandLandmark>().ToArray();

			for (int i = 0; i < landmarks.Length; i++)
			{
				var landmark = landmarks[i];
				var viewElement = CreateLandmarkValuesPackageView(i);
				landmarkValuesViews.Add(landmark.Values, viewElement);
			}

			foreach (KeyValuePair<HandLandmarkSO, VisualElement> pair in landmarkValuesViews)
			{
				container.Add(pair.Value);
			}
		}

		VisualElement CreateLandmarkValuesPackageView(int landmarkIndex)
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
			VisualElement tagLabel = new Label("Tag: " + HandUtils.LandmarkTags[landmarkIndex]);
			VisualElement indexLabel = new Label("Index: " + landmarkIndex);
			
			view.Add(tagLabel);
			view.Add(indexLabel);
			return view;
		}
        
		void RefreshLandmarks()
		{
			if (target is not HandLandmarkStateHandler stateHandler)
				return;
			serializedObject.Update();
			
			ClearChildLandmarkGameObjects(stateHandler);

			Dictionary<int, string> landmarkTags = HandUtils.LandmarkTags; 
			for (int i = 0; i < landmarkTags.Keys.Count; i++)
			{
				Transform stateHandlerTf = stateHandler.transform;
				int containerChildCount = stateHandlerTf.childCount;
				bool landmarkFound = false;
				string landmarkTag = $"{i}. {landmarkTags[i]}";
				HandLandmark landmarkComponent;
				
				if (containerChildCount is not 0)
				{
					for (int j = 0; j < containerChildCount; j++)
					{ 
						landmarkComponent = stateHandlerTf.GetChild(j)?.GetComponent<HandLandmark>();
						landmarkFound = landmarkComponent != null
						                && landmarkComponent.name.Contains(landmarkTag);
						if (landmarkComponent != null 
						    && stateHandler.LandmarkComponents.Contains(landmarkComponent) is false)
							stateHandler.LandmarkComponents.Add(landmarkComponent);
						if (landmarkFound)
							break;
					}
					if (landmarkFound)
						continue;
				}
				landmarkComponent = AddLandmarkChildToStateHandler(stateHandlerTf, i);
				landmarkComponent.MeshFilter.mesh = HandUtils.GetLandmarkMesh(stateHandler.MeshType);
				
				if (stateHandler.LandmarkComponents.Contains(landmarkComponent))
					continue;
				stateHandler.LandmarkComponents.Add(landmarkComponent);
			}
			serializedObject.ApplyModifiedProperties();
		}

		void ClearChildLandmarkGameObjects(HandLandmarkStateHandler stateHandler)
		{
			stateHandler.LandmarkComponents.Clear();
			for (int i = stateHandler.transform.childCount - 1; i >= 0; i--)
			{
				DestroyImmediate(stateHandler.transform.GetChild(i).gameObject);
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
	}
}