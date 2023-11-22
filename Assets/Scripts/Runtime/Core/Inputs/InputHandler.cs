// Copyright © Christian Holm Christensen
// 26/10/2023

using System;
using System.Collections.Generic;
using System.Linq;
using P307.Runtime.Inputs;
using UnityEngine;

namespace P307.Runtime.Core.Inputs
{
	public sealed class InputHandler : MonoBehaviour, IP307CoreComponent
	{
		public static Func<InputModeSO> getCurrentInputMode = () => null;
		public static Func<InputDeviceSO> getCurrentInputDevice = () => null;
		public static Action<InputModeSO> inputModeChanged = delegate {  };
		public static Action<InputDeviceSO> inputDeviceChanged = delegate {  };

		[SerializeField] InputModeSO currentInputMode;
		[SerializeField] InputDeviceSO currentInputDevice;

		public InputModeSO[] modes = new InputModeSO[3];
		public InputDeviceSO[] devices = new InputDeviceSO[2];
		
		public bool Initialize()
		{
			CreateInputObjects(ref modes);
			CreateInputObjects(ref devices);
			currentInputMode = modes[0];
			currentInputDevice = devices[0];
			return true;
		}

		void OnEnable()
		{
			getCurrentInputMode = OnGetCurrentInputMode;
			getCurrentInputDevice = OnGetCurrentInputDevice;
		}

		void OnDisable()
		{
			getCurrentInputMode -= OnGetCurrentInputMode;
			getCurrentInputDevice -= OnGetCurrentInputDevice;
		}
		
		void Start()
		{
			inputModeChanged?.Invoke(currentInputMode);
			inputDeviceChanged?.Invoke(currentInputDevice);
		}

		void CreateInputObjects<T>(ref T[] array) where T : ScriptableObject
		{
			IEnumerable<Type> types = GetType().
				Assembly.GetTypes()
				.Where(type =>
					type.IsClass
					&& type.BaseType == typeof(T));
			int i = 0;
			foreach (Type type in types)
			{
				array[i++] = ScriptableObject.CreateInstance(type) as T;
			}
		}

		InputModeSO OnGetCurrentInputMode()
		{
			if (currentInputMode != null)
				return currentInputMode;
			currentInputMode = modes[0];
			inputModeChanged?.Invoke(currentInputMode);
			return currentInputMode;
		}

		InputDeviceSO OnGetCurrentInputDevice()
		{
			if (currentInputDevice != null)
				return currentInputDevice;
			currentInputDevice = devices[0];
			inputDeviceChanged?.Invoke(currentInputDevice);
			return currentInputDevice;
		}
	}
}