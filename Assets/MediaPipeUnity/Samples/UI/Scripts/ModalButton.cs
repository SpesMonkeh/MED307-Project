// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.
// Modified by SpesMonkeh, 2023. 

using P307.Runtime.ComputerVision.MediaPipe;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mediapipe.Unity.UI
{
  public class ModalButton : MonoBehaviour, IModalContentOwner<ModalButton>
  {
    [SerializeField] Modal modal;
    [SerializeField] GameObject _contents;

    int contentPrefabInstanceId;

    public ModalButton Owner => this;
    public Object Content => _contents;

    bool ContentsAreTransferable => modal is { IsAvailable: true }
                                    && _contents != null;

    void Awake()
    {
      modal = transform.root.GetComponentInChildren<Modal>(includeInactive: true);
      contentPrefabInstanceId = _contents.GetInstanceID();
    }

    public void ToggleModal()
    {
      if (ContentsAreTransferable)
        modal.Open(_contents, this, false);
      else
        modal.CloseSpecificContentWithPrefabId(contentPrefabInstanceId);
    }
    
    public void Open() => modal.Open(_contents, this);

    public void Close() => modal.CloseSpecificContentWithPrefabId(contentPrefabInstanceId);

    public void OpenAndPause()
    {
      if (_contents != null)
        modal.OpenAndPause(ref _contents, this);
    }
  }
}
