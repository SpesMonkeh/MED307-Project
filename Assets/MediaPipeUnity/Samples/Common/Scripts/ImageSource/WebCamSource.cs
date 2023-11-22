// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Mediapipe.Unity
{
  public class WebCamSource : ImageSource
  {
    [Tooltip("For the default resolution, the one whose width is closest to this value will be chosen")]
    [SerializeField] int _preferableDefaultWidth = 1280;

    const string _TAG = nameof(WebCamSource);

    [SerializeField] ResolutionStruct[] _defaultAvailableResolutions = {
      new(176, 144, 30),
      new(320, 240, 30),
      new(424, 240, 30),
      new(640, 360, 30),
      new(640, 480, 30),
      new(848, 480, 30),
      new(960, 540, 30),
      new(1280, 720, 30),
      new(1600, 896, 30),
      new(1920, 1080, 30),
    };

    static readonly object _PermissionLock = new();
    static bool _IsPermitted;

    WebCamTexture _webCamTexture;
    WebCamTexture webCamTexture
    {
      get => _webCamTexture;
      set
      {
        if (_webCamTexture != null)
        {
          _webCamTexture.Stop();
        }
        _webCamTexture = value;
      }
    }

    public override int textureWidth => !isPrepared ? 0 : webCamTexture.width;
    public override int textureHeight => !isPrepared ? 0 : webCamTexture.height;

    public override bool isVerticallyFlipped => isPrepared && webCamTexture.videoVerticallyMirrored;
    public override bool isFrontFacing => isPrepared && (webCamDevice is { } valueOfWebCamDevice) && valueOfWebCamDevice.isFrontFacing;
    public override RotationAngle rotation => !isPrepared ? RotationAngle.Rotation0 : (RotationAngle)webCamTexture.videoRotationAngle;

    WebCamDevice? _webCamDevice;
    WebCamDevice? webCamDevice
    {
      get => _webCamDevice;
      set
      {
        if (_webCamDevice is { } valueOfWebCamDevice)
        {
          if (value is { } valueOfValue && valueOfValue.name == valueOfWebCamDevice.name)
          {
            // not changed
            return;
          }
        }
        else if (value == null)
        {
          // not changed
          return;
        }
        _webCamDevice = value;
        resolution = GetDefaultResolution();
      }
    }
    public override string sourceName => (webCamDevice is { } valueOfWebCamDevice) ? valueOfWebCamDevice.name : null;

    private WebCamDevice[] _availableSources;
    private WebCamDevice[] availableSources
    {
      get
      {
        if (_availableSources == null)
        {
          _availableSources = WebCamTexture.devices;
        }

        return _availableSources;
      }
      set => _availableSources = value;
    }

    public override string[] sourceCandidateNames => availableSources?.Select(device => device.name).ToArray();

#pragma warning disable IDE0025
    public override ResolutionStruct[] availableResolutions
    {
      get
      {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (webCamDevice is WebCamDevice valueOfWebCamDevice) {
          return valueOfWebCamDevice.availableResolutions.Select(resolution => new ResolutionStruct(resolution)).ToArray();
        }
#endif
        return webCamDevice == null ? null : _defaultAvailableResolutions;
      }
    }
#pragma warning restore IDE0025

    public override bool isPrepared => webCamTexture != null;
    public override bool isPlaying => webCamTexture != null && webCamTexture.isPlaying;
    bool _isInitialized;

    IEnumerator Start()
    {
      yield return GetPermission();

      if (!_IsPermitted)
      {
        _isInitialized = true;
        yield break;
      }

      availableSources = WebCamTexture.devices;

      if (availableSources != null && availableSources.Length > 0)
      {
        webCamDevice = availableSources[0];
      }

      _isInitialized = true;
    }

    IEnumerator GetPermission()
    {
      lock (_PermissionLock)
      {
        if (_IsPermitted)
        {
          yield break;
        }

#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
          Permission.RequestUserPermission(Permission.Camera);
          yield return new WaitForSeconds(0.1f);
        }
#elif UNITY_IOS
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam)) {
          yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }
#endif

#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
          Logger.LogWarning(_TAG, "Not permitted to use Camera");
          yield break;
        }
#elif UNITY_IOS
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam)) {
          Logger.LogWarning(_TAG, "Not permitted to use WebCam");
          yield break;
        }
#endif
        _IsPermitted = true;

        yield return new WaitForEndOfFrame();
      }
    }

    public override void SelectSource(int sourceId)
    {
      if (sourceId < 0 || sourceId >= availableSources.Length)
      {
        throw new ArgumentException($"Invalid source ID: {sourceId}");
      }

      webCamDevice = availableSources[sourceId];
    }

    public override IEnumerator Play()
    {
      yield return new WaitUntil(() => _isInitialized);
      if (!_IsPermitted)
      {
        throw new InvalidOperationException("Not permitted to access cameras");
      }

      InitializeWebCamTexture();
      webCamTexture.Play();
      yield return WaitForWebCamTexture();
    }

    public override IEnumerator Resume()
    {
      if (!isPrepared)
      {
        throw new InvalidOperationException("WebCamTexture is not prepared yet");
      }
      if (!webCamTexture.isPlaying)
      {
        webCamTexture.Play();
      }
      yield return WaitForWebCamTexture();
    }

    public override void Pause()
    {
      if (isPlaying)
      {
        webCamTexture.Pause();
      }
    }

    public override void Stop()
    {
      if (webCamTexture != null)
      {
        webCamTexture.Stop();
      }
      webCamTexture = null;
    }

    public override Texture GetCurrentTexture()
    {
      return webCamTexture;
    }

    ResolutionStruct GetDefaultResolution()
    {
      ResolutionStruct[] resolutions = availableResolutions;
      return resolutions == null || resolutions.Length == 0 ? new ResolutionStruct() : resolutions.OrderBy(res => res, new ResolutionStructComparer(_preferableDefaultWidth)).First();
    }

    void InitializeWebCamTexture()
    {
      Stop();
      if (webCamDevice is { } valueOfWebCamDevice)
      {
        webCamTexture = new WebCamTexture(valueOfWebCamDevice.name, resolution.width, resolution.height, (int)resolution.frameRate.value);
        return;
      }
      throw new InvalidOperationException("Cannot initialize WebCamTexture because WebCamDevice is not selected");
    }

    IEnumerator WaitForWebCamTexture()
    {
      const int timeout_frame = 2000;
      int count = 0;
      Logger.LogVerbose("Waiting for WebCamTexture to start");
      yield return new WaitUntil(() => count++ > timeout_frame || webCamTexture.width > 16);

      if (webCamTexture.width <= 16)
      {
        throw new TimeoutException("Failed to start WebCam");
      }
    }

    class ResolutionStructComparer : IComparer<ResolutionStruct>
    {
      readonly int _preferableDefaultWidth;

      public ResolutionStructComparer(int preferableDefaultWidth)
      {
        _preferableDefaultWidth = preferableDefaultWidth;
      }

      public int Compare(ResolutionStruct a, ResolutionStruct b)
      {
        var aDiff = Mathf.Abs(a.width - _preferableDefaultWidth);
        var bDiff = Mathf.Abs(b.width - _preferableDefaultWidth);
        if (aDiff != bDiff)
        {
          return aDiff - bDiff;
        }
        if (a.height != b.height)
        {
          // prefer smaller height
          return a.height - b.height;
        }
        // prefer smaller frame rate
        return (int)(a.frameRate.value - b.frameRate.value);
      }
    }
  }
}
