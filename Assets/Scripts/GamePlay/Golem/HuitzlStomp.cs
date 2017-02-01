using UnityEngine;
using System.Collections;

public class HuitzlStomp : MonoBehaviour, AttackInterface
{
    #region Data Members

  //  private AttackManager attackManager;
    private SkillManager skillManager;
    private Vector3 target;
    private Animator golemAnim;
    private BaseAttackAction attackInfo;
    private bool isOnCoolDown;
    private int numTargets;
    private bool isAnimDone;

    #endregion

    #region Setters & Getters

    public bool Skill_On_CD
    {
        get { return isOnCoolDown; }
    }

    public AttackType Get_Attack_Type
    {
        get { return attackInfo.ATTACK_TYPE_ENUM; }
    }

    public int Number_Of_Targets
    {
        get { return numTargets; }
    }

    public float Get_Attack_Range
    {
        get { return attackInfo.ATTACK_RANGE_VALUE; }
        set { attackInfo.ATTACK_RANGE_VALUE = value; }
    }

    public bool Attack_Anim_Done
    {
        get { return isAnimDone; }
        set { isAnimDone = value; }
    }

    public string Attack_Name
    {
        get { return attackInfo.ATTACK_NAME_VALUE; }
    }

    public float Skill_CD_Value
    {
        get { return attackInfo.ATTACK_CD_VALUE; }
        set { attackInfo.ATTACK_CD_VALUE = value; }
    }

    #endregion

    #region Built-in Unity Methods

    // Use this for initialization
	void Start () 
    {
        numTargets = 1;
        isOnCoolDown = false;
        isAnimDone = false;
        golemAnim = gameObject.GetComponentInParent<Animator>();
        skillManager = gameObject.GetComponentInParent<SkillManager>();
        //attackManager = gameObject.GetComponentInParent<AttackManager>();
        //golemAnim = gameObject.GetComponent<Animator>();
	    attackInfo = new BaseAttackAction("HuitzlStomp", 6.0f, 6.0f, 10.0f, AttackType.AOE); 
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(isAnimDone == true)
        {
            StartCoroutine(CoolDownTimer());
            isAnimDone = false;
        }
    }

    #endregion

    #region Main Methods

    public void ExecuteAttack()
    {
        golemAnim.SetBool("GolemStomp", true);
        //StartCoroutine(AttackAnimationState());
    }

    public void SetTarget(Vector3 pos)
    {

    }

    public void ManualStartCoolDown()
    {
        golemAnim.SetBool("GolemStomp", false);
        StartCoroutine(CoolDownTimer());
    }

    IEnumerator AttackAnimationState()
    {
        isAnimDone = false;
        golemAnim.SetBool("GolemStomp", true);
        yield return new WaitForSeconds(2.5f);
        golemAnim.SetBool("GolemStomp", false);
        isAnimDone = true;
      //  attackManager.Is_Attack_Done = isAnimDone;
    }

    IEnumerator CoolDownTimer()
    {
        isOnCoolDown = true;
        yield return new WaitForSeconds(attackInfo.ATTACK_CD_VALUE);
        isOnCoolDown = false;
        Debug.Log("Pushing stomp to stack...");
        skillManager.PutSkillOnQueue(this.gameObject);
    }

    #endregion
}
