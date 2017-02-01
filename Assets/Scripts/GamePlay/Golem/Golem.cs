using UnityEngine;
using System.Collections;

public abstract class Golem : Unit
{
    #region Data Members
    
   // public Transform target;

	private Rigidbody physicBody;

    public delegate void DefendTrigger(bool val);
    public delegate void StunTrigger(bool val);
    public delegate void MoveTrigger(bool val);
    public event DefendTrigger OnDefend;
    public event StunTrigger OnStunned;
    public event MoveTrigger OnMove;
    public string moveAnimationState;
    public string defendAnimationState;
    public string stunAnimationState;

    private Animator golemAnim;

    [SerializeField]
    private int hitCount;
    [SerializeField]
    private int maxHitThreshold;
    [SerializeField]
    private float defendAnimationTime;
    [SerializeField]
    private float stunAnimationTime;
    [SerializeField]
    private bool isDefending;
    [SerializeField]
    private bool isStunned;
    [SerializeField]
    private float defaultMoveRange;

    #endregion

    #region Setters & Getters

    public int Golem_Hit_Count
    {
        get { return hitCount; }
        set { hitCount = value; }
    }

    public int Golem_Hit_Threshold
    {
        get { return maxHitThreshold; }
        set { maxHitThreshold = value; }
    }

    public float Golem_Defend_Duration
    {
        get { return defendAnimationTime; }
        set { defendAnimationTime = value; }
    }

    public float Golem_Stun_Duration
    {
        get { return stunAnimationTime; }
        set { stunAnimationTime = value; }
    }

    public float Golem_Default_Move_Range
    {
        get { return defaultMoveRange; }
        set { defaultMoveRange = value; }
    }

    public float Golem_Sight_Range
    {
        get { return base.Unit_Sight_Range; }
		set { Unit_Sight_Range = value; }
    }

    public bool Golem_Defend_Duration_Done
    {
        get { return isDefending; }
        set { isDefending = value; }
    }

	public int Golem_MaxHealth
	{
		get { return base.Unit_Max_HP; }
		set { base.Unit_Max_HP = value;
			base.Unit_Current_HP = value;
		}
	}

	public int Golem_CurrentHealth
	{
		get { return base.Unit_Current_HP; }
		set { base.Unit_Current_HP = value; }
	}

	public float Golem_MoveSpeed
	{
		get { return base.Unit_Move_Speed; }
		set { base.Unit_Move_Speed = value;}
	}
	
    public string Get_Golem_Name
    {
        get { return base.Unit_Name; }
    }

    public bool Movement_Done
    {
        get { return base.Unit_Is_Moving; }
    }

    #endregion

    #region Built-in Unity Methods

    // Use this for initialization
	protected virtual void Start () 
    {
		physicBody = gameObject.GetComponent<Rigidbody>();
        base.Unit_Is_Moving = false;
        isDefending = false;
        golemAnim = gameObject.GetComponent<Animator>();
	}

    protected virtual void Update()
    {
        //Debug.Log(golemAnim);
    }
	
    #endregion

    #region Main Methods

    public void Move(float direction)
    {
        golemAnim.SetBool(moveAnimationState, true);
        MoveThisUnit(direction);
    }

    public void Sprint(float direction)
    {
        SprintUnit(direction);
    }

    public void SetMovingAnimation(bool val)
    {
        //Might change this later.
        golemAnim.SetBool(moveAnimationState, val);
    }

    public void Defend()
    {
        if(!isDefending)
        {
            golemAnim.SetBool(defendAnimationState, true);
            isDefending = true;

            GolemDefendPosition();
        }
    }

    public void Stun()
    {
        GolemStunnedPosition();
    }

    public void EndDefend()
    {
        //Set the animation state back to its default animation.
        golemAnim.SetBool(defendAnimationState, false);
        isDefending = false;
        EventManager.TriggerEvent("EndState");
    }

    public void OnHit(int damageValue)
    {
        base.Unit_Current_HP -= damageValue;
		if (base.Unit_Current_HP <= 0) {
			CombatSceneManager.instance.RemoveGolemFromList (gameObject);
		}
        hitCount += damageValue;

        if(hitCount >= maxHitThreshold)
        {

        }
    }

    /// <summary>
    /// Overridden method that implements movement 
    /// for Golem objects.
    /// </summary>
    /// <param name="direction"></param>
    protected override void MoveThisUnit(float direction)
    {
        //Move speed is determined by this unit's Move Speed stat
		physicBody.velocity = new Vector3(direction*base.Unit_Move_Speed,physicBody.velocity.y,physicBody.velocity.z);
    }

    void SprintUnit(float direction)
    {
        //Move speed is determined by this unit's Move Speed stat
		physicBody.velocity = new Vector3(direction*base.Unit_Move_Speed*2,physicBody.velocity.y,physicBody.velocity.z);
    }

    protected virtual void GolemDefendPosition()
    {
        StartCoroutine(GolemDefend());
    }

    public virtual void GolemStunnedPosition()
    {
        StartCoroutine(GolemStunned());
    }

    #endregion

    #region Helper Methods
   
    /// <summary>
    /// This helper function is a couroutine that will 
    /// deal with Golem movement. Will move the Golem
    /// to the +x position based on which conditions were
    /// satisfied in the MoveThisUnit() method.
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    /// <returns></returns>
    IEnumerator MoveRight(Vector3 fromPosition, Vector3 toPosition)
    {
        while(gameObject.transform.position.x < toPosition.x)
        {
            //gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, toPosition, Time.deltaTime * base.Unit_Move_Speed);

            if(gameObject.transform.position.x >= toPosition.x)
            {
                base.Unit_Is_Moving = false;
                golemAnim.SetBool(moveAnimationState, false);
                OnMove(base.Unit_Is_Moving);
            }

            yield return null;
        }
    }

    /// <summary>
    /// This helper function is a couroutine that will 
    /// deal with Golem movement. Will move the Golem
    /// to the -x position based on which conditions were
    /// satisfied in the MoveThisUnit() method.
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <param name="toPosition"></param>
    /// <returns></returns>
    IEnumerator MoveLeft(Vector3 fromPosition, Vector3 toPosition)
    {
        while(gameObject.transform.position.x > toPosition.x)
        {
            //gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, toPosition, base.Unit_Move_Speed * Time.deltaTime);

            if(gameObject.transform.position.x <= toPosition.x)
            {
                base.Unit_Is_Moving = false;
                golemAnim.SetBool(moveAnimationState, false);
                OnMove(base.Unit_Is_Moving);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator GolemDefend()
    {
        //Amount of time the golem will actually be in the defending state, plus the time, in seconds, 
        //that the golem will animates that state.
        yield return new WaitForSeconds(defendAnimationTime);

        //Set the animation state back to its default animation.
        golemAnim.SetBool(defendAnimationState, false);

        isDefending = false;

        EventManager.TriggerEvent("EndState");
    }

    IEnumerator GolemStunned()
    {
        isStunned = true;

        golemAnim.SetBool(stunAnimationState, true);

        yield return new WaitForSeconds(stunAnimationTime);

        isStunned = false;

        OnStunned(false);
    }

    #endregion
}
