// Copyright © Christian Holm Christensen
// 03/10/2023

using System.Collections.Generic;
using Mediapipe.Unity;
using P307.Runtime.Hands;

namespace P307.Shared
{
	public static class Shared307
	{
		public static Dictionary<PointAnnotation, HandLandmark> pointsAndLandmarks = new();
	}
}