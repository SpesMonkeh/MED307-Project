// Copyright © Christian Holm Christensen
// 12/10/2023

using P307.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static P307.Shared.Const307;
using TMP_Text = TMPro.TMP_Text;

namespace P307.Runtime.UI.Common
{
	[RequireComponent(typeof(Button))][AddComponentMenu("307/UI/uGUI/Button307")]
	public sealed class P307Button : MonoBehaviour
	{
		[SerializeField] string textOnButton = THREE_DOTS;
		
		[Space(10)]
		[SerializeField] ButtonObjectDisplayType display = ButtonObjectDisplayType.Text;
		[SerializeField] Image buttonImage;
		[SerializeField] TMP_Text buttonText;
		// TODO: [SerializeField] TMP_Text tooltip;

		const string IMAGE = "Image";
		const string TEXT = "Text";
		const string TEXT_MISSING = "?!";
		const string THREE_DOTS = "...";

		void SetButtonText(string newText)
		{
			if (buttonText == null || buttonText.text == newText)
				return;
			buttonText.text = string.IsNullOrEmpty(newText) is false ? newText : TEXT_MISSING;
		}
		
		void Awake()
		{
			Init(textOnButton);	
		}

		void Init(string text = "")
		{
			InitImage();
			InitText(text);
		}

		void InitText(string text = null)
		{
			if (buttonText != null)
			{
				SetButtonText(text);
				return;
			}
			var textGO = new GameObject(TEXT, typeof(TextMeshProUGUI));
			buttonText = Instantiate(textGO, parent: transform, worldPositionStays: true).GetComponent<TMP_Text>();
			SetButtonText(text);
		}

		void InitImage()
		{
			if (buttonImage != null)
				return;
			var imageGO = new GameObject(IMAGE, typeof(Image));
			buttonImage = Instantiate(imageGO, transform, true).GetComponent<Image>();
		}
		
		void ResetImageAndTextComponents()
		{
			ResetImageComponent();
			ResetTextComponent();
		}
		
		void ResetImageComponent()
		{
			if (buttonImage == null)
				return;
			buttonImage.raycastTarget = false;
		}
		
		void ResetTextComponent()
		{
			if (buttonText == null)
				return;
			buttonText.text = TEXT_MISSING;
			buttonText.alignment = TextAlignmentOptions.Center;
		}
		
		
#if UNITY_EDITOR
		void OnValidate()
		{
			if (buttonImage != null)
				buttonImage.ActivateGameObject(display is ButtonObjectDisplayType.Image);
			
			if (buttonText != null && buttonText.ActivateGameObject(display is ButtonObjectDisplayType.Text))
				buttonText.text = textOnButton;
		}

		void Reset()
		{
			if (gameObject.IsEditingPrefab_EditorOnly())
				return;
			
			int children = transform.childCount;
			
			Image imageComponent = GetComponentInChildren<Image>();
			TMP_Text textComponent = GetComponentInChildren<TMP_Text>();

			if (buttonImage == null && imageComponent != null)
				buttonImage = imageComponent;
			if (buttonText == null && textComponent != null)
				buttonText = textComponent;
			
			bool foundImage = buttonImage != null;
			bool foundText = buttonText != null;

			if (children is TWO && foundImage && foundText)
				return;

			buttonText = null;
			buttonImage = null;
				
			for (int i = children - ONE; i >= ZERO; i--)
			{
				var childGameObject = transform.GetChild(i).gameObject;
				DestroyImmediate(childGameObject);
			}
			
			InitImage();
			InitText();
			ResetImageAndTextComponents();
		}
#endif
	}
	
	internal enum ButtonObjectDisplayType
	{
		Text,
		Image
	}
}