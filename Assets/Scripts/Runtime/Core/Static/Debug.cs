// Copyright © Christian Holm Christensen
// 02/10/2023

using System.Text;
using P307.Shared;
using UnityEngine;
using static P307.Shared.Const307;
using ColorUtility = UnityEngine.ColorUtility;

namespace P307.Runtime
{
	public class Debug
	{
		public static string Log(object message, string title = P307_TAG)
		{
			const string title_format = "<color=#{0}>{1}</color> ::  {2}";
			
			string rgbaHex = ColorUtility.ToHtmlStringRGBA(Color307.p307Primary);
			StringBuilder builder = new StringBuilder();
			
			builder.AppendFormat(title_format, rgbaHex, title, message);
			UnityEngine.Debug.unityLogger.Log(LogType.Log, builder);
			return builder.ToString();
		}
		
		public static string Log(object message, Object context, string title = P307_TAG)
		{
			const string title_format = FORMATTED_TITLE + "<i>{2}</i> :: {3}";
			
			string rgbaHex = ColorUtility.ToHtmlStringRGBA(Color307.p307Primary);
			StringBuilder builder = new StringBuilder();
			
			builder.AppendFormat(title_format, rgbaHex, title, context, message);
			UnityEngine.Debug.unityLogger.Log(LogType.Log, builder, context);
			return builder.ToString();
		}

		const string FORMATTED_TITLE = "<color=#{0}>{1}</color> :: ";
		const string FORMATTED_CONDITION_INFO = "<color=#{2}>Condition was <b>[{3}]</b></color> :: ";
		const string FORMATTED_CONTEXT_INFO = "Context: <i>{4}</i> :: ";
		const string FORMATTED_LOG_MESSAGE = "{5}";
		const string CONDITIONAL_LOG_TEXT_FORMAT = FORMATTED_TITLE + FORMATTED_CONDITION_INFO + FORMATTED_CONTEXT_INFO + FORMATTED_LOG_MESSAGE;
		public static string Log(bool condition, object message, Object context, bool logFalseCondition = false, string title = P307_TAG)
		{
			/*
			 * 0. titleColorHex 2. boolColorHex			4. context.ToString()
			 * 1. title			3. condition.ToString() 5. message.ToString()
			 */

			if (condition is false && logFalseCondition is false)
				return string.Empty;

			string titleColorHex = ColorUtility.ToHtmlStringRGBA(Color307.p307Primary);
			string boolColorHex = ColorUtility.ToHtmlStringRGBA(condition ? Color307.trueBoolColor : Color307.falseBoolColor);
			StringBuilder builder = new StringBuilder();
			
			builder.AppendFormat(CONDITIONAL_LOG_TEXT_FORMAT, titleColorHex, title, boolColorHex, condition, context, message);
			UnityEngine.Debug.unityLogger.Log(LogType.Log, builder, context);
			return builder.ToString();
		}
		
		public static string LogWarning(object message, string title = P307_TAG)
		{
			const string title_format = "[ <color=#{0}><b>!</b></color> ] <color=#{1}>{2}</color> ::  {3}";

			string rgbaHex = ColorUtility.ToHtmlStringRGBA(Color307.p307Primary);
			string warnHex = ColorUtility.ToHtmlStringRGBA(Color307.warning);
			StringBuilder builder = new StringBuilder();
			
			builder.AppendFormat(title_format, warnHex, rgbaHex, title, message);
			UnityEngine.Debug.unityLogger.Log(LogType.Warning, builder);
			return builder.ToString();
		}
		
		public static string LogWarning(object message, Object context, string title = P307_TAG) // TODO Ryd op. Ligner method ovenover.
		{
			const string title_format = "[ <color=#{0}><b>!</b></color> ] <color=#{1}>{2}</color> :: <i>{3}</i> :: {4}";

			string rgbaHex = ColorUtility.ToHtmlStringRGBA(Color307.p307Primary);
			string warnHex = ColorUtility.ToHtmlStringRGBA(Color307.warning);
			StringBuilder builder = new StringBuilder();
			
			builder.AppendFormat(title_format, warnHex, rgbaHex, title, context, message);
			UnityEngine.Debug.unityLogger.Log(LogType.Warning, builder, context);
			return builder.ToString();
		}
		
		public static string LogError(object message, string title = P307_TAG)
		{
			const string title_format = "[ <color=#{0}><b>!!</b></color> ] <color=#{1}>{2}</color> :: {3}";

			string rgbaHex = ColorUtility.ToHtmlStringRGBA(Color307.p307Primary);
			string errorHex = ColorUtility.ToHtmlStringRGBA(Color307.error);
			StringBuilder builder = new StringBuilder();
			
			builder.AppendFormat(title_format, errorHex, rgbaHex, title, message);
			UnityEngine.Debug.unityLogger.Log(LogType.Error, builder);
			return builder.ToString();
		}
		
		public static string LogError(object message, Object context, string title = P307_TAG)
		{
			const string title_format = "[ <color=#{0}><b>!!</b></color> ] <color=#{1}>{2}</color> :: <i>{3}</i> :: {4}";

			string rgbaHex = ColorUtility.ToHtmlStringRGBA(Color307.p307Primary);
			string errorHex = ColorUtility.ToHtmlStringRGBA(Color307.error);
			StringBuilder builder = new StringBuilder();
			
			builder.AppendFormat(title_format, errorHex, rgbaHex, title, context, message);
			UnityEngine.Debug.unityLogger.Log(LogType.Error, builder, context);
			return builder.ToString();
		}
	}
}