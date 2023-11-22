// Copyright © Christian Holm Christensen
// 13/09/2023

using UnityEngine;
using UnityEngine.UIElements;

namespace P307.Shared
{
	public static class Utils307
	{
		public static readonly Color P307LabelTextColor = new(.6f, 0.7f, 0.7f);
		public static readonly Color P307LabelBackgroundColor = new(0.1f, 0.1f, 0.36f);

		const string P307_COMPONENT = "P307 Component";
		
		public static VisualElement P307Label()
		{
			Label label = new Label(P307_COMPONENT)
			{
				style =
				{
					fontSize = 16,
					unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold),
					unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleLeft),
					color = new StyleColor(P307LabelTextColor),
					backgroundColor = P307LabelBackgroundColor,
					flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
					borderTopLeftRadius = 5,
					borderTopRightRadius = 5,
					borderBottomLeftRadius = 5,
					borderBottomRightRadius = 5,
				}
			};
			return label;
		}
	}
}