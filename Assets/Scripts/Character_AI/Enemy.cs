
using Boo.Lang.Environments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    private enum State
    {
        IDLE,
        SEARCHING,
        CHASE_ENEMY,
        RUN_ENEMY,
        SHOOT_MISSILE,
        CHASE_PICKUP,
        CHASE_FOOD

    }

    private State? _enemyState = null;
    private int _oldEnemyState = (int)State.IDLE;

    [Header("Score")] [SerializeField] private Scoreboard _scoreboard;
    [SerializeField] private float _pullingRadius;
    [Header("Pull Foods")] [SerializeField] private bool _foodPullingActive = false;
    [SerializeField] private SphereCollider _pullingPower;
    [SerializeField] private Texture[] _textures;
    [SerializeField] private Material _defaultMaterial;


    private NavMeshAgent _agent;
    private MeshRenderer _enemyRenderer;
    private Rigidbody rigid;
    public static bool isPlayerDead = false;

    private GameObject _nameObj;
    private TMPro.TextMeshPro _nameText;

    public static event Action<string> IamDeadEvent;
    public static event Action<string, UInt32> IamSpawnedEvent;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _enemyRenderer = GetComponent<MeshRenderer>();
        _agent = GetComponent<NavMeshAgent>();
        _scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<Scoreboard>();
        _nameText = GetComponentInChildren<TMPro.TextMeshPro>();
        rigid = GetComponent<Rigidbody>();
        
    }
    private void Start()
    {
        SetObjectTextureAndName();
        _currentPoint = 1;
        IamSpawnedEvent?.Invoke(name, GetCurrentPoint());
        int frameLater = Random.Range(0, 100);
        //Debug.Log(name + " = frame later is - " + frameLater);
        Invoke("StartAI", 4 + (frameLater) * 0.01f);
    }
    private void SetObjectTextureAndName()
    {
        _nameText.SetText(this.name);
        Texture texture = _textures[UnityEngine.Random.Range(0, _textures.Length)];
        Material mat = new Material(_defaultMaterial);
        mat.SetTexture("_BaseMap", texture);
        _enemyRenderer.material = mat;
    }
    private void FixedUpdate()
    {
        if(transform.localScale.x >= 3)
        {
            _agent.speed = 10f / (transform.localScale.x * 0.33f);
            //Debug.Log(_agent.velocity.magnitude);
        } else
        {
            _agent.speed = 10f;
            //Debug.Log(_agent.velocity.magnitude);
        }
        
        if(_enemyState != null)
        {
            EnemyAI();
            //_nameText.SetText(_enemyState.ToString());
        }
        _pullingPower.radius = _foodPullingActive ? _pullingRadius : 0f;
        _currentPoint = GetCurrentPoint();
              
        if (!rigid.isKinematic)
        {
            if (!IsInvoking("CheckToStop"))
            {
                InvokeRepeating("CheckToStop", 2f, 2f);
            }
        }
    }

    private void CheckToStop()
    {
        float suankispeed = rigid.velocity.magnitude;

        if (suankispeed > 9f)
        {
            return;
        }
  
        this.GetComponent<Rigidbody>().isKinematic = true;
        _agent.enabled = true;
        CancelInvoke("CheckToStop");
    }

   
    private void OnDestroy()
    {
        IamDeadEvent?.Invoke(gameObject.name); // TODO neden null veriyor.
    }
  

    //AI stuff

    private void EnemyAI()
    {
        if (_enemyState != (State)_oldEnemyState)
        {
            //Debug.Log("State Seçiyorum.");
            switch (_enemyState)
            {

                case State.IDLE:
                    _oldEnemyState = (int)_enemyState;
                    bool AFKchance = UnityEngine.Random.Range(0, 101) < 1;
                    if (AFKchance) StartCoroutine(BeAFK());
                    else StartCoroutine(WalkRandomly());
                    break;
                case State.SEARCHING:
                    _oldEnemyState = (int)_enemyState;
                    StartCoroutine(Searching());
                    break;
                case State.CHASE_ENEMY:
                    _oldEnemyState = (int)_enemyState;
                    StartCoroutine(ChaseEnemy());
                    break;
                case State.RUN_ENEMY:
                    _oldEnemyState = (int)_enemyState;
                    StartCoroutine(RunAwayFromEnemy());
                    break;
                case State.SHOOT_MISSILE:
                    _oldEnemyState = (int)_enemyState;
                    StartCoroutine(FireMissile());
                    break;
                case State.CHASE_PICKUP:
                    _oldEnemyState = (int)_enemyState;
                    StartCoroutine(ChasePickup());
                    break;
                case State.CHASE_FOOD:
                    _oldEnemyState = (int)_enemyState;
                    StartCoroutine(ChaseFood());
                    break;
            }
        }
    }

    private IEnumerator Searching()
    {
        if (_hasMissile)
        {
            if (_agent.enabled)
            {
                //Debug.Log("Shoot Missile");
                _enemyState = State.SHOOT_MISSILE;
                yield break;
            }
        }
        var nearestFood = FindClosestFood.FindClosest(transform.position,this.name);
        var nearestMissile = FindClosestMissile.FindClosest(transform.position);
        var nearestEnemy = FindClosestEnemy.FindClosest(transform, 2);
        //check enemy size

        //float enemySize = nearestEnemy.Values.First().gameObject.tag == "Enemy" ?
        //                  nearestEnemy.Values.First().GetComponent<Enemy>().GetCurrentPoint() :
        //                  nearestEnemy.Values.First().GetComponent<Player>().GetCurrentPoint();
        float enemySize = nearestEnemy.Values.First().GetComponent<Character>().GetCurrentPoint();
      
        //Debug.Log(enemySize + " ---- " + "My name is = " + name + " and enemy name is = " + nearestEnemy.Values.First());
        //check distances
        float distFood = Vector3.Distance(transform.position, nearestFood.transform.position);
        float distMissile;
        if(nearestMissile == null)
        {
            distMissile = Mathf.Infinity;
        } else
        {
            distMissile = Vector3.Distance(transform.position, nearestMissile.transform.position);
        }
        float distEnemy = Vector3.Distance(transform.position, nearestEnemy.Values.First().transform.position);

        float[] dists = new float[] { distEnemy, distMissile, distFood };
        switch (Array.IndexOf(dists, dists.Min()))
        {
            case 0:
                if (GetCurrentPoint() > enemySize * _eatRatio)
                {
                    //Debug.Log("Enemy - Chase State");
                    _enemyState = State.CHASE_ENEMY;
                }
                else if (enemySize > GetCurrentPoint() * _eatRatio)
                {
                    //Debug.Log("Enemy - Run State");
                    _enemyState = State.RUN_ENEMY;
                }
                else
                {
                    //Debug.Log("Search State");
                    _enemyState = State.CHASE_FOOD;
                }
                yield break;
            case 1:
                //Debug.Log("Pickup - Chase State");
                _enemyState = State.CHASE_PICKUP;
                yield break;
            case 2:
                //Debug.Log("Food - Chase State");
                _enemyState = State.CHASE_FOOD;
                yield break;
            default:
                //Debug.Log("Search State");
                _enemyState = State.SEARCHING;
                yield break;
        }
    }
    private IEnumerator WalkRandomly()
    {
        int walkCount = UnityEngine.Random.Range(1, 3);
        //Debug.Log(walkCount + " kadar tekrar edecek.");
        for (int i = 0; i < walkCount; i++)
        {
            Vector3 locationToWalk = transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f),
                                                                  0f,
                                                                  UnityEngine.Random.Range(-10f, 10f));
            //Debug.Log("Gidecegim yer = " + locationToWalk);
            //Debug.Log((i + 1) + "." + "Yürüme");
            _agent.SetDestination(locationToWalk);
            // Check if we've reached the destination
            while (true)
            {
                //Debug.Log("yürüme devam ediyor.");
                if (!_agent.pathPending)
                {
                    //Debug.Log(_agent.remainingDistance + " - Walking -  " + _agent.stoppingDistance);
                    if ( _agent.remainingDistance <= _agent.stoppingDistance)
                    {
                        if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                        {
                            // Done
                            //Debug.Log("Yürüme bitti.");
                            yield return new WaitForSeconds(UnityEngine.Random.Range(0, 1));
                            //Debug.Log("Bekleme bitti.");
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForEndOfFrame();
            }
        }
        //go back to searching.
        //Debug.Log("Search State");
        _enemyState = State.SEARCHING;
    }
    private IEnumerator BeAFK()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(5f,20f));
        //back to searching.
        //Debug.Log("Search State");
        _enemyState = State.SEARCHING;
    }
    private IEnumerator FireMissile()
    {
        var nearestEnemies = FindClosestEnemy.FindClosest(transform, _scoreboard.GetEnemyCount()); //?? null; why get enemy count?
        if (nearestEnemies.Count != 0)
        {
            string[] names = new string[nearestEnemies.Count];
            for (int i = 0; i < names.Length; ++i)
            {
                names[i] = nearestEnemies.ElementAt(i).Value.name;
            }
            string enemyNameToThrowMissile = _scoreboard.CheckIfSuitableForThrowMissileOrDefault(names, gameObject.name); // enemyNameToThrowMissile değeri füze atılacak kişinin ismi
            //füze atılacak kişiye dönme olayı burda
            Vector3 lookAt;
            
            //if its player 
            if (enemyNameToThrowMissile == PlayerData.Instance.GetPlayerUserName())
            {
                lookAt = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
            } else
            {
                //if the enemy is enemy 
                lookAt = transform.parent.Find(enemyNameToThrowMissile).transform.position - transform.position;
            }
            
            lookAt.y = transform.position.y;
            var rotation = Quaternion.LookRotation(lookAt);
            transform.rotation = rotation;
            //yield return new WaitForSeconds(Random.Range(0.5f,1f)); //TODO : waitforseconds
            base.ShootMissile();

            _hasMissile = false;
        }
        bool idleChance = Random.Range(0, 101) < 1;
        if (idleChance)
        {
            //Debug.Log("State IDLE");
            _enemyState = State.IDLE; //%30 chance to go idle if not go back to searching.
        }
        else
        {
            //Debug.Log("State Search");
            _enemyState = State.SEARCHING;
        }
        yield break;
    }
    private IEnumerator ChasePickup()
    {
        var nearestMissile = FindClosestMissile.FindClosest(transform.position);
        Transform pickup = nearestMissile.transform;
        while (true)
        {
            if (pickup == null || _hasMissile)
            {
                _enemyState = State.SEARCHING;
                yield break;
            }
            _agent.SetDestination(pickup.position);
            while (true)
            {
                //Debug.Log("missile yemeying devam ediyor.");
                if (pickup == null || _hasMissile)
                {
                    _enemyState = State.SEARCHING;
                    yield break;
                }
                if (Time.frameCount % 60 == 0)
                {
                    //Debug.Log(Time.frameCount);
                    //yakın enemye bak , onun distance i food dan daha yakınsa büyüklüğüne göre chase yada run. 
                    var nearestEnemy = FindClosestEnemy.FindClosest(transform, 2);
                    var nearestEnemyDist = nearestEnemy.Keys.First();
                    var nearestPickupDist = (this.transform.position - pickup.position).sqrMagnitude;
                    //Debug.Log("My name = " + this.name + " - " + "Enemy = " + nearestEnemyDist + " " + nearestEnemy.Values.First().name + " -- Food =  " + nearestPickupDist);
                    if (nearestEnemyDist < nearestPickupDist)
                    {
                        //Debug.Log("enemy detected");
                        float enemyPoint = nearestEnemy.Values.First().GetComponent<Character>().GetCurrentPoint();
                        float myPoint = GetCurrentPoint();
                        if (enemyPoint * _eatRatio < myPoint)
                        {
                            //Debug.Log("chase enemy");
                            _enemyState = State.CHASE_ENEMY;
                            yield break;
                        }
                        else if (enemyPoint > _eatRatio * myPoint)
                        {
                            //Debug.Log("run from enemy");
                            _enemyState = State.RUN_ENEMY;
                            yield break;
                        }
                    }
                    _agent.SetDestination(pickup.position);
                }
                yield return new WaitForEndOfFrame();
            }
        }

    }
    private IEnumerator ChaseEnemy()
    {
        //while chasing him check his size , if size becomes unedible run away little bit than back to searching.
        var nearestEnemy = FindClosestEnemy.FindClosest(transform, 2);
        Character enemy = nearestEnemy.Values.First().GetComponent<Character>();
        string enemyTag = enemy.tag;
        while (true)
        {
            
            //check his size 
            if (enemyTag == "Enemy")
            {
                if (enemy == null)
                {
                    //enemy dead.
                    _enemyState = State.SEARCHING;
                    yield break;
                }
            } else if(enemyTag == "Player")
            {
                if (Enemy.isPlayerDead)
                {
                    //player dead.
                    _enemyState = State.SEARCHING;
                    yield break;
                }
            }
            
            bool Chase = enemy.GetCurrentPoint() * _eatRatio < GetCurrentPoint();
            //continue chasing.
            if (!Chase)  //if enemy got bigger suddenly. run away 90% or idle,searching %10. 
            {
                //Debug.Log("Enemy got bigger than me try to run away.");
                bool RunOrNot = UnityEngine.Random.Range(0, 101) < 99;
                if (RunOrNot)
                {
                    //Debug.Log("Decided to run away. %90");
                    _enemyState = State.RUN_ENEMY;
                }
                else
                {
                    //Debug.Log("Enemy got bigger but I didnt see it whatever dont run away. %10");
                    bool SearchingOrNot = UnityEngine.Random.Range(0, 101) < 99;
                    if (SearchingOrNot)
                    {
                        //Debug.Log("Searching confirmed. %90");
                        _enemyState = State.SEARCHING;
                    }
                    else
                    {
                        //Debug.Log("IDLE confirmed. %10 , Walk Randomly");
                        //TODO: GO IDLE , or Walk Away
                        _enemyState = State.IDLE;
                    }
                }
                yield break;
            }
            _agent.SetDestination(enemy.transform.position);
            //check distance if distance got insane go back to searching or idle?
            float distBetween = Vector3.Distance(this.transform.position, enemy.transform.position);
            //Debug.Log("Distance = " + distBetween);
            if (distBetween > 50) // go searching or idle ( 100 is changeable)
            {
                //Debug.Log("Distance > 50");
                bool SearchingOrNot = UnityEngine.Random.Range(0, 101) < 99;
                if (SearchingOrNot)
                {
                    //Debug.Log("Searching confirmed. %90");
                    _enemyState = State.SEARCHING;
                }
                else
                {
                    //Debug.Log("IDLE confirmed. %10 , Walk Randomly");
                    _enemyState = State.IDLE;
                }
                yield break;
            }

            bool Bored = UnityEngine.Random.Range(0, 101) < 1;
            if (Bored) // go back to idle or searching. maybe only food or pickup 
            {
                //Debug.Log("Bored. IDLE");
                _enemyState = State.IDLE;
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator RunAwayFromEnemy()
    {
        var nearestEnemy = FindClosestEnemy.FindClosest(transform, 2);
        Character enemy = nearestEnemy.Values.First().GetComponent<Character>();
        //runing away check his size if he gets smaller go back to searching or idle ?
        while (true)
        {
            if (enemy == null)
            {
                //enemy dead.
                _enemyState = State.SEARCHING;
                yield break;
            }
            //stop running based on size
            bool StopRunning = GetCurrentPoint() * _eatRatio > enemy.GetCurrentPoint() ;
            if (StopRunning)
            {
                //go searching or idle ?
                bool SearchingOrNot = Random.Range(0, 101) < 98;
                if (SearchingOrNot)
                {
                    _enemyState = State.SEARCHING;
                }
                else
                {
                    _enemyState = State.IDLE;
                }
                yield break;
            }
            Vector3 destination = this.transform.position * 2 - enemy.transform.position;
            _agent.SetDestination(destination);
            //stop running based on distance
            float distBetween = Vector3.Distance(this.transform.position, enemy.transform.position);
            //Debug.Log("Distance = " + distBetween);
            if (distBetween > 50)
            {
                //Debug.Log("Distance > 50");
                bool SearchingOrNot = UnityEngine.Random.Range(0, 101) < 99;
                if (SearchingOrNot)
                {
                    //Debug.Log("Searching confirmed. %90");
                    _enemyState = State.SEARCHING;
                }
                else
                {
                    //Debug.Log("IDLE confirmed. %10 , Walk Randomly");
                    _enemyState = State.IDLE;
                }
                yield break;
            }
            bool Bored = UnityEngine.Random.Range(0, 101) < 1;
            if (Bored) // go back to idle or searching. maybe only food or pickup 
            {
                //Debug.Log("Bored. IDLE");
                _enemyState = State.IDLE;
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator ChaseFood()
    {
        var nearestFood = FindClosestFood.FindClosest(transform.position,this.name);
        Transform food = nearestFood.transform;
        bool isDefaultFood = food.GetComponent<Food>().Name == "default";
        while (true)
        {
            if (food == null)
            {
                _enemyState = State.SEARCHING;
                yield break;
            }
            _agent.SetDestination(food.position);
            while (true)
            {
                //Debug.Log("Yeme devam ediyor.");
                if(food == null)
                {
                    _enemyState = State.SEARCHING;
                    yield break;
                }
                if (Time.frameCount % 60 ==  0)
                {
                    //Debug.Log(Time.frameCount);
                    //yakın enemye bak , onun distance i food dan daha yakınsa büyüklüğüne göre chase yada run. 
                    var nearestEnemy = FindClosestEnemy.FindClosest(transform, 2);
                    var nearestEnemyDist = nearestEnemy.Keys.First();
                    var nearestFoodDist = (this.transform.position - food.position).sqrMagnitude;
                    //Debug.Log("My name = " + this.name + " - " + "Enemy = " + nearestEnemyDist + " " + nearestEnemy.Values.First().name + " -- Food =  " + nearestFoodDist);
                    if (nearestEnemyDist < nearestFoodDist)
                    {
                        //Debug.Log("enemy detected");
                        float enemyPoint = nearestEnemy.Values.First().GetComponent<Character>().GetCurrentPoint();
                        float myPoint = GetCurrentPoint();
                        if (enemyPoint * _eatRatio < myPoint)
                        {
                            //Debug.Log("chase enemy");
                            _enemyState = State.CHASE_ENEMY;
                            yield break;
                        }
                        else if (enemyPoint > _eatRatio * myPoint)
                        {
                            //Debug.Log("run from enemy");
                            _enemyState = State.RUN_ENEMY;
                            yield break;
                        }
                        
                    }
                    _agent.SetDestination(food.position);
                }
                

                yield return new WaitForEndOfFrame();
            }
        }

    }
    private void StartAI()
    {
        _enemyState = State.SEARCHING;
    }
}


