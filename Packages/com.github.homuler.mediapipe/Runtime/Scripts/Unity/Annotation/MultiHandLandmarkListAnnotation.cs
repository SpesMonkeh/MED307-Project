// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Mediapipe.Unity
{
#pragma warning disable IDE0065
  using Color = UnityEngine.Color;
#pragma warning restore IDE0065

  public sealed class MultiHandLandmarkListAnnotation : ListAnnotation<HandLandmarkListAnnotation>
  {
    [SerializeField] Color _leftLandmarkColor = Color.green;
    [SerializeField] Color _rightLandmarkColor = Color.green;
    [SerializeField] float _landmarkRadius = 15.0f;
    [SerializeField] Color _connectionColor = Color.white;
    [SerializeField, Range(0, 1)] float _connectionWidth = 1f;

#if UNITY_EDITOR
    void OnValidate()
    {
      if (UnityEditor.PrefabUtility.IsPartOfAnyPrefab(this))
        return;
      ApplyLeftLandmarkColor(_leftLandmarkColor);
      ApplyRightLandmarkColor(_rightLandmarkColor);
      ApplyLandmarkRadius(_landmarkRadius);
      ApplyConnectionColor(_connectionColor);
      ApplyConnectionWidth(_connectionWidth);
    }
#endif

    public void SetLeftLandmarkColor(Color leftLandmarkColor)
    {
      _leftLandmarkColor = leftLandmarkColor;
      ApplyLeftLandmarkColor(_leftLandmarkColor);
    }

    public void SetRightLandmarkColor(Color rightLandmarkColor)
    {
      _rightLandmarkColor = rightLandmarkColor;
      ApplyRightLandmarkColor(_rightLandmarkColor);
    }

    public void SetLandmarkRadius(float landmarkRadius)
    {
      _landmarkRadius = landmarkRadius;
      ApplyLandmarkRadius(_landmarkRadius);
    }

    public void SetConnectionColor(Color connectionColor)
    {
      _connectionColor = connectionColor;
      ApplyConnectionColor(_connectionColor);
    }

    public void SetConnectionWidth(float connectionWidth)
    {
      _connectionWidth = connectionWidth;
      ApplyConnectionWidth(_connectionWidth);
    }

    public void SetHandedness([NotNull]IList<ClassificationList> handedness)
    {
      int count = handedness.Count;
      for (var i = 0; i < Mathf.Min(count, Children.Count); i++)
      {
        Children[i].SetHandedness(handedness[i]);
      }
      for (var i = count; i < Children.Count; i++)
      {
        Children[i].SetHandedness((IList<Classification>)null);
      }
    }

    public void Draw(IList<NormalizedLandmarkList> targets, bool visualizeZ = false)
    {
      if (ActivateFor(targets) is false)
        return;
      
      CallActionForAll(targets, (annotation, target) =>
      { 
        if (annotation != null) 
          annotation.Draw(target, visualizeZ);
      });
    }

    protected override HandLandmarkListAnnotation InstantiateChild(bool setActive = true)
    {
      var annotation = base.InstantiateChild(setActive);
      annotation.SetLeftLandmarkColor(_leftLandmarkColor);
      annotation.SetRightLandmarkColor(_rightLandmarkColor);
      annotation.SetLandmarkRadius(_landmarkRadius);
      annotation.SetConnectionColor(_connectionColor);
      annotation.SetConnectionWidth(_connectionWidth);
      return annotation;
    }

    void ApplyLeftLandmarkColor(Color leftLandmarkColor)
    {
      foreach (var handLandmarkList in Children.Where(handLandmarkList => handLandmarkList != null))
      { 
        handLandmarkList.SetLeftLandmarkColor(leftLandmarkColor);
      }
    }

    private void ApplyRightLandmarkColor(Color rightLandmarkColor)
    {
      foreach (var handLandmarkList in Children)
      {
        if (handLandmarkList != null) { handLandmarkList.SetRightLandmarkColor(rightLandmarkColor); }
      }
    }

    private void ApplyLandmarkRadius(float landmarkRadius)
    {
      foreach (var handLandmarkList in Children)
      {
        if (handLandmarkList != null) { handLandmarkList.SetLandmarkRadius(landmarkRadius); }
      }
    }

    private void ApplyConnectionColor(Color connectionColor)
    {
      foreach (var handLandmarkList in Children)
      {
        if (handLandmarkList != null) { handLandmarkList.SetConnectionColor(connectionColor); }
      }
    }

    private void ApplyConnectionWidth(float connectionWidth)
    {
      foreach (var handLandmarkList in Children)
      {
        if (handLandmarkList != null) { handLandmarkList.SetConnectionWidth(connectionWidth); }
      }
    }
  }
}
