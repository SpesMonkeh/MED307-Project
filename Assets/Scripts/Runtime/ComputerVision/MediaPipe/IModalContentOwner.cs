// Copyright © Christian Holm Christensen
// 24/10/2023

using UnityEngine;

namespace P307.Runtime.ComputerVision.MediaPipe
{
	public interface IModalContentOwner<out TComponent> where TComponent : Component
	{
		public int OwnerInstanceId => Owner.GetInstanceID();
		public Object Content { get; }
		public TComponent Owner { get; } 
	}
}