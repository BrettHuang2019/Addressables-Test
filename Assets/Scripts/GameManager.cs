using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public UIManager _UIManager;
    public Slider progressBar;
    public List<AssetReference> levelReferences = new List<AssetReference>();
    public AsyncOperationHandle<SceneInstance> handle;
    public Text debugText;
    private int sceneDownloaded;

    private static GameManager _instance;

    public static GameManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _UIManager.Init();

        for (int i = 0; i < levelReferences.Count; i++)
        {
            StartCoroutine(GetDownloadSize(i));
        }
    }

    private void GetBundleStatus()
    {
        foreach (var level in levelReferences)
        {
            
        }
    }

    public void ClearCash(int levelIndex)
    {
        Addressables.ClearDependencyCacheAsync(levelReferences[levelIndex].RuntimeKey);
    }

    private IEnumerator GetDownloadSize(int levelIndex)
    {
        Debug.Log(levelIndex);
        AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(levelReferences[levelIndex].RuntimeKey);
        yield return getDownloadSize;

        Debug.Log(getDownloadSize.Result.ToString());
        if (getDownloadSize.Result > 0)
        {
            DebugLog(getDownloadSize.Result.ToString());
            _UIManager.InitLevelBtn(levelIndex, true);
        }
        else
            _UIManager.InitLevelBtn(levelIndex, false);
    }

    public void LoadScene(int sceneIndex)
    {
        _UIManager.OnEnterLevel();
        var loadScene = Addressables.LoadSceneAsync(levelReferences[sceneIndex], LoadSceneMode.Additive);
        loadScene.Completed += obj => handle = obj;
    }

    public void OpenDownloadPanel(int index)
    {
        Addressables.GetDownloadSizeAsync(levelReferences[index].RuntimeKey).Completed += op =>
        {
            _UIManager.ShowDownloadPanel(true, index,0, op.Result);
        };
    }

    public void StartDownloadScene(int index)
    {
        StartCoroutine(DownloadScene(index));
    }
    
    IEnumerator DownloadScene(int index)
    {
        var downloadScene = Addressables.DownloadDependenciesAsync(levelReferences[index], false);
        downloadScene.Completed += SceneDownloadComplete;
        sceneDownloaded = index;
        

        while (!downloadScene.IsDone)
        {
            var status = downloadScene.GetDownloadStatus();
            float progress = status.Percent;
            _UIManager.ShowDownloadPanel(true, index, progress);
            yield return null;
        }
    }

    private void SceneDownloadComplete(AsyncOperationHandle obj)
    {
        _UIManager.ShowDownloadPanel(false);
        _UIManager.InitLevelBtn(sceneDownloaded,false);
    }

    public void OpenDeletePanel(int index)
    {
        _UIManager.ShowDownloadPanel(false);
    }

    public void UnloadScene()
    {
        Addressables.UnloadSceneAsync(handle, true).Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                DebugLog("Successfully unloaded scene.");
                _UIManager.OnExitLevel();
            }
        };
    }

    private void DebugLog(string log)
    {
        debugText.text += log + "\n";
    }
}
