using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Character
{
    [SerializeField] private float _pullingRadius;
    [Header("Pull Foods")] [SerializeField] private bool _foodPullingActive = false;
    [SerializeField] private SphereCollider _pullingPower;
    [Header("Ad")] [SerializeField] private MyAdScript _adManager;
    [Header("UI")] [SerializeField] private GameObject _panelOnDeath;
    [SerializeField] private Button _fuzeButton;
    [SerializeField] private TextMeshProUGUI _currentBest;
    [SerializeField] private TextMeshProUGUI _currentTicketCount;
    [Header("DeathCam")] [SerializeField] private Camera _deathCam;
    [Header("Canvas")] [SerializeField] private RectTransform _canvas;
    [SerializeField] private Button _jumpButton;
    private Camera _mainCamera;
    [SerializeField] private GameObject _cameraParent;


    [SerializeField] private FoodCollision _foodCollision;
    private Texture _playerTexture;
    private Material _playerMat;
    private GameObject _fuzeAim;
    private Collider other;
    private UInt32 _currentHighestScore = 1;
    public static event Action<string, UInt32> IamSpawned;
    private static Player playerInstance;

    private void Awake()
    {
        //_mainCamera = _cameraParent.GetComponentInChildren<Camera>();
        _fuzeAim = transform.Find("Aim").gameObject;
        /*Set player texture*/
        _playerTexture = PlayerData.Instance.GetSkinsList()[PlayerData.Instance.GetPlayerChoosenSkinIdx()].texture;
        _playerMat = GetComponent<MeshRenderer>().material;
        Material mat = new Material(_playerMat);
        mat.SetTexture("_BaseMap", _playerTexture); 
        this.GetComponent<MeshRenderer>().material = mat;
        this.name = PlayerData.Instance.GetPlayerUserName();
        
    }
    private void Start()
    {
        ////int players = FindObjectsOfType<Player>().Length;
        ////if (players != 1)
        ////{
        ////    Destroy(this.gameObject);

        ////}
        ////// if more then one music player is in the scene
        //////destroy ourselves
        ////else
        ////{
        ////    DontDestroyOnLoad(gameObject);
        ////}
        //GameObject camPrefab = Instantiate(_cameraParent, this.transform);
        //GameObject maincam = camPrefab.transform.Find("Main Camera").gameObject;
        //CameraFogClip fogclip = maincam.AddComponent<CameraFogClip>();
        //fogclip._cameraTransform = camPrefab.transform;
        _currentPoint = 1;
        _deathCam.enabled = false;
        IamSpawned?.Invoke(name, GetCurrentPoint());
        GameTime.GameFinishedEvent += OnDeath;
        //transform.Find("CameraParent").Find("MainCamera").GetComponent<CameraFogClip>().enabled = true;
        StartCoroutine(UpdateCurrentValues());
    }

    private  void FixedUpdate()
    {
        _pullingPower.radius = _foodPullingActive ? _pullingRadius : 0f;
        
        //EatAnimation();
        if (trigger)
        {
            if (other != null)
            {
                /*Player eats enemy*/
                if (other.tag == "Enemy" && other.isTrigger)
                {
                    if (other == null) return;
                    _othersPoint = other.transform.parent.GetComponent<Enemy>().GetCurrentPoint();
                    if ((_currentPoint / _othersPoint) > 1.33f) // 3 / 4 oran
                    {
                        EatSomething(_othersPoint);
                        MessageBox.Instance.CreateText(PlayerData.Instance.GetPlayerUserName(), other.transform.parent.name, MessageBox.Action.Eaten);
                        Destroy(other.transform.parent.gameObject);
                        trigger = false;
                    }
                }
            }
        }
       
        CalculateBestScore();
    }

    private void Update()
    {



        if (_hasMissile)
        {
            //fuze buttonu aç
            _fuzeButton.interactable = true;
            //fuze aimi aç
            _fuzeAim.SetActive(true);

        }
        else
        {
            _fuzeButton.interactable = false;
            _fuzeAim.SetActive(false);
            //kapa
        }

        //for debug only
        if (Input.GetKeyDown(KeyCode.E))
        base.ShootMissile();
    }
    
    public void ShootMissileButtonClicked()
    {
        base.ShootMissile();
    }

    bool trigger;

    private void CalculateBestScore()
    {
        if (_currentPoint > _currentHighestScore) _currentHighestScore = _currentPoint;
    }

 

    public void OnDeath()
    {
        if (_currentHighestScore > PlayerData.Instance.GetPlayerBestScore()) PlayerData.Instance.SetPlayerBestScore(_currentHighestScore);
        if (PlayerData.Instance.GetPlayerBestScore() == 14000) PlayerData.Instance.SetPlayerBestScore(0);
        Debug.Log("Player Best Score = " + PlayerData.Instance.GetPlayerBestScore());
        Debug.Log("Current score = " + _currentHighestScore);
        Enemy.isPlayerDead = true;
        //update db for coin
        PlayerData.Instance.SetPlayerCurrentAdWatchedCount(_foodCollision.adCount);
        //disable player
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
        ////_mainCamera.gameObject.SetActive(false);
        //disable rendering
        //GetComponent<FindClosestEnemy>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        //open death cam
        //_mainCamera.enabled = false;
        _deathCam.enabled = true;
        _deathCam.GetComponent<AudioListener>().enabled = true;

        _panelOnDeath.SetActive(true);        
        if(_adManager.IsIntAdLoaded())
        {
            Debug.Log("start int ad in 4 seconds.");
            GameManager.Instance.LoadAd(_adManager);

        } else
        {
            Debug.Log("ad didnt load.");
        }
    }
    public void Restart()
    {
        GameManager.Instance.ChangeSceneRoutine("GameScene");
        _deathCam.enabled = false;
        //_mainCamera.enabled = true;
        _panelOnDeath.SetActive(false);
        ////GetComponent<MeshRenderer>().enabled = true;
        ////_mainCamera.gameObject.SetActive(true);
        Debug.Log("Restart Clicked.");
        //loading image gerekebilir.
        
        //en sonda player yaşıyo de
        Enemy.isPlayerDead = false;
        

    }
    public void Exit()
    {
        _panelOnDeath.SetActive(false);
        Debug.Log("exit clicked.");
        //loading image eklencek
        RectTransform _loadingImage = Instantiate(GameAssets.i.LoadingImage,_canvas.transform);
        _loadingImage.sizeDelta = new Vector2(_canvas.rect.width, _canvas.rect.height);
        _loadingImage.gameObject.SetActive(true);
        GameManager.Instance.ChangeSceneRoutine("StartScene");
    }
    //public void ContinueReward()
    //{
    //    try
    //    {
    //        //disable popup
    //        _panelOnDeath.SetActive(false);
    //        //disable deathcam
    //        _deathCam.enabled = false;
    //        //activate player
    //        this.gameObject.SetActive(true);
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log(e);
    //    }

    //    StartCoroutine(ImmunityTimer());
    //    //en sonda player yaşıyo de.
    //    Enemy.isPlayerDead = false;
    //    if (this.gameObject.transform.localScale.x > 2f)
    //    {
    //        //TODO
    //        //UpdateScaleAnimation(this.gameObject.transform.localScale * 0.5f * -1f); 
    //    } else
    //    {
    //        //TODO
    //        //UpdateScaleAnimation(new Vector3(1f, 1f, 1f));
    //    }       
    //    //UpdateScoreboard();

    //}
    //private IEnumerator ImmunityTimer()
    //{
    //    yield return new WaitForSeconds(5f);
    //    GetComponent<MeshRenderer>().enabled = true;
    //}

    private IEnumerator UpdateCurrentValues()
    {
        //if(PlayerData.Instance.GetPlayerBestScore() > _currentHighestScore) _currentBest.SetText("Your Current Best = " + PlayerData.Instance.GetPlayerBestScore().ToString());
        //else _currentBest.SetText("Your Current Best = " + _currentHighestScore.ToString());
        while(true)
        {
            _currentBest.SetText("Your Current Best = " + _currentHighestScore.ToString());
            _currentTicketCount.SetText("Coins = " + _foodCollision.adCount);
            yield return new WaitForSeconds(1f);
        }        
    }
    private void OnDestroy()
    {
        GameTime.GameFinishedEvent -= OnDeath;
    }

}
