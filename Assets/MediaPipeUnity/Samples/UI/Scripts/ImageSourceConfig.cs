// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.
// Modified by SpesMonkeh, 2023. 

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mediapipe.Unity.UI
{
  public class ImageSourceConfig : ModalContents
  {
    const string MEDIAPIPE_SOLUTION = "MEDIAPIPE - Solution";
    
    const string SOURCE_TYPE_PATH = "Scroll View/Viewport/Contents/SourceType/Dropdown";
    const string SOURCE_PATH = "Scroll View/Viewport/Contents/Source/Dropdown";
    const string RESOLUTION_PATH = "Scroll View/Viewport/Contents/Resolution/Dropdown";
    const string IS_HORIZONTALLY_FLIPPED_PATH = "Scroll View/Viewport/Contents/IsHorizontallyFlipped/Toggle";

    bool isChanged;
    Toggle isHorizontallyFlippedInput;
    Dropdown sourceTypeInput;
    Dropdown sourceInput;
    Dropdown resolutionInput;
    Solution solution;

    void Start()
    {
      solution = GameObject.Find(MEDIAPIPE_SOLUTION).GetComponent<Solution>();
      InitializeContents();
    }

    public override void Exit() => GetModal().CloseAndResume(isChanged);

    void InitializeContents()
    {
      InitializeSourceType();
      InitializeSource();
      InitializeResolution();
      InitializeIsHorizontallyFlipped();
    }

    void InitializeSourceType()
    {
      sourceTypeInput = gameObject.transform.Find(SOURCE_TYPE_PATH).gameObject.GetComponent<Dropdown>();
      sourceTypeInput.ClearOptions();
      sourceTypeInput.onValueChanged.RemoveAllListeners();

      var options = Enum.GetNames(typeof(ImageSourceType)).Where(x => x != ImageSourceType.Unknown.ToString()).ToList();
      sourceTypeInput.AddOptions(options);

      var currentSourceType = ImageSourceProvider.CurrentSourceType;
      var defaultValue = options.FindIndex(option => option == currentSourceType.ToString());

      if (defaultValue >= 0)
      {
        sourceTypeInput.value = defaultValue;
      }

      sourceTypeInput.onValueChanged.AddListener(delegate
      {
        ImageSourceProvider.ImageSource = solution.bootstrap.GetImageSource((ImageSourceType)sourceTypeInput.value);
        isChanged = true;
        InitializeContents();
      });
    }

    void InitializeSource()
    {
      sourceInput = gameObject.transform.Find(SOURCE_PATH).gameObject.GetComponent<Dropdown>();
      sourceInput.ClearOptions();
      sourceInput.onValueChanged.RemoveAllListeners();

      var imageSource = ImageSourceProvider.ImageSource;
      var sourceNames = imageSource.sourceCandidateNames;

      if (sourceNames == null)
      {
        sourceInput.enabled = false;
        return;
      }

      var options = new List<string>(sourceNames);
      sourceInput.AddOptions(options);

      var currentSourceName = imageSource.sourceName;
      var defaultValue = options.FindIndex(option => option == currentSourceName);

      if (defaultValue >= 0)
      {
        sourceInput.value = defaultValue;
      }

      sourceInput.onValueChanged.AddListener(delegate
      {
        imageSource.SelectSource(sourceInput.value);
        isChanged = true;
        InitializeResolution();
      });
    }

    void InitializeResolution()
    {
      resolutionInput = gameObject.transform.Find(RESOLUTION_PATH).gameObject.GetComponent<Dropdown>();
      resolutionInput.ClearOptions();
      resolutionInput.onValueChanged.RemoveAllListeners();

      var imageSource = ImageSourceProvider.ImageSource;
      var resolutions = imageSource.availableResolutions;

      if (resolutions == null)
      {
        resolutionInput.enabled = false;
        return;
      }

      var options = resolutions.Select(resolution => resolution.ToString()).ToList();
      resolutionInput.AddOptions(options);

      var currentResolutionStr = imageSource.resolution.ToString();
      var defaultValue = options.FindIndex(option => option == currentResolutionStr);

      if (defaultValue >= 0)
      {
        resolutionInput.value = defaultValue;
      }

      resolutionInput.onValueChanged.AddListener(delegate
      {
        imageSource.SelectResolution(resolutionInput.value);
        isChanged = true;
      });
    }

    void InitializeIsHorizontallyFlipped()
    {
      isHorizontallyFlippedInput = gameObject.transform.Find(IS_HORIZONTALLY_FLIPPED_PATH).gameObject.GetComponent<Toggle>();

      var imageSource = ImageSourceProvider.ImageSource;
      isHorizontallyFlippedInput.isOn = imageSource.isHorizontallyFlipped;
      isHorizontallyFlippedInput.onValueChanged.AddListener(delegate
      {
        imageSource.isHorizontallyFlipped = isHorizontallyFlippedInput.isOn;
        isChanged = true;
      });
    }
  }
}
