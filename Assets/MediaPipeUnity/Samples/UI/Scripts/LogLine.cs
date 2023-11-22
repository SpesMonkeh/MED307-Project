// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.
// Modified by SpesMonkeh, 2023. 

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static P307.Shared.Const307;

namespace Mediapipe.Unity
{
#pragma warning disable IDE0065
  using Color = UnityEngine.Color;
#pragma warning restore IDE0065

  public class LogLine : MonoBehaviour
  {
    [SerializeField] Text _utcTimeArea;
    [SerializeField] Text _tagArea;
    [SerializeField] Text _messageArea;

    // TODO: [SerializeField] TMP_Text utCTimeArea;
    // TODO: [SerializeField] TMP_Text tagArea;
    // TODO: [SerializeField] TMP_Text messageArea;

    static readonly Color purple = new(.6f, 0f, .8f);
    
    public string UtcTime => GetTextFrom(_utcTimeArea);
    public string LineTag => GetTextFrom(_tagArea);
    public string Message => GetTextFrom(_messageArea);
    
    public void SetLog(MemorizedLogger.LogStruct logStruct)
    {
      _utcTimeArea.text = FormatUtcTime(logStruct.utcTime);
      _tagArea.text = FormatTag(logStruct.tag);
      _messageArea.text = FormatMessage(logStruct.message);
      _messageArea.color = GetMessageColor(logStruct.logLevel);
    }

    static string FormatUtcTime(DateTime utcTime)
      => utcTime.ToString("MMM dd hh:mm:ss.fff");
    static string FormatTag(string lineTag)
      => string.IsNullOrEmpty(lineTag) is false ? $"{lineTag}:" : string.Empty;
    static string FormatMessage(object message)
      => message is not null ? message.ToString() : NULL_STRING;
    static Color GetMessageColor(Logger.LogLevel logLevel) => logLevel switch
    {
       Logger.LogLevel.Fatal => purple,
       Logger.LogLevel.Error => Color.red,
       Logger.LogLevel.Warn => Color.yellow,
       Logger.LogLevel.Info => Color.green,
       Logger.LogLevel.Debug => Color.gray,
       Logger.LogLevel.Verbose => Color.white, 
       _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
    };

    static string GetTextFrom(Text component) => component != null  // TODO: erstat med (TMP_TEXT component)
      ? component.text
      : string.Empty;
  }
}
