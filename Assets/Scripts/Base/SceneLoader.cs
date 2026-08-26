using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SceneLoader : Singleton<SceneLoader>
{
    private Dictionary<string, int> sceneNumDict = new Dictionary<string, int>();
    private Dictionary<string, List<Action<string>>> sceneBeforeActionDict = new Dictionary<string, List<Action<string>>>();
    private Dictionary<string, List<Action<string>>> sceneActionDict = new Dictionary<string, List<Action<string>>>();
    private static string curScene_;
    public static string CurSceneName
    {
        get => curScene_;
        set
        {
            curScene_ = value;
        }
    }
    bool isChange = false;

    private void Awake()
    {
        Initialize();
        DontDestroyOnLoad(gameObject);

        SetSceneIndices();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #region Scene Setting
    private void SetSceneIndices()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            sceneNumDict[sceneName] = i;
            Debug.Log(sceneName);
        }
    }

    private static void SetScenes(List<ISceneLoadAction> l)
    {
        ComponentTypeFinder.Initialize();
        CurSceneName = SceneManager.GetActiveScene().name;
        Debug.Log(CurSceneName);
        foreach (var a in l)
        {
            a.SetSceneAction();
        }
    }

    private static void SetBeforeActionDict(List<ISceneLoadAction> l)
    {
        if (!Inst.sceneBeforeActionDict.ContainsKey(CurSceneName))
        {
            Inst.sceneBeforeActionDict[CurSceneName] = new List<Action<string>>();
        }
        foreach (var a in l)
        {
            Inst.sceneBeforeActionDict[CurSceneName].Add(a.BeforAction);
        }
    }

    private static void SetActionDict(List<ISceneLoadAction> l)
    {
        if (!Inst.sceneActionDict.ContainsKey(CurSceneName))
        {
            Inst.sceneActionDict[CurSceneName] = new List<Action<string>>();
        }
        foreach (var a in l)
        {
            Inst.sceneActionDict[CurSceneName].Add(a.AfterAction);
        }
    }
    #endregion

    #region  Scene Invoking
    private static void InvokeBeforeActionDict(string name)
    {
        if (!Inst.sceneBeforeActionDict.ContainsKey(name)) return;

        foreach (var a in Inst.sceneBeforeActionDict[name])
        {
            a.Invoke(name);
        }

    }

    private static void InvokeActionDict(string name)
    {
        if (!Inst.sceneActionDict.ContainsKey(name)) return;

        foreach (var a in Inst.sceneActionDict[name])
        {
            a.Invoke(name);
        }
    }
    #endregion
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RuntimeInitOnLoad()
    {
        List<ISceneLoadAction> l = ComponentTypeFinder.FindAllImplementing<ISceneLoadAction>();
        SetScenes(l);
        SetBeforeActionDict(l);
        SetActionDict(l);
        InvokeActionDict(CurSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        List<ISceneLoadAction> l = ComponentTypeFinder.FindAllImplementing<ISceneLoadAction>();
        SetScenes(l);
        SetBeforeActionDict(l);
        SetActionDict(l);
        InvokeActionDict(scene.name);
    }

    public static void ChangeScene(int i)
    {
        if (!Inst.isChange) Inst.StartCoroutine(Inst.Loading(i));
    }

    public static void ChangeScene(string scene)
    {
        InvokeBeforeActionDict(scene);

        ChangeScene(Inst.sceneNumDict[scene]);
    }

    IEnumerator Loading(int i)
    {
        isChange = true;
        yield return SceneManager.LoadSceneAsync(sceneNumDict["Loading"]);
        GameObject obj = GameObject.Find("LoadingGage");
        Slider slider = obj.GetComponent<Slider>();
        slider.value = 0.0f;
        yield return StartCoroutine(LoadingTarget(slider, i));
        isChange = false;
    }

    IEnumerator LoadingTarget(Slider slider, int i)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(i);
        // 씬로딩이 끝나기 전까진 씬을 활성화 시키지 않음
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            slider.value = ao.progress / 0.9f;
            if (Mathf.Approximately(slider.value, 1.0f))
            {
                // 씬로딩이 끝났으므로 씬 활성화
                ao.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
