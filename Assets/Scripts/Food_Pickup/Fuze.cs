using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Fuze : MonoBehaviour
{
    // TODO other foodlar objectcontainerın altinda olusmuyorlar

    public GameObject _foodPrefab;
    private GameObject _missileContainer;
    [SerializeField] private float _missileSpeed = 10f;
    [SerializeField] private VisualEffectAsset foodvfx;
    private Transform _charTransform;
    private Rigidbody _rb;
    private Collider other;

    private Gradient _gradient = new Gradient();
    private GradientColorKey[] _colorKeys = new GradientColorKey[1];
    private GradientAlphaKey[] _alphaKeys = new GradientAlphaKey[3];
    private Texture _ballTexture;

    private string _owner;
    private const UInt32 _maxFoodPointValue = 50;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _missileContainer = GameObject.FindGameObjectWithTag("MissileContainer");
        for (int i = 0; i < _colorKeys.Length; i++)
        {
            _colorKeys[i] = new GradientColorKey();
        }
        for (int i = 0; i<_alphaKeys.Length;i++)
        {
            _alphaKeys[i] = new GradientAlphaKey();
        }
        _alphaKeys[0].alpha = 0.2f;
        _alphaKeys[0].time = 0.0f;
        _alphaKeys[1].alpha = 1f;
        _alphaKeys[1].time = 0.5f;
        _alphaKeys[2].alpha = 0.2f;
        _alphaKeys[2].time = 1.0f;
        
    }
    private void FixedUpdate()
    {
        _rb.velocity = transform.forward * _missileSpeed * Time.deltaTime;
        if(triggerEnter)
        {
            string otherTag = other.tag;
            string hitObjectName = "";
            UInt32 objectPoint = 0;
            if (_owner == other.transform.name) return;
            /*Everybody hits everybody*/
            if (otherTag == "Enemy" || otherTag == "Player")
            {
                if (otherTag == "Enemy")
                {
                    Enemy enemy = other.gameObject.GetComponent<Enemy>();
                    Rigidbody rb = other.GetComponent<Rigidbody>();
                    NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
                    enemy.HitMissile();
                    hitObjectName = other.transform.name;
                    objectPoint = enemy.GetCurrentPoint();
                    //agent.enabled = false;
                    //rb.isKinematic = false;
                    rb.AddForce((this.transform.position - other.transform.position).normalized * -10000f);
                    // enemy.ChangeAgentBack();

                }
                else if (otherTag == "Player")
                {
                    Player player = other.GetComponent<Player>() ?? null;
                    if (player != null)
                    {
                        Rigidbody rb = player.GetComponent<Rigidbody>();
                        player.HitMissile();
                        hitObjectName = PlayerData.Instance.GetPlayerUserName();
                        objectPoint = player.GetCurrentPoint();
                        rb.AddForce((this.transform.position - other.transform.position).normalized * -10000f);
                    }

                }
                else
                {
                    //Debug.Log("What");
                }

                /*Calculate total food point*/
                UInt32 totalFoodPoint = objectPoint / 2;
                /*Generate random foods*/
                List<UInt32> randomFoodPoints = CreateRandomFoods(totalFoodPoint * 3);
                /*check local scale of other game object. the amount of food that will spawn determines that.*/
                for (int i = 0; i < randomFoodPoints.Count; i++)
                {
                    GameObject obj = Instantiate(_foodPrefab,
                        other.transform.position + new Vector3(0f, (other.transform.localScale.y + CalculateScale(randomFoodPoints[i]).y) / 2, 0f),
                        Quaternion.identity);
                    if (_owner == PlayerData.Instance.GetPlayerUserName())
                    {

                        //Debug.Log("----");
                        //Debug.Log(other.name);
                        //Debug.Log(other.transform.position);
                        //Debug.Log(other.transform.localScale);
                        //Debug.Log(CalculateScale(randomFoodPoints[i]));
                        //Debug.Log("----");
                    }
                    obj.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-5f, 5f), Random.Range(0, 10f), Random.Range(-5f, 5f)) * 100f);
                    obj.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-5f, 5f), Random.Range(0, 10f), Random.Range(-5f, 5f)) * 100f);
                    obj.GetComponent<Food>().Name = other.gameObject.name;
                    obj.name = other.gameObject.name;
                    VisualEffect vfx = obj.AddComponent<VisualEffect>();
                    vfx.visualEffectAsset = foodvfx;                    
                    vfx.SetInt("SpawnCount", Convert.ToInt16(randomFoodPoints[i]));
                    MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();
                    objRenderer.material = other.GetComponent<MeshRenderer>().material;
                    objRenderer.material.SetColor("_EmissionColor", new Color(objRenderer.material.color.r, objRenderer.material.color.g, objRenderer.material.color.b, 1));
                    //objRenderer.material.EnableKeyword("_EMISSION");
                    Color gradientColor = new Color(objRenderer.material.color.r, objRenderer.material.color.g, objRenderer.material.color.b, objRenderer.material.color.a * 10);
                    vfx.SetTexture("BallTexture", objRenderer.material.mainTexture);
                    _colorKeys[0].color = gradientColor;
                    _gradient.SetKeys(_colorKeys, _alphaKeys);
                    vfx.SetGradient("Gradient", _gradient);
                    vfx.GetGradient("Gradient").mode = GradientMode.Blend;
                    obj.GetComponent<Food>().SetCurrentPoint(randomFoodPoints[i]);
                    obj.transform.localScale = CalculateScale(randomFoodPoints[i]);
                    vfx.SetFloat("Radius", obj.transform.localScale.x * 0.5f);
                    obj.transform.parent = GameObject.Find("MissileFood").transform;
                }

                Destroy(gameObject);

                /*Create text*/
                MessageBox.Instance.CreateText(_owner, hitObjectName, MessageBox.Action.Missiled);
            }

            if (other.name == "Wall")
            {
                Destroy(this.gameObject, 2f);
            }
        }

    }
    bool triggerEnter;
    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.tag;
        if ((otherTag == "Enemy" || otherTag == "Player" || other.name == "Wall") && other.isTrigger == false)
        {
            triggerEnter = true;
            this.other = other;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        string otherTag = other.tag;
        if ((otherTag == "Enemy" || otherTag == "Player" || other.name == "Wall") && other.isTrigger == false)
        {
            triggerEnter = false;
            this.other = other;
        }
    }
    private List<UInt32> CreateRandomFoods(UInt32 totalFoodPoint)
    {
        // food point shall be between 5 - _maxFoodPointValue
        List<UInt32> randomFoods = new List<UInt32>();
        if(totalFoodPoint <= _maxFoodPointValue)
        {
            for(int i = 0; i<(int) (totalFoodPoint / 5) + 1; i++)
            {
                randomFoods.Add(5);
            }
            return randomFoods;
        }

        UInt32 randomFoodPoint = 0;
        
        do
        {
            randomFoodPoint = Convert.ToUInt32(Random.Range(5, _maxFoodPointValue + 1));
            if(totalFoodPoint >= randomFoodPoint)
            {
                totalFoodPoint -= randomFoodPoint;
                randomFoods.Add(randomFoodPoint);
            }
            else
            {
                if(totalFoodPoint <= 5)
                {
                    randomFoods.Add(5);
                }
                else
                {
                    randomFoods.Add(totalFoodPoint);
                }
                totalFoodPoint = 0;
            }
        } while (totalFoodPoint > 0);

        return randomFoods;
    }
    private Vector3 CalculateScale(UInt32 point)
    {
        float cuberoot = (float)1 / 3;
        float tmp = Mathf.Pow(point, cuberoot);
        return new Vector3(tmp, tmp, tmp);
    }

    public void SetMovement(Transform character)
    {
        _charTransform = character;
        _owner = character.name;
        this.transform.parent = _missileContainer.transform;
        this.transform.forward = character.forward;      
    }
}
