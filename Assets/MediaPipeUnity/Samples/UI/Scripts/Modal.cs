// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.
// Modified by SpesMonkeh, 2023. 

using System.Collections.Generic;
using P307.Runtime.ComputerVision.MediaPipe;
using UnityEngine;
using static P307.Shared.Const307;
using Object = UnityEngine.Object;

namespace Mediapipe.Unity.UI
{
  public class Modal : MonoBehaviour
  {
    [SerializeField] Solution solution;
    [SerializeField] Vector3 contentLocalScale = new(.8f, .8f, 1f);

    int currentContentInstanceId = ZERO;
    int currentContentOwnerInstanceId = ZERO;
    int currentContentPrefabInstanceId = ZERO;
    Vector3 originalContentLocalScale;
    Vector3 originalContentWorldPosition;
    GameObject content;
    Component contentOwner;

    readonly Dictionary<int, GameObject> contentEntries = new();

    public bool IsAvailable => currentContentInstanceId is ZERO;
    public (Component ownerObj, int ownerId) Owner
    {
      get
      {
        if (content != null)
          return (contentOwner, currentContentOwnerInstanceId);
        
        contentOwner = null;
        currentContentOwnerInstanceId = ZERO;
        return (contentOwner, currentContentOwnerInstanceId);
      }
      private set
      {
        if (IsAvailable)
          return;
        (contentOwner, currentContentOwnerInstanceId) = (value.ownerObj, value.ownerId);
      }
    }
    
    
    public void Open(GameObject gameObj, bool allowMultipleInstances = false)
    {
      if (IsAvailable is false && allowMultipleInstances is false)
        return;
      int instanceId = gameObj.GetInstanceID();
      bool isAlreadyActive = instanceId == currentContentInstanceId;
      
      if (isAlreadyActive || allowMultipleInstances is false)
        return;
      SetContent(gameObj);
      gameObject.SetActive(true);
    }

    public void Open<TComponent>(GameObject gameObj, IModalContentOwner<TComponent> owner, bool allowMultipleInstances = false)
      where TComponent : Component
    {
      if (IsAvailable is false && allowMultipleInstances is false)
        return;
      int instanceId = gameObj.GetInstanceID();
      if (ContentAlreadyActive(instanceId, allowMultipleInstances) || owner.OwnerInstanceId is ZERO)
        return;
      GameObject newContent = GetModalContent<TComponent>(instanceId, gameObj);
      SetContentTo(newContent, instanceId, owner);
      gameObject.SetActive(true);
    }

    public void OpenAndPause(GameObject contents, bool allowMultipleInstances = false)
    {
      Open(contents);
      if (solution != null)
        solution.Pause();
    }

    public void OpenAndPause<T>(ref GameObject contents, IModalContentOwner<T> owner, bool allowMultipleInstances = false) where T : Component
    {
      Open(contents, owner);
      if (solution != null)
        solution.Pause();
    }
    
    public void Close()
    {
      if (currentContentInstanceId is ZERO || currentContentPrefabInstanceId is ZERO || content == null)
      {
        ResetCurrentContentValues();
        return;
      }
      CloseSpecificContentWithPrefabId(currentContentPrefabInstanceId);
    }
    
    public void CloseSpecificContentWithPrefabId(int instanceId)
    {
      if (content != null)
      {
        if (contentEntries.ContainsKey(instanceId))
          content.gameObject.SetActive(false);
        else
          Destroy(content.gameObject);
      }
      ResetCurrentContentValues();
      gameObject.SetActive(false);
    }
    
    public void Close<T>(ref GameObject prefabToClose, IModalContentOwner<T> owner) where T : Component
    {
      if (currentContentOwnerInstanceId != owner.OwnerInstanceId)
        return;
      CloseSpecificContentWithPrefabId(prefabToClose.GetInstanceID());
    }

    public void CloseAndResume(bool forceRestart = false)
    {
      CloseSpecificContentWithPrefabId(currentContentPrefabInstanceId);
      if (solution == null)
        return;

      if (forceRestart)
        solution.Play();
      else
        solution.Resume();
    }

    public void CloseAndResume<T>(IModalContentOwner<T> owner, bool forceRestart = false) where T : Component
    {
      Close(ref content, owner);
      if (solution == null)
        return;

      if (forceRestart)
        solution.Play();
      else
        solution.Resume();
    }

    void Awake()
    {
      gameObject.SetActive(false);
    }
    
    GameObject InstantiateNewModal(GameObject prefab)
    {
      if (prefab == null)
        return null;
      var newObject = Instantiate(prefab, gameObject.transform);
      newObject.transform.localScale = contentLocalScale;
      newObject.transform.SetParent(gameObject.transform, false);
      int instanceId = prefab.GetInstanceID();
      contentEntries.Add(instanceId, newObject);
      return contentEntries[instanceId];
    }

    bool CheckIfContentCanBeTransferred(Object requestedObj, GameObject owner)
    {
      bool argumentsAreNotInvalid = requestedObj != null && owner != null;
      if (argumentsAreNotInvalid)
        return ValidateOwnerAsRequesterWithId(owner.GetInstanceID())
               && ValidateContentAsRequestedObjectWithId(requestedObj.GetInstanceID());
      
      P307.Runtime.Debug.Log(
        $"Requester Object with Instance Id: {owner} "
        + $"tried transferring a Content Object, {requestedObj}, " 
        + $"from Modal: {name}. "
        +"Requester Id cannot be 0 and Content Object cannot be NULL.");
      return false;
    }

    bool ValidateContentAsRequestedObjectWithId(int objectId)
      => currentContentInstanceId is not ZERO
         && objectId == currentContentInstanceId;

    bool ValidateOwnerAsRequesterWithId(int requesterId)
      => currentContentOwnerInstanceId is not ZERO 
         && requesterId == currentContentOwnerInstanceId;
    
    bool ContentAlreadyActive(int instanceId, bool allowMultiple) => instanceId == currentContentInstanceId && allowMultiple is false;

    GameObject GetModalContent<T>(int instanceId, GameObject gameObj) where T : Component
    {
      bool isNewEntry = contentEntries.ContainsKey(instanceId) is false;
      GameObject newContent = isNewEntry
        ? InstantiateNewModal(gameObj)
        : contentEntries[instanceId];
      return newContent;
    }
    
    void SetContentTo<TComponent>(GameObject newContent, int prefabId, IModalContentOwner<TComponent> owner) where TComponent : Component
    {
      content = newContent;
      contentOwner = owner.Owner;
      currentContentInstanceId = content.GetInstanceID();
      currentContentOwnerInstanceId = contentOwner.GetInstanceID();
      currentContentPrefabInstanceId = prefabId;
      content.gameObject.SetActive(true);
    }
    
    void SetContent(GameObject newContent)
    {
      content = newContent;
    }

    void ResetCurrentContentValues()
    {
      currentContentInstanceId = ZERO;
      currentContentOwnerInstanceId = ZERO;
      currentContentPrefabInstanceId = ZERO;
      content = null;
      contentOwner = null;
    }
  }
}
