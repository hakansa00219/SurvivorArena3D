using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class InitSingletons : MonoBehaviour
{
    private static InitSingletons Instance;
    private void Awake()
    {
        PermissionStuff();

        //add other singletons if needed
        Debug.Log("Start");
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(gameObject);
        }



        Debug.Log("SelectScene");
        SelectScene(SceneManager.GetActiveScene().name);       

    }

    private void PermissionStuff()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            //continue
            Debug.Log("write true");
        }
        else
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            Debug.Log("write");
            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite)) Application.Quit();

        }
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Debug.Log("read true");
        }
        else
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            Debug.Log("read");
            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead)) Application.Quit();
        }
        if (Permission.HasUserAuthorizedPermission("android.permission.INTERNET"))
        {
            Debug.Log("internet true");
        }
        else
        {
            Permission.RequestUserPermission("android.permission.INTERNET");
            Debug.Log("internet");
            if (Permission.HasUserAuthorizedPermission("android.permission.INTERNET")) Application.Quit();
        }
        if (Permission.HasUserAuthorizedPermission("android.permission.ACCESS_NETWORK_STATE"))
        {
            Debug.Log("network state true");
        }
        else
        {
            Permission.RequestUserPermission("android.permission.ACCESS_NETWORK_STATE");
            Debug.Log("network state true");
            if (Permission.HasUserAuthorizedPermission("android.permission.ACCESS_NETWORK_STATE")) Application.Quit();
        }
    }

    private void SelectScene(string sceneName)
    {
        switch (sceneName)
        {
            case "StartScene" :
                GameManager.Instance.Initialize();
                InternetConnCheckSc.Instance.Initialize();
                FirebaseDB.Instance.Initialize();
                PlayerData.Instance.Initialize();
                break;
            case "GameScene":
                GameManager.Instance.Initialize();
                InternetConnCheckSc.Instance.Initialize();
                FirebaseDB.Instance.Initialize();
                PlayerData.Instance.Initialize();
                MessageBox.Instance.Initialize();
                break;
            case "LoadingScene":
                GameManager.Instance.Initialize();
                InternetConnCheckSc.Instance.Initialize();
                FirebaseDB.Instance.Initialize();
                PlayerData.Instance.Initialize();

                break;
        }
    }
}
