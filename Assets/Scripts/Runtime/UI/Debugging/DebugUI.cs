// Copyright © Christian Holm Christensen
// 26/10/2023

using P307.Runtime.Core.Inputs;
using P307.Runtime.Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace P307.Runtime.UI.Debugging
{
	[DisallowMultipleComponent]
	public sealed class DebugUI : MonoBehaviour
	{
		[SerializeField] Button pointerModeButton;
		[SerializeField] Button movementModeButton;
		[SerializeField] Button signLanguageModeButton;

		void OnEnable()
		{
			InputHandler.inputModeChanged += OnInputModeChanged;
			InputHandler.inputDeviceChanged += OnInputDeviceChanged;
		}

		void OnDisable()
		{
			InputHandler.inputModeChanged -= OnInputModeChanged;
			InputHandler.inputDeviceChanged -= OnInputDeviceChanged;
		}

		void OnInputModeChanged(InputModeSO mode)
		{
			pointerModeButton.image.color = mode is PointerInputModeSO
				? Color.blue
				: Color.black;
			pointerModeButton.GetComponentInChildren<TextMeshProUGUI>().color = mode is PointerInputModeSO
				? Color.white
				: Color.black;

			movementModeButton.image.color = mode is MovementInputModeSO
				? Color.blue
				: Color.black;
			movementModeButton.GetComponentInChildren<TextMeshProUGUI>().color = mode is MovementInputModeSO
				? Color.white
				: Color.black;

			signLanguageModeButton.image.color = mode is SignLanguageInputModeSO
				? Color.blue
				: Color.black;
			signLanguageModeButton.GetComponentInChildren<TextMeshProUGUI>().color = mode is SignLanguageInputModeSO
				? Color.white
				: Color.black;
		}

		void OnInputDeviceChanged(InputDeviceSO device)
		{
			Debug.LogWarning($"{nameof(OnInputDeviceChanged)} er ikke implementeret endnu."); // TODO
		}
	}
}