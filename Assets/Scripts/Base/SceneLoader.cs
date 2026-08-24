using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class SceneLoader : Singleton<SceneLoader>
{
    private Dictionary<string, int> sceneNumDict = new Dictionary<string, int>();
    private Dictionary<string, UnityAction> sceneBeforeActionDict = new Dictionary<string, UnityAction>();
    private Dictionary<string, UnityAction> sceneActionDict = new Dictionary<string, UnityAction>();

    bool isChange = false;

    private void Awake()
    {
        base.Initialize();
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

    private void SetSceneIndices()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            sceneNumDict[sceneName] = i;
        }
    }

    private static void SetScenes()
    {
        
    }

    private static void SetBeforeActionDict()
    {
        
    }

    private static void SetActionDict()
    {
        
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RuntimeInitOnLoad()
    {
        SetScenes();
        SetBeforeActionDict();
        SetActionDict();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetScenes();
        if (sceneActionDict.ContainsKey(scene.name) == false) return;
        sceneActionDict[scene.name]?.Invoke();
    }

    public static void ChangeScene(int i)
    {
        if (!Inst.isChange) Inst.StartCoroutine(Inst.Loading(i));
    }

    public static void ChangeScene(string scene)
    {
        if (Inst.sceneBeforeActionDict.ContainsKey(scene))
        {
            Inst.sceneBeforeActionDict[scene]?.Invoke();
        }

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
