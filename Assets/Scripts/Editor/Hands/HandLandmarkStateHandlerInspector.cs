// Copyright © Christian Holm Christensen
// 13/09/2023

using System;
using System.Collections.Generic;
using System.Linq;
using P307.Runtime.Hands;
using P307.Runtime.Hands.ScriptableObjects;
using P307.Runtime.Utils;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace P307Editor.Hands
{
	[CustomEditor(typeof(HandLandmarkStateHandler))]
	public class HandLandmarkStateHandlerInspector : Editor
	{
		[SerializeField] internal int primitiveTypeIndex;
		[SerializeField] internal PrimitiveType primitiveType = PrimitiveType.Sphere;
		
		Dictionary<HandLandmarkSO, VisualElement> landmarkValuesViews = new();

		const string LANDMARKS = "Landmarks";
		const string HAND_RED_MATERIAL_PATH = "Materials/HandRed";
		const string HAND_GREEN_MATERIAL_PATH = "Materials/HandGreen";
		
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

			//AddHandLandmarkValuePackageViews(ref container);
			
			return container;
		}

		void AddHandLandmarkValuePackageViews(ref VisualElement container)
		{
			landmarkValuesViews.Clear();
			
			HandLandmark[] landmarks = target.GetComponentsInChildren<HandLandmark>().ToArray();

			Dictionary<int, string> landmarkTags = HandUtils.LandmarkTags;
			Dictionary<int, int[]> connections = HandUtils.ConnectionsToIndex;
			
			
			
			for (int i = 0; i < landmarks.Length; i++)
			{
				var landmark = landmarks[i];
				if (landmark.Values == null)
				{
					landmark.Values = HandLandmarkSO.Create(i, landmarkTags[i], connections[i], landmark);
				}

				var viewElement = CreateLandmarkValuesPackageView(landmarkTags[i], i);
				landmarkValuesViews.Add(landmark.Values, viewElement);
			}

			foreach (KeyValuePair<HandLandmarkSO, VisualElement> pair in landmarkValuesViews)
			{
				container.Add(pair.Value);
			}
		}

		VisualElement CreateLandmarkValuesPackageView(string landmarkTag, int landmarkIndex)
		{
			VisualElement view = new VisualElement()
			{
				style =
				{
					alignContent = new StyleEnum<Align>(Align.Stretch),
					flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
				}
			};
			VisualElement tagLabel = new Label("Tag: " + landmarkTag);
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
			Transform transform = stateHandler.transform;

			stateHandler.LandmarkComponents.Clear();
			Dictionary<int, string> landmarkTags = HandUtils.LandmarkTags; 
			
			for (int i = 0; i < landmarkTags.Keys.Count; i++)
			{
				Transform stateHandlerTf = stateHandler.transform;
				int containerChildCount = stateHandlerTf.childCount;
				bool landmarkFound = false;
				string landmarkTag = $"{i}. {landmarkTags[i]}";
				Transform landmark;
				HandLandmark landmarkComponent;
				
				if (containerChildCount is not 0)
				{
					for (int j = 0; j < containerChildCount; j++)
					{
						landmark = stateHandlerTf.GetChild(j);
						landmarkFound = landmark.name.Contains(landmarkTag);
						landmarkComponent = landmarkFound ? landmark.GetComponent<HandLandmark>() : null;
						if (landmarkComponent != null && stateHandler.LandmarkComponents.Contains(landmarkComponent) is false)
							stateHandler.LandmarkComponents.Add(landmarkComponent);
						if (landmarkFound)
							break;
					}
					if (landmarkFound)
						continue;
				}

				const string mesh = "mesh";
				
				landmark = AddLandmarkChildToStateHandler(landmarkTag, stateHandlerTf, i);

				var landmarkMaterial = Resources.Load<Material>(HAND_RED_MATERIAL_PATH);
				var lineMaterial = Resources.Load<Material>(HAND_GREEN_MATERIAL_PATH);
				PrimitiveType primitive = Enum.GetValues(typeof(PrimitiveType)).Cast<PrimitiveType>().ToArray()[primitiveTypeIndex];
				GameObject meshGO = GameObject.CreatePrimitive(primitive);
				meshGO.GetComponent<MeshRenderer>().SetMaterials(new List<Material> { landmarkMaterial });
				meshGO.name = mesh;
				Transform meshTf = meshGO.transform; 
				meshTf.SetParent(landmark, false);
				meshTf.localScale = new Vector3(.5f, .5f, .5f);
				
				landmarkComponent = landmark.GetComponent<HandLandmark>();
				landmarkComponent.MeshFilter.mesh = HandUtils.GetLandmarkMesh(primitiveType);
				if (stateHandler.LandmarkComponents.Contains(landmarkComponent))
					continue;
				
				landmarkComponent.Init(i, landmarkTags[i], lineMaterial);
				stateHandler.LandmarkComponents.Add(landmarkComponent);
			}
			serializedObject.ApplyModifiedProperties();
		}

		static Transform AddLandmarkChildToStateHandler(string landmarkTag, Transform tf, int i)
		{
			Transform landmarkTf = new GameObject(landmarkTag, typeof(HandLandmark)).transform;
			landmarkTf.SetParent(tf, false);
			landmarkTf.SetSiblingIndex(Mathf.Clamp(i, i, tf.childCount));
			var landmark = landmarkTf.GetComponent<HandLandmark>();
			landmark.Values = HandLandmarkSO.Create(i, HandUtils.LandmarkTags[i], HandUtils.ConnectionsToIndex[i], landmark);
			return landmarkTf;
		}
	}
}