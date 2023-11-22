// Copyright © Christian Holm Christensen
// 02/10/2023

using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace P307.Shared
{
	public static class Extensions307
	{
		// INTs
		/// <summary> Hurtig funktion til at tage antallet af værdier lagret i en <see cref="IList"/>, og returnerer antallet af indicer. </summary>
		/// <returns><see cref="int"/> - Indicer i den givne <c>IList</c>.</returns>
		public static int Indices(this IList value) => SubtractOneFrom(value.Count);
		
		/// <summary> Hurtig funktion til at tage antallet af værdier lagret i en <see cref="IDictionary"/>, og returnerer antallet af indicer. </summary>
		/// <returns><see cref="int"/> - Indicer i den givne <c>IDictionary</c>.</returns>
		public static int Indices(this IDictionary value) => SubtractOneFrom(value.Count);
		
		/// <summary> Hurtig funktion til at tage antallet af værdier lagret i en <see cref="Array"/>, og returnerer antallet af indicer. </summary>
		/// <returns><see cref="int"/> - Indicer i den givne <c>Array</c>.</returns>
		public static int Indices(this Array value) => SubtractOneFrom(value.Length);

		
		static int SubtractOneFrom(int value) => value - Const307.ONE;

		
		
		// GAME OBJECTs
		/// <summary>
		/// Aktiverer eller deaktiverer dét <see cref="GameObject"/>, som dette <see cref="Component"/> er påsat.
		/// <br/><br/>
		/// <font color ="#FFA500"><b>OBS:</b> Denne funktion tager <b>IKKE</b> højde for, om andre komponenter benytter, er afhængige af, eller
		/// samtidigt manipulerer med dette komponents GameObject!</font>
		/// </summary>
		/// <param name="value">Komponentet, hvis GameObject skal manipuleres.</param>
		/// <param name="setActive">Skal komponentets GameObject aktiveres eller deaktiveres?</param>
		/// <returns><see cref="bool"/> - <c>true</c>, hvis komponentets GameObject er aktiveret, ellers; <c>false</c>.</returns>
		public static bool ActivateGameObject(this Component value, bool setActive)
		{
			var gObject = value.gameObject;
			gObject.SetActive(setActive);
			return gObject.activeSelf;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">Dét GameObject, som skal skal tjekkes.</param>
		/// <returns><c>false</c> - hvis koden ikke køres i editor eller hvis koden køres i editor, men i prefab mode. Ellers <c>true</c>.</returns>
		public static bool IsEditingPrefab_EditorOnly(this GameObject value)
		{
			bool result = false;
#if UNITY_EDITOR
			result = PrefabStageUtility.GetCurrentPrefabStage().IsPartOfPrefabContents(value);
#endif
			return result;
		} 
		
	}
}