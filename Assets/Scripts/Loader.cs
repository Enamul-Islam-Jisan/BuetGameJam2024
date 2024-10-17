using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class Loader : MonoBehaviour
{
    public static Loader Current;
    [SerializeField]
    private string targetSceneName;
    [SerializeField]
    private string loaderSceneName = "Loading";
    [SerializeField]
    private float manualDelay = 2f;
    [SerializeField]
    private bool autoBegin = false;
    private Transition transition;

    public static event Action loadingStarted;
    public static event Action<float> loadingOngoing;
    public static event Action loadingEnded;
    private static AsyncOperationHandle<SceneInstance> loadedSceneHandle;
    private static bool initializationDone = false;

    private void Awake()
    {
        transition = GetComponentInChildren<Transition>();
        if(autoBegin)
            Begin();
    }

    public void Begin()
    {
        if (Current) return;
        Current = this;
        StartCoroutine(BeginInternal());
    }

    private IEnumerator BeginInternal()
    {
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        if (transition)
            yield return transition.DoOut();
        foreach(GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            go.SetActive(false);
        }
        loadingStarted?.Invoke();
        if (!string.IsNullOrEmpty(loaderSceneName))
        {

            AsyncOperationHandle<SceneInstance> loadingSceneHandle = Addressables.LoadSceneAsync(loaderSceneName, LoadSceneMode.Additive, true);

            yield return loadingSceneHandle;

            if (initializationDone && loadedSceneHandle.IsValid() && initializationDone)
            {
                AsyncOperationHandle<SceneInstance> unloadingSceneHandle = Addressables.UnloadSceneAsync(loadedSceneHandle.Result);
                yield return unloadingSceneHandle;
            }

            if (transition)
                yield return transition.DoIn();

            loadedSceneHandle = Addressables.LoadSceneAsync(targetSceneName, LoadSceneMode.Single, false);
            yield return new WaitForSeconds(manualDelay);
            do
            {
                loadingOngoing?.Invoke(loadedSceneHandle.PercentComplete);
                yield return null;
            } while (!loadedSceneHandle.IsDone);
            if (transition)
                yield return transition.DoOut();
            yield return loadedSceneHandle.Result.ActivateAsync();
            if (transition)
                yield return transition.DoIn();
        }
        else
        {
            if (loadedSceneHandle.IsValid() && loadedSceneHandle.IsDone && initializationDone)
            {
                AsyncOperationHandle<SceneInstance> unloadingSceneHandle = Addressables.UnloadSceneAsync(loadedSceneHandle.Result, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                yield return unloadingSceneHandle;
            }

            loadedSceneHandle = Addressables.LoadSceneAsync(targetSceneName, LoadSceneMode.Single, false);
            yield return new WaitForSeconds(manualDelay);
            do
            {
                loadingOngoing?.Invoke(loadedSceneHandle.PercentComplete);
                yield return null;
            } while (!loadedSceneHandle.IsDone);
            yield return loadedSceneHandle.Result.ActivateAsync();
            if (transition)
                yield return transition.DoIn();
        }
        loadingEnded?.Invoke();
        if (!initializationDone)
        {
            initializationDone = true;
        }
        Destroy(gameObject);
    }

}
