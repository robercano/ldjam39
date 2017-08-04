using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentManager : MonoBehaviour
{

    public enum State
    {
        Deciding,
        Harvesting,
        Recharging,
        Fighting,
        FindingOtherPlayer,
    }

    public State state;
    [Range(0.0f, 10.0f)]
    public float distanceToConsiderEnemyClose = 5.0f;
    [Range(0.0f, 2.0f)]
    public float minDecisionTime = 0.4f;
    [Range(0.0f, 2.0f)]
    public float maxDecisionTime = 0.8f;
    [Range(0.0f, 2.0f)]
    public float minAfterHarvestTime = 0.3f;
    [Range(0.0f, 2.0f)]
    public float maxAfterHarvestTime = 0.5f;
    [Range(0.0f, 2.0f)]
    public float minAfterRecollectTime = 0.3f;
    [Range(0.0f, 2.0f)]
    public float maxAfterRecollectTime = 0.5f;
    [Range(0.0f, 2.0f)]
    public float minAfterFightTime = 0.4f;
    [Range(0.0f, 2.0f)]
    public float maxAfterFightTime = 1.4f;
    [Range(0.0f, 2.0f)]
    public float minAfterFindPlayerTime = 0.2f;
    [Range(0.0f, 2.0f)]
    public float maxAfterFindPlayerTime = 0.4f;

    [Range(0.0f, 5.0f)]
    public float minShootingDeviation = 1.0f;
    [Range(0.0f, 5.0f)]
    public float maxShootingDeviation = 4.0f;

    private Vector3? target;
    private GameObject refOpposingPlayer;
    private Coroutine corroutineInProgress;
    private GameManager refGameManager;
    private NavMeshAgent refNavMeshAgent;
    private SceneBatteryManager refSceneBatteryManager;
    private PlayerBatteryManager refPlayerBatteryManager;
    private PlayerFistAttack refPlayerFistAttack;
    private Fortress refPlayerFortress;
    private PlayerHealth refPlayerHealth;
    private PlayerHealth refOpposingPlayerHealth;
    private Vector3 targetBatteryPosition;
    private Vector3 targetPlayerPosition;

    void Awake()
    {
        GameConfig.PlayerConfig playerConfig = GameConfig.Instance.GetPlayerConfig(gameObject.tag);
        if (playerConfig.controlType != GameConfig.ControlType.AI)
        {
            enabled = false;
        }
    }

    // Use this for initialization
    void Start()
    {
        GameObject levelObject = GameObject.Find("Level");
        this.refGameManager = levelObject.GetComponent<GameManager>();
        this.refOpposingPlayer = GameObject.Find((this.tag == "Blue") ? "RedPlayer" : "BluePlayer");
        this.refNavMeshAgent = this.GetComponent<NavMeshAgent>();
        this.refSceneBatteryManager = levelObject.GetComponent<SceneBatteryManager>();
        this.refPlayerFortress = GameObject.Find((this.tag == "Blue") ? "BlueBase" : "RedBase").GetComponent<Fortress>();
        this.refPlayerHealth = this.GetComponent<PlayerHealth>();
        this.refPlayerBatteryManager = this.GetComponent<PlayerBatteryManager>();
        this.refPlayerFistAttack = this.GetComponent<PlayerFistAttack>();
        this.refOpposingPlayerHealth = this.refOpposingPlayer.GetComponent<PlayerHealth>();

        this.refPlayerFistAttack.SetInputEnabled(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFistsPosition();
        if (ShouldStopCurrentRoutine())
        {
            StopCurrentRoutine();
        }
        else if (this.corroutineInProgress == null)
        {
            StartCorroutineDependingOnState();
        }
    }

    private void StopCurrentRoutine()
    {
        if (this.corroutineInProgress != null)
        {
            StopCoroutine(this.corroutineInProgress);
            this.corroutineInProgress = null;
        }
        this.GoTo(null);
        this.state = State.Deciding;
    }

    private bool ShouldStopCurrentRoutine()
    {
        return this.refGameManager.gameState != GameState.InGame ||
               this.refPlayerHealth.IsKnockedOut() ||
               ShouldRethink();
    }

    private void UpdateFistsPosition()
    {
        if (this.refGameManager.gameState == GameState.InGame)
        {
            Vector3 positionToLookAt = refOpposingPlayer.transform.position;
            switch (this.state)
            {

                case State.Harvesting:
                    if (targetBatteryPosition != Vector3.up)
                    {
                        positionToLookAt = targetBatteryPosition;
                    }
                    break;

                case State.Recharging:
                    positionToLookAt = this.refPlayerFortress.transform.position;
                    break;

            }
            this.refPlayerFistAttack.SetTarget(positionToLookAt);
        }
    }

    private bool ShouldRethink()
    {
        if (this.state == State.Deciding)
        {
            return false;
        }
        if (this.state != GetNextState())
        {
            return true;
        }
        if (ParamsForCurrentStateAreStillTheSame())
        {
            return false;
        }
        return true;
    }

    private void StartCorroutineDependingOnState()
    {
        switch (this.state)
        {
            case State.Deciding:
                this.corroutineInProgress = StartCoroutine(Deciding());
                break;

            case State.Harvesting:
                this.corroutineInProgress = StartCoroutine(Harvesting());
                break;

            case State.Recharging:
                this.corroutineInProgress = StartCoroutine(Recharging());
                break;

            case State.FindingOtherPlayer:
                this.corroutineInProgress = StartCoroutine(FindingOtherPlayer());
                break;

            case State.Fighting:
                this.corroutineInProgress = StartCoroutine(Fighting());
                break;
        }

    }

    public void GoTo(Vector3? target, float proximity = 0f)
    {
        if (!target.HasValue)
            this.refNavMeshAgent.enabled = false;
        else
        {
            this.refNavMeshAgent.enabled = true;
            this.refNavMeshAgent.stoppingDistance = proximity;
            this.refNavMeshAgent.SetDestination(target.Value);
        }
    }

    public void Finish()
    {
        this.GoTo(null);
        this.corroutineInProgress = null;
        this.state = State.Deciding;
    }

    IEnumerator Deciding()
    {
        this.state = GetNextState();
        StoreParamsForCurrentState();
        //Doubting delay
        yield return new WaitForSeconds(Random.Range(minDecisionTime, maxDecisionTime));

        this.corroutineInProgress = null;
    }

    IEnumerator Harvesting()
    {
        //Go to closest battery
        GameObject closestBattery = this.refSceneBatteryManager.ClosestBattery(this.transform.position);

        if (closestBattery)
        {
            this.GoTo(closestBattery.transform.position);

            while (!this.refPlayerBatteryManager.HasBattery())
            {
                yield return new WaitForSeconds(Random.Range(minAfterHarvestTime, minAfterHarvestTime));
            }
        }

        this.Finish();
    }

    IEnumerator Recharging()
    {
        //Bring your battery back to your fortress
        this.GoTo(this.refPlayerFortress.transform.position, 1f);

        while (this.refPlayerBatteryManager.HasBattery())
        {
            yield return new WaitForSeconds(Random.Range(minAfterRecollectTime, maxAfterRecollectTime));
        }

        this.Finish();
    }

    IEnumerator FindingOtherPlayer()
    {
        this.GoTo(this.refOpposingPlayer.transform.position, 1f);

        while (!EnemyIsClose())
        {
            yield return new WaitForSeconds(Random.Range(minAfterFindPlayerTime, maxAfterFindPlayerTime));
        }
    }


    IEnumerator Fighting()
    {
        while (!this.refOpposingPlayerHealth.IsKnockedOut())
        {
            if (this.refPlayerFistAttack.canShoot())
            {
                Vector3 randomInterference = new Vector3(
                    Random.Range(minShootingDeviation, maxShootingDeviation),
                    0,
                    Random.Range(minShootingDeviation, maxShootingDeviation)
                );
                Vector3 positionToShootAt = this.refOpposingPlayer.transform.position + randomInterference;
                this.refPlayerFistAttack.SetTarget(positionToShootAt);
                this.refPlayerFistAttack.Shoot();
            }

            yield return new WaitForSeconds(Random.Range(minAfterFightTime, maxAfterFightTime));
        }

        this.Finish();
    }

    private void StoreParamsForCurrentState()
    {
        targetBatteryPosition = Vector3.up; // this acts as null value
        targetPlayerPosition = Vector3.up;

        if (this.state == State.Harvesting)
        {
            GameObject closestBattery = this.refSceneBatteryManager.ClosestBattery(this.transform.position);
            if (closestBattery)
            {
                targetBatteryPosition = closestBattery.transform.position;
            }
        }

        if (this.state == State.FindingOtherPlayer)
        {
            targetPlayerPosition = this.refOpposingPlayer.transform.position;
        }
    }

    private bool ParamsForCurrentStateAreStillTheSame()
    {
        if (this.state == State.Harvesting)
        {
            GameObject closestBattery = this.refSceneBatteryManager.ClosestBattery(this.transform.position);
            if (!closestBattery || (targetBatteryPosition != closestBattery.transform.position))
            {
                return false;
            }
        }
        if (this.state == State.FindingOtherPlayer)
        {
            if (Vector3.Distance(targetPlayerPosition, this.refOpposingPlayer.transform.position) > 5.0f)
            {
                return false;
            }
        }
        return true;
    }

    private State GetNextState()
    {
        bool isCarryingBattery = this.refPlayerBatteryManager.HasBattery();
        bool myFortressIsLowOnBattery = (this.refPlayerFortress.GetPercentage() < 0.75f);
        bool thereAreBatteries = !!this.refSceneBatteryManager.ClosestBattery(this.transform.position);
        bool enemyIsClose = EnemyIsClose();
        bool enemyIsKnockedOut = this.refOpposingPlayerHealth.IsKnockedOut();

        if (isCarryingBattery)
        {
            return State.Recharging;
        }
        if (myFortressIsLowOnBattery && thereAreBatteries)
        {
            return State.Harvesting;
        }
        if (enemyIsClose && !enemyIsKnockedOut)
        {
            return State.Fighting;
        }
        if (!enemyIsKnockedOut)
        {
            return State.FindingOtherPlayer;
        }
        if (thereAreBatteries)
        {
            return State.Harvesting;
        }
        return State.FindingOtherPlayer;
    }

    private bool EnemyIsClose()
    {
        return Vector3.Distance(this.transform.position, this.refOpposingPlayer.transform.position) < distanceToConsiderEnemyClose;
    }
}
