using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private UnityEngine.UI.Image _loadingbar;
    private TextMeshProUGUI _details;
    private bool ADMOB;
    private bool Firebase;
    

    public static GameManager Instance
    { 
        get
        {
            if(instance == null)
            {
                GameObject gameobject = Resources.Load("GameManager") as GameObject;
                if (gameobject == null) Debug.Log("gamemanager null");
                if(GameObject.FindObjectOfType<GameManager>() == null) 
                {
                    instance = Instantiate(gameobject).GetComponent<GameManager>();
                } else
                {
                    instance = FindObjectOfType<GameManager>();
                }
                DontDestroyOnLoad(instance);
                //instance.ChangeSceneRoutine("StartScene");
            }
            return instance;
        }
    }
    private void Start()
    {
        StartCoroutine(FirstStart());
        if (SceneManager.GetActiveScene().name == "LoadingScene")
        {
            GameObject loading = Instantiate(Loading, FindObjectOfType<Canvas>().transform);
            _loadingbar = loading.transform.Find("loadingbar").GetComponent<UnityEngine.UI.Image>();
            _details = loading.transform.Find("Details").GetComponent<TextMeshProUGUI>();
        }
            
    }

    private IEnumerator FirstStart()
    {
        yield return new WaitForSeconds(2f);
        if (SceneManager.GetActiveScene().name == "LoadingScene")
        {
            GameObject loadImage = Instantiate(loadingImage, FindObjectOfType<Canvas>().transform);
            loadImage.transform.SetAsFirstSibling();
            //Checking internet connection, bar %20
            var connection = InternetConnCheckSc.Instance.CheckForInternetConnection();
            //write checking internet connection for 1 sec.
            _details.SetText("Checking for internet connection...");
            yield return new WaitForSeconds(1f);
            if (connection) 
            {
                _details.SetText("Connection found.");
                _loadingbar.fillAmount = 0.1f;
                yield return new WaitForSeconds(0.5f);
            }
            else 
            {
                _details.SetText("No internet connection.");
                int cnt = 0;
                while (!connection)
                {
                    connection = InternetConnCheckSc.Instance.CheckForInternetConnection();
                    yield return new WaitForSeconds(0.5f);
                    cnt++;
                    if (cnt > 20)
                    {
                        _details.SetText("Check your internet connection and restart application.");
                        yield break;
                    }
                }

                _details.SetText("Connection found.");
                _loadingbar.fillAmount = 0.1f;
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.5f);
            //firebase %40
            if(Firebase)
            {
                Debug.Log("its on.");
                _details.SetText("Loading user data...");
                _loadingbar.fillAmount = 0.3f;
                yield return new WaitForSeconds(0.5f);

            } else
            {
                _details.SetText("Trying to load user data...");
                Debug.Log("its off.");
                yield return new WaitForSeconds(0.5f);
                int cnt = 0;
                while (!Firebase)
                {                   
                    cnt++;
                    if(cnt > 2000)
                    {
                        _details.SetText("Cannot load user data.Please restart application.");
                        yield break;
                    }
                    yield return new WaitForEndOfFrame();
                }
                _details.SetText("User data loaded.");
                _loadingbar.fillAmount = 0.3f;
            }
            yield return new WaitForSeconds(0.5f);
            //ADMOB %60
            if (ADMOB)
            {
                _details.SetText("Assets loading...");
                _loadingbar.fillAmount = 0.6f;
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                _details.SetText("Trying to load assets...");
                int cnt = 0;
                while (!ADMOB)
                {
                   
                    cnt++;
                    if (cnt > 100)
                    {
                        //ADMOB = true;
                        //_details.SetText("Cannot load assets.Please restart application.");
                        break;
                        //yield break;
                    }
                    yield return new WaitForEndOfFrame();
                }
                _details.SetText("Assets loaded.");
                _loadingbar.fillAmount = 0.6f;
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.5f);
            //Scene loading. other %60-100
            _details.SetText("Loading game settings...");
            yield return new WaitForSeconds(1f);
            AsyncOperation mainmenu = SceneManager.LoadSceneAsync("StartScene");         
            while (mainmenu.progress < 1)
            {
                //still loading
                _details.SetText("Almost over...");
                _loadingbar.fillAmount = 0.6f + mainmenu.progress * 0.4f;
                yield return new WaitForEndOfFrame();
            }
            Destroy(_details);
            Destroy(_loadingbar);
            Destroy(loadImage);
        }
        
        yield return new WaitForEndOfFrame();
    }
    /// <summary>
    /// sceneIndex : After (loadingtime) seconds changes scene to selected scene name .(0:LoadingScene , 1:StartScene 2:GameScene ...(might add more later))
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <returns></returns>
    private IEnumerator ChangeScene(int sceneIndex)
    {
        StartCoroutine(LoadAsyncOperation(sceneIndex));
        yield return new WaitForEndOfFrame();
    }
    /// <summary>
    /// sceneIndex : After (loadingtime) seconds changes scene to selected scene name .(0:LoadingScene , 1:StartScene 2:GameScene ...(might add more later))
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator ChangeScene(string sceneName)
    {
        StartCoroutine(LoadAsyncOperation(sceneName));
        yield return new WaitForEndOfFrame();

    }
    public void ChangeSceneRoutine(string sceneName)
    {
        StartCoroutine(ChangeScene(sceneName));
    }
    public void ChangeSceneRoutine(int sceneIndex)
    {
        StartCoroutine(ChangeScene(sceneIndex));
    }
    public void Initialize()
    {

    }
    private IEnumerator LoadAsyncOperation(string sceneName)
    {
        GameObject loadImage = Instantiate(loadingImage, FindObjectOfType<Canvas>().transform);
        GameObject loading = Instantiate(Loading, FindObjectOfType<Canvas>().transform);
        _loadingbar = loading.transform.Find("loadingbar").GetComponent<UnityEngine.UI.Image>();
        _details = loading.transform.Find("Details").GetComponent<TextMeshProUGUI>();
        _details.SetText("Loading assets...");
        loadImage.transform.SetAsLastSibling();
        loading.transform.SetAsLastSibling();
        yield return new WaitForSeconds(1f);
        AsyncOperation mainmenu = SceneManager.LoadSceneAsync(sceneName);       
        while (mainmenu.progress < 1)
        {
            //still loading
            _details.SetText("Almost over...");
            _loadingbar.fillAmount = mainmenu.progress;
            yield return new WaitForEndOfFrame();

        }
        Destroy(loading);
        Destroy(loadImage);
        yield return new WaitForEndOfFrame();

    }
    private IEnumerator LoadAsyncOperation(int sceneIndex)
    {
        GameObject loadImage = Instantiate(loadingImage, FindObjectOfType<Canvas>().transform);
        GameObject loading = Instantiate(Loading, FindObjectOfType<Canvas>().transform);
        _loadingbar = loading.transform.Find("loadingbar").GetComponent<UnityEngine.UI.Image>();
        _details = loading.transform.Find("Details").GetComponent<TextMeshProUGUI>();
        _details.SetText("Loading assets...");
        loadImage.transform.SetAsLastSibling();
        loading.transform.SetAsLastSibling();
        yield return new WaitForSeconds(1f);
        AsyncOperation mainmenu = SceneManager.LoadSceneAsync(sceneIndex);
        while (mainmenu.progress < 1)
        {
            //still loading
            _details.SetText("Almost over...");
            _loadingbar.fillAmount = mainmenu.progress;
            yield return new WaitForEndOfFrame();

        }
        Destroy(loading);
        Destroy(loadImage);
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator IntADLoadingRoutine(MyAdScript _adManager)
    {
        yield return new WaitForSeconds(4);
        if (_adManager.IsIntAdLoaded())
        {
            ADLoadingImage(_adManager);
            _adManager.ShowIntAd();
            Debug.Log("int ad shown");

        }
        else
        {
            Debug.Log("Ad isnt loaded.");
        }
    }

    public GameObject loadingImage;
    public GameObject Loading;
    public GameObject ADLoading;

    public IEnumerator ADLoadingImage(MyAdScript myAd)
    {
        GameObject adLoadingImage = Instantiate(ADLoading, FindObjectOfType<Canvas>().transform);
        adLoadingImage.transform.SetAsLastSibling();
        while(!myAd._RewAdOpened && !myAd._IntAdOpened)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Destroy called?");
        Destroy(adLoadingImage);
        // if ad closed close the image aswell or if ad opened.
    }

    public void LoadAd(MyAdScript _adManager)
    {
        StartCoroutine(IntADLoadingRoutine(_adManager));
    }

    public void FirebaseCompleted(bool completed)
    {
        Firebase = completed;
    }
    public void ADMOBCompleted(bool completed)
    {
        ADMOB = completed;
    }
}
