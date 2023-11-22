// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.
// Modified by SpesMonkeh, 2023. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static P307.Shared.Const307;

namespace Mediapipe.Unity.UI
{
	public class GUIConsole : MonoBehaviour
	{
		[SerializeField] GameObject _logLinePrefab;
		[SerializeField] int _maxLines = 200;
		
		int lines = 0;
		Transform contentRoot;
		MemorizedLogger logger;
		Queue<MemorizedLogger.LogStruct> scheduledLogs;
		WaitForEndOfFrame waitForEndOfFrame;

		const string CONTENT_PATH = "Viewport/Content";
        
		ScrollRect ScrollRect => gameObject.GetComponent<ScrollRect>();

		GameObject FirstLineGameObject => contentRoot != null 
			? contentRoot.GetChild(ZERO).gameObject
			: null;

		void Start()
		{
			waitForEndOfFrame = new WaitForEndOfFrame();
			scheduledLogs = new Queue<MemorizedLogger.LogStruct>();
			InitializeView();
		}

		void LateUpdate()
		{
			RenderScheduledLogs();
		}

		void OnDestroy()
		{
			logger.OnLogOutput -= ScheduleLog;
		}

		void InitializeView()
		{
			contentRoot = gameObject.transform.Find(CONTENT_PATH).gameObject.transform;
			if (Logger.InternalLogger is not MemorizedLogger memorizedLogger)
				return;
			
			logger = memorizedLogger;
			lock (((ICollection)logger.histories).SyncRoot)
			{
				foreach (var log in logger.histories)
					AppendLog(log);
				logger.OnLogOutput += ScheduleLog;
			}

			_ = StartCoroutine(ScrollToBottom());
		}

		protected void ScheduleLog(MemorizedLogger.LogStruct log)
		{
			lock (((ICollection)scheduledLogs).SyncRoot)
			{
				scheduledLogs.Enqueue(log);
			}
		}

		void RenderScheduledLogs()
		{
			lock (((ICollection)scheduledLogs).SyncRoot)
			{
				while (scheduledLogs.Count > ZERO)
					AppendLog(scheduledLogs.Dequeue());
			}
			if (ScrollRect.verticalNormalizedPosition < ONE_MILLIONTH)
				_ = StartCoroutine(ScrollToBottom());
		}

		void AppendLog(MemorizedLogger.LogStruct logStruct)
		{
			MakeLine(logStruct);
			if (++lines <= _maxLines)
				return;
			RemoveLine();
		}

		void MakeLine(MemorizedLogger.LogStruct logStruct)
		{
			const int max_game_obj_name_characters = 30;
			GameObject logLineObj = Instantiate(_logLinePrefab, contentRoot);
			LogLine newLine = logLineObj.GetComponent<LogLine>();
			newLine.SetLog(logStruct);
			string lineTag = newLine.LineTag;
			int lastCharIndex = Mathf.Max(max_game_obj_name_characters - lineTag.Length, ZERO);
			logLineObj.name = $"{lineTag} :: {newLine.Message[..lastCharIndex]}";
		}

		void RemoveLine()
		{
			if (lines is ZERO || FirstLineGameObject == null)
				return;
			Destroy(FirstLineGameObject);
			lines--;
		}

		IEnumerator ScrollToBottom()
		{
			yield return waitForEndOfFrame;
			Canvas.ForceUpdateCanvases();
			ScrollRect.verticalNormalizedPosition = ZERO;
		}
	}
}