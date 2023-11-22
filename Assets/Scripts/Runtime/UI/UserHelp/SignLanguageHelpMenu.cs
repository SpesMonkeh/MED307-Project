// Copyright © Christian Holm Christensen
// 11/10/2023

using UnityEngine;
using UnityEngine.UIElements;

namespace P307.Runtime.UI.UserHelp
{
	[DisallowMultipleComponent][RequireComponent(typeof(UIDocument))]
	public sealed class SignLanguageHelpMenu : MonoBehaviour
	{
		[SerializeField] UIDocument uiDocument;

		StyleSheet styleSheet;
		VisualElement root;

		void Awake()
		{
			uiDocument = GetComponent<UIDocument>();
			root = uiDocument.rootVisualElement;
		}

		void Start()
		{
			CreateUI();
		}

		void CreateUI()
		{
			Label uiTitleLabel = new Label("Ordbog")
			{
				style =
				{
					flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
					justifyContent = new StyleEnum<Justify>(Justify.Center),
					fontSize = 18
				}
			};
			root.Add(uiTitleLabel);
		}
	}
}