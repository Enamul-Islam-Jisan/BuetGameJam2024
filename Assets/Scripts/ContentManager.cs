using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ContentManager : SingletonMonoBehaviour<ContentManager>
{
    [SerializeField]
    private TextMeshProUGUI downloadingText;

    [SerializeField]
    private List<AssetReference> scenes = new List<AssetReference>();

    [SerializeField]
    private UnityEvent downloadCompleted;
    private IEnumerable<object> keys;


    protected override void Awake()
    {
        base.Awake();
        keys = scenes.Select(s => s.RuntimeKey);
    }

    private IEnumerator Start()
    {
        yield return CheckContentStatus();
    }

    private IEnumerator CheckContentStatus()
    {
        AsyncOperationHandle<long> getSizeHandle = Addressables.GetDownloadSizeAsync(keys);
        yield return getSizeHandle;
        long size = getSizeHandle.Result;
        Addressables.Release(getSizeHandle);
        if (size == 0)
        {
            downloadingText.text = "";
            downloadCompleted?.Invoke();
            yield break;
        }
        yield return DownloadIfRequiredInternal();
    }

    private IEnumerator DownloadIfRequiredInternal()
    {
        AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(keys, Addressables.MergeMode.Union);
        do
        {
            DownloadStatus status = downloadHandle.GetDownloadStatus();
            downloadingText.text = $"Downloading Content - {status.Percent * 100:0}%";
            yield return null;
        } while (!downloadHandle.IsDone);
        Addressables.Release(downloadHandle);
        downloadCompleted?.Invoke();
        downloadingText.text = "";
    }
}
