using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EatType { ET_SMALL, ET_LARGE, ET_MISSILE,ET_COIN, ET_NONE};

public class Character : MonoBehaviour
{    
    [SerializeField]
    protected UInt32 _currentPoint;
    [Header("Missile")]
    [SerializeField]
    private Transform _missileLocation;
    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    public bool _hasMissile = false;
    [Header("Food")]
    [SerializeField]
    protected float _scaleFactor;
    private CameraFogClip _fogClip;
    private GameObject _pickupParent;

    
    protected float _eatRatio = 1.33f;
    protected GameObject obj;
    
    protected UInt32 startPoint = 1;
    protected UInt32 endPoint = 1;
    protected const UInt32 foodPoint = 5;
    protected UInt32 _othersPoint = 1;
    [SerializeField]
    protected const float eatingAnimationSpeed = 1.0f;
    protected float startTime;

    public static event Action<string, UInt32, EatType> IEatSomethingEvent;

    protected void ShootMissile()
    {
        if (!_hasMissile) return;
        GameObject obj = Instantiate(_prefab, _missileLocation.position, Quaternion.identity, _missileLocation);
        obj.GetComponent<Fuze>().SetMovement(this.transform);      
        _hasMissile = false;
        this.transform.Find("Pickup_Parent").Find("Fuze_Pickup").gameObject.SetActive(false);
    }
    /*
    public void UpdateScaleAnimation(UInt32 point)
    {
        startTime = Time.time;
        startPoint = GetCurrentPoint();
        endPoint = endPoint + point;
    }
    
    protected void EatAnimation()
    {
        if (endPoint == GetCurrentPoint()) return;

        float yuzdeDegeri = (Time.time - startTime) * eatingAnimationSpeed;
        
        transform.localScale = Vector3.Lerp(startScale, endScale, yuzdeDegeri);
        //Debug.Log("distCovered: " + distCovered);
        //Debug.Log("yuzdeDegeri: " + yuzdeDegeri * 100 +" Time = " + (Time.time - startTime));
    }

    protected void UpdateScoreboard()
    {
        _size = System.Convert.ToUInt64((Mathf.Pow(endScale.x, 3)) * 10);
        IEatSomethingEvent.Invoke(name, _size); //_scoreboard.SetScore(name, _size);
    }
    */

    
    public UInt32 GetCurrentPoint()
    {
        return _currentPoint;
    }

    protected void SetScale(UInt32 point)
    {
        this.transform.localScale = CalculateScale(point);
    }

    protected Vector3 CalculateScale(UInt32 point)
    {
        float cuberoot = (float)1 / 3;
        float tmp = Mathf.Pow(point, cuberoot);
        return new Vector3 ( tmp, tmp, tmp );
    }
    
    public void EatSomething(UInt32 otherPoint = foodPoint)
    {
        EatType eatType = otherPoint > 40 ? EatType.ET_LARGE : EatType.ET_SMALL; // TODO boyutlar degistirilebilir, kendi boyutuna yakinsa vs diye.
        _currentPoint = GetCurrentPoint() + otherPoint;
        SetScale(GetCurrentPoint());
        //_fogClip = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFogClip>();
        //_fogClip.PlayerScaleIncreased(name, GetCurrentPoint() + 10, eatType);
        IEatSomethingEvent?.Invoke(name, GetCurrentPoint(), eatType);
    }

    public void HitMissile()
    {
        if (_currentPoint >= 20) /*20 puandan kucukken fuze ile vurulsa bile kuculmesin*/
        {
            _currentPoint = GetCurrentPoint() / 4;
        }
        SetScale(GetCurrentPoint());
        IEatSomethingEvent?.Invoke(name, GetCurrentPoint(), EatType.ET_MISSILE);
    }
    public void CoinAlindi()
    {
        IEatSomethingEvent?.Invoke(name, GetCurrentPoint(), EatType.ET_COIN);
    }
}
