using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFogClip : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private AudioClip smallEat;
    [SerializeField] private AudioClip largeEat;
    [SerializeField] private AudioClip hitMissile;
    [SerializeField] private AudioClip coin;
    private AudioSource _source;
    public Transform _cameraTransform;
    // Start is called before the first frame update
    private void Start()
    {
        _camera = transform.GetComponent<Camera>();
        _source = GetComponent<AudioSource>();
        //Debug.Log("Camera: " + _camera);
        //Debug.Log("Source: " + _source);
        Player.IEatSomethingEvent += this.PlayerScaleIncreased;
        
        
    }

    public void PlayerScaleIncreased(string name, UInt32 totalPoint, EatType eatType)
    {
        if(eatType != EatType.ET_COIN)
        {
            if (name != PlayerData.Instance.GetPlayerUserName()) return;

            Vector3 scale = CalculateScale(totalPoint);

            //IncreaseFarClipPlane(scale.x);
            //IncreaseFog(scale.x);
            IncreaseRotationOfCamera(scale.x);
        }        
        PlaySound(eatType);
    }

    private void PlaySound(EatType type)
    {
      
        
        if (type == EatType.ET_SMALL)
        {
            _source.PlayOneShot(smallEat, 0.5f);
        }else if(type == EatType.ET_LARGE)
        {
            _source.PlayOneShot(largeEat, 0.6f);
        }
        else if(type == EatType.ET_MISSILE)
        {
            _source.PlayOneShot(hitMissile, 0.3f);
        }
        else if (type == EatType.ET_COIN)
        {
            _source.PlayOneShot(coin, 0.5f);
        }
        else
        {
            Debug.Log("this log should not be printed. Check if printed.");
        }
    }

    void IncreaseFarClipPlane(float scale)
    {
        _camera.farClipPlane = 1000 + (scale * 100f);
    }
    void IncreaseFog(float scale)
    {
        RenderSettings.fogStartDistance = 10 + (scale * 10f);
        RenderSettings.fogEndDistance = 50 + (scale * 20f);
    }
    
    void IncreaseRotationOfCamera(float scale)
    {       

        Vector3 newLocalEulerAngle = new Vector3(scale * 3f, 0, 0);

        newLocalEulerAngle.x = Mathf.Clamp(newLocalEulerAngle.x, 15f, 30f);

        _cameraTransform.localEulerAngles = newLocalEulerAngle;
    }

    protected Vector3 CalculateScale(UInt32 point)
    {
        float cuberoot = (float)1 / 3;
        float tmp = Mathf.Pow(point, cuberoot);
        return new Vector3(tmp, tmp, tmp);
    }
    private void OnDestroy()
    {
        Debug.Log("lul");
        Player.IEatSomethingEvent -= this.PlayerScaleIncreased;
    }
}
