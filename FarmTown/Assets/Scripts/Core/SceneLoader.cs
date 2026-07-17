using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public Text text;
    public Image img;
    private int j;
    float time = 3f;
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (!PlayerPrefs.HasKey("firstOpen"))
        {
            PlayerPrefs.SetInt("firstOpen", 1);
            FirstLoad(2);
        }
        else
        {
            FirstLoad(1);
        }
    }

 
    public void Load(int id)
    {
        img.fillAmount = 0;
        transform.GetChild(0).gameObject.SetActive(true);
        text.text = "0%";
        StartCoroutine(LoadScene(id));
    }
    IEnumerator LoadScene(int idScene)
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(idScene, LoadSceneMode.Single);
        //asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            img.fillAmount = asyncOperation.progress;
            text.text = (asyncOperation.progress * 100) + "%";
            yield return null;
        }
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void FirstLoad(int id)
    {
        img.fillAmount = 0;
        img.DOFillAmount(1, 3f).SetEase(Ease.Linear);
        StartCoroutine(DelayTime(id));

    }
    int temp = 0;
    IEnumerator DelayTime(int idScene)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(idScene);
        asyncOperation.allowSceneActivation = false;
        for (int i = 0; i < 100; i++)
        {
            temp++;
            text.text = temp + "%";
            yield return new WaitForSeconds(time / 100);
            if (temp == 98)
                AppOpenAdController.Instance.ShowAdIfAvailable();
        }
        asyncOperation.allowSceneActivation = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
