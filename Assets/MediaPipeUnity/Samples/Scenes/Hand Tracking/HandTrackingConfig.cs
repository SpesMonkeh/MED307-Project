// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.
// Modified by SpesMonkeh, 2023. 

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe.Unity.UI;
using static P307.Shared.Const307;

namespace Mediapipe.Unity.HandTracking.UI
{
  public class HandTrackingConfig : ModalContents
  {
    const string MODEL_COMPLEXITY_PATH = "Scroll View/Viewport/Contents/Model Complexity/Dropdown";
    const string MAX_NUM_HANDS_PATH = "Scroll View/Viewport/Contents/Max Num Hands/InputField";
    const string MIN_DETECTION_CONFIDENCE_PATH = "Scroll View/Viewport/Contents/Min Detection Confidence/InputField";
    const string MIN_TRACKING_CONFIDENCE_PATH = "Scroll View/Viewport/Contents/Min Tracking Confidence/InputField";
    const string RUNNING_MODE_PATH = "Scroll View/Viewport/Contents/Running Mode/Dropdown";
    const string TIMEOUT_MILLISEC_PATH = "Scroll View/Viewport/Contents/Timeout Millisec/InputField";

    bool isChanged;
    Dropdown runningModeInput;
    Dropdown modelComplexityInput;
    InputField maxNumHandsInput;
    InputField minTrackingConfidenceInput;
    InputField minDetectionConfidenceInput;
    InputField timeoutMillisecInput;
    HandTrackingSolution solution;


    void Start()
    {
      solution = GameObject.Find(MEDIAPIPE_SOLUTION).GetComponent<HandTrackingSolution>();
      InitializeContents();
    }

    public override void Exit()
    {
      GetModal().CloseAndResume(isChanged);
    }

    public void SwitchModelComplexity()
    {
      solution.modelComplexity = (HandTrackingGraph.ModelComplexity)modelComplexityInput.value;
      isChanged = true;
    }

    public void UpdateMaxNumHands()
    {
      if (int.TryParse(maxNumHandsInput.text, out var value))
      {
        solution.maxNumHands = Mathf.Max(0, value);
        isChanged = true;
      }
    }

    public void SetMinDetectionConfidence()
    {
      if (float.TryParse(minDetectionConfidenceInput.text, out var value))
      {
        solution.minDetectionConfidence = value;
        isChanged = true;
      }
    }

    public void SetMinTrackingConfidence()
    {
      if (float.TryParse(minTrackingConfidenceInput.text, out var value))
      {
        solution.minTrackingConfidence = value;
        isChanged = true;
      }
    }

    public void SwitchRunningMode()
    {
      solution.runningMode = (RunningMode)runningModeInput.value;
      isChanged = true;
    }

    public void SetTimeoutMillisec()
    {
      if (int.TryParse(timeoutMillisecInput.text, out var value))
      {
        solution.timeoutMillisec = value;
        isChanged = true;
      }
    }

    void InitializeContents()
    {
      InitializeModelComplexity();
      InitializeMaxNumHands();
      InitializeMinDetectionConfidence();
      InitializeMinTrackingConfidence();
      InitializeRunningMode();
      InitializeTimeoutMillisec();
    }

    private void InitializeModelComplexity()
    {
      modelComplexityInput = gameObject.transform.Find(MODEL_COMPLEXITY_PATH).gameObject.GetComponent<Dropdown>();
      modelComplexityInput.ClearOptions();

      var options = new List<string>(Enum.GetNames(typeof(HandTrackingGraph.ModelComplexity)));
      modelComplexityInput.AddOptions(options);

      var currentModelComplexity = solution.modelComplexity;
      var defaultValue = options.FindIndex(option => option == currentModelComplexity.ToString());

      if (defaultValue >= 0)
      {
        modelComplexityInput.value = defaultValue;
      }

      modelComplexityInput.onValueChanged.AddListener(delegate { SwitchModelComplexity(); });
    }

    private void InitializeMaxNumHands()
    {
      maxNumHandsInput = gameObject.transform.Find(MAX_NUM_HANDS_PATH).gameObject.GetComponent<InputField>();
      maxNumHandsInput.text = solution.maxNumHands.ToString();
      maxNumHandsInput.onEndEdit.AddListener(delegate { UpdateMaxNumHands(); });
    }

    private void InitializeMinDetectionConfidence()
    {
      minDetectionConfidenceInput = gameObject.transform.Find(MIN_DETECTION_CONFIDENCE_PATH).gameObject.GetComponent<InputField>();
      minDetectionConfidenceInput.text = solution.minDetectionConfidence.ToString();
      minDetectionConfidenceInput.onValueChanged.AddListener(delegate { SetMinDetectionConfidence(); });
    }

    private void InitializeMinTrackingConfidence()
    {
      minTrackingConfidenceInput = gameObject.transform.Find(MIN_TRACKING_CONFIDENCE_PATH).gameObject.GetComponent<InputField>();
      minTrackingConfidenceInput.text = solution.minTrackingConfidence.ToString();
      minTrackingConfidenceInput.onValueChanged.AddListener(delegate { SetMinTrackingConfidence(); });
    }

    private void InitializeRunningMode()
    {
      runningModeInput = gameObject.transform.Find(RUNNING_MODE_PATH).gameObject.GetComponent<Dropdown>();
      runningModeInput.ClearOptions();

      var options = new List<string>(Enum.GetNames(typeof(RunningMode)));
      runningModeInput.AddOptions(options);

      var currentRunningMode = solution.runningMode;
      var defaultValue = options.FindIndex(option => option == currentRunningMode.ToString());

      if (defaultValue >= 0)
      {
        runningModeInput.value = defaultValue;
      }

      runningModeInput.onValueChanged.AddListener(delegate { SwitchRunningMode(); });
    }

    private void InitializeTimeoutMillisec()
    {
      timeoutMillisecInput = gameObject.transform.Find(TIMEOUT_MILLISEC_PATH).gameObject.GetComponent<InputField>();
      timeoutMillisecInput.text = solution.timeoutMillisec.ToString();
      timeoutMillisecInput.onValueChanged.AddListener(delegate { SetTimeoutMillisec(); });
    }
  }
}
