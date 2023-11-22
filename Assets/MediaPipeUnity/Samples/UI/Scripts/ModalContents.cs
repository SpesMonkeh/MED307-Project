// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;

namespace Mediapipe.Unity.UI
{
  public class ModalContents : MonoBehaviour
  {
    protected Modal GetModal() => transform.parent.TryGetComponent(out Modal m) ? m : null;

    protected bool TryGetModal(out Modal m) => transform.parent.TryGetComponent(out m);
    
    public virtual void Exit()
    {
      if (TryGetModal(out Modal m) is false)
        return;
      m.Close();
    }
  }
}
