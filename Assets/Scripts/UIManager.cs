using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject downloadPanel;
    public GameObject deleteConfirmPanel;
    public GameObject levelBtnsPanel;
    public GameObject[] levelBtns;
    private int currentLevelIndex;

    public void Init()
    {
        downloadPanel.SetActive(false);
        deleteConfirmPanel.SetActive(false);
        ResetLevelBtns();
    }

    public void OnEnterLevel()
    {
        downloadPanel.SetActive(false);
        deleteConfirmPanel.SetActive(false);
        levelBtnsPanel.SetActive(false);
    }
    
    public void OnExitLevel()
    {
        levelBtnsPanel.SetActive(true);
    }



    public void ResetLevelBtns()
    {
        foreach (var button in levelBtns)
        {
            button.GetComponent<Button>().interactable = false;
            button.transform.Find("DownloadIcon").gameObject.SetActive(true);
            button.transform.Find("DeleteBtn").gameObject.SetActive(false);
        }
    }

    public void InitLevelBtn(int index, bool isRemote)
    {
        Button button = levelBtns[index].GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        if (isRemote)
        {
            button.transform.Find("DownloadIcon").gameObject.SetActive(true);
            button.transform.Find("DeleteBtn").gameObject.SetActive(false);
            button.interactable = true;
            button.onClick.AddListener(() => GameManager.Instance.OpenDownloadPanel(index));
        }
        else
        {
            button.transform.Find("DownloadIcon").gameObject.SetActive(false);
            if (index != 0)
                button.transform.Find("DeleteBtn").gameObject.SetActive(true);
            else
                button.transform.Find("DeleteBtn").gameObject.SetActive(false);

            button.onClick.AddListener(() => GameManager.Instance.LoadScene(index));
            button.interactable = true;
        }
    }

    public void ShowDownloadPanel(bool enable, int levelIndex = 0, float percent = 0, long size = 0)
    {
        downloadPanel.transform.Find("DownloadBtn").gameObject.SetActive(true);
        downloadPanel.transform.Find("CancelBtn").gameObject.SetActive(true);
        
        currentLevelIndex = levelIndex;
        downloadPanel.SetActive(enable);
        downloadPanel.transform.Find("Percent").GetComponent<Text>().text = $"{percent * 100:0.0}" + "%";
        downloadPanel.transform.Find("ProgressBar").GetComponent<Slider>().value = percent;
        if (size != 0)
            downloadPanel.transform.Find("Size").GetComponent<Text>().text = "Size: " + $"{(size / 1214f) / 1024f:0.00}" + "MB";
    }

    public void OnDownloadConfirmed()
    {
        downloadPanel.transform.Find("DownloadBtn").gameObject.SetActive(false);
        downloadPanel.transform.Find("CancelBtn").gameObject.SetActive(false);
        GameManager.Instance.StartDownloadScene(currentLevelIndex);
    }
    public void OnDownloadCancelled()
    {
        ShowDownloadPanel(false);
    }

    public void ShowDeletePanel(bool enable, int levelIndex = 0, float percent = 0, long size = 0)
    {
        
    }
}
