// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe.Unity
{
  public abstract class ListAnnotation<T> : HierarchicalAnnotation where T : HierarchicalAnnotation
  {
    [SerializeField] GameObject _annotationPrefab;
    [SerializeField] List<T> _children = new();

    protected List<T> Children => _children ??= new List<T>();

    public List<T> GetChildren => Children;
    
    public T this[int index] => Children[index];

    public int Count => Children.Count;

    public void Fill(int count)
    {
      while (Children.Count < count)
      {
        Children.Add(InstantiateChild(false));
      }
    }

    public void Add(T element)
    {
      Children.Add(element);
    }

    public override bool isMirrored
    {
      set
      {
        foreach (T child in Children)
        {
          child.isMirrored = value;
        }
        base.isMirrored = value;
      }
    }

    public override RotationAngle rotationAngle
    {
      set
      {
        foreach (var child in Children)
        {
          child.rotationAngle = value;
        }
        base.rotationAngle = value;
      }
    }

    protected virtual void Destroy()
    {
      foreach (var child in Children)
      {
        Destroy(child);
      }
      _children = null;
    }

    protected virtual T InstantiateChild(bool setActive = true)
    {
      T annotation = InstantiateChild<T>(_annotationPrefab);
      annotation.SetActive(setActive);
      return annotation;
    }

    /// <summary>
    ///   Zip <see cref="Children" /> and <paramref name="argumentList" />, and call <paramref name="action" /> with each pair.
    ///   If <paramref name="argumentList" /> has more elements than <see cref="Children" />, <see cref="Children" /> elements will be initialized with <see cref="InstantiateChild" />.
    /// </summary>
    /// <param name="argumentList"></param>
    /// <param name="action">
    ///   This will receive 2 arguments and return void.
    ///   The 1st argument is <typeparamref name="T" />, that is an ith element in <see cref="Children" />.
    ///   The 2nd argument is <typeparamref name="TArg" />, that is also an ith element in <paramref name="argumentList" />.
    /// </param>
    protected void CallActionForAll<TArg>(IList<TArg> argumentList, Action<T, TArg> action)
    {
      for (var i = 0; i < Mathf.Max(Children.Count, argumentList.Count); i++)
      {
        if (i >= argumentList.Count)
        {
          // children.Count > argumentList.Count
          action(Children[i], default);
          continue;
        }

        // reset annotations
        if (i >= Children.Count)
        {
          // children.Count < argumentList.Count
          Children.Add(InstantiateChild());
        }
        else if (Children[i] == null)
        {
          // child is not initialized yet
          Children[i] = InstantiateChild();
        }
        action(Children[i], argumentList[i]);
      }
    }
  }
}
