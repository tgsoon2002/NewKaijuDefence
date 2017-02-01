using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the unit's skills.
/// Work in progress.
/// </summary>
public class SkillManager : MonoBehaviour
{
    #region Data Members

    public List<GameObject> skillList;

    private GameObject skillObject;
    [SerializeField]
    private Queue<GameObject> skillQueue;
    private bool skillIsExecuting;
    private bool skillDurationDone;
    private bool skillNotNull;

    #endregion

    #region Setters & Getters

    public bool Skill_Duration
    {
        get { return skillDurationDone; }
        set { skillDurationDone = value; }
    }

    public bool Is_Skill_Picked_Null
    {
        get { return skillNotNull; }
        set { skillNotNull = value; }
    }

    #endregion

    #region Built-in Unity Methods

    // Use this for initialization
    void Awake()
    {
        //Instantiates all the gameobjects in the attacklist
        //and stores it back into attacklist
        for(int i = 0; i < skillList.Count; i++)
        {
            GameObject tmp = Instantiate(skillList[i]);
            tmp.transform.parent = gameObject.transform;
            tmp.transform.position = gameObject.transform.position;
        	skillList[i] = tmp;
        }
    }

    void OnEnable()
    {
        EventManager.StartListening("TriggerSkill", TriggerSkill);
    }

	void OnDisable()
	{
		EventManager.StopListening("TriggerSkill", TriggerSkill);
	}

	void Start () 
    {   
	    skillQueue = new Queue<GameObject>();
 
        //Populate our Deque of skills
        for(int i = 0; i < skillList.Count; i++)
        {
            skillQueue.Enqueue(skillList[i]);
        }

        //Initialize our booleans
        skillDurationDone = false;
        skillNotNull = false;
        skillIsExecuting = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        //Debug.Log(skillQueue.Count);
    }

    

    #endregion

    #region Main Methods

    public bool IsSkillInRange(float nearestUnit)
    {
        //Declaring local variables
        AttackInterface iAttack;
        bool isInRange;
        float distance;

        //Initializing local variables
        isInRange = false;

        if(skillObject != null)
        {
            iAttack = skillObject.GetComponent(typeof(AttackInterface))
                                as AttackInterface;

            distance = Mathf.Sqrt(nearestUnit);

            //Check if the unit's distance is within range of attack.
            if(iAttack.Get_Attack_Range <= distance)
            {
                isInRange = true;
            }
        }
      
        return isInRange;
    }

    public float AttackRangeValue()
    {
        //Declaring local variables
        AttackInterface iAttack;

        //Setting the interface with the object
        iAttack = skillObject.GetComponent(typeof(AttackInterface))
                                as AttackInterface;

        return iAttack.Get_Attack_Range;
    }

    public float GetNewMoveDistance(float nearestUnit)
    {
        //Declaring local variables
        AttackInterface iAttack;
        float distance;

        iAttack = skillObject.GetComponent(typeof(AttackInterface))
                                as AttackInterface;

        distance = Mathf.Sqrt(nearestUnit);

        return distance - iAttack.Get_Attack_Range;
    }

    public void PickAction()
    {
        if(skillQueue.Count > 0)
        {
            skillObject = skillQueue.Peek();
            skillNotNull = false;
        }
        else
        {
            skillObject = null;
            skillNotNull = true;
        }
    }

    public void ExecuteSkill()
    {
        //Declaring local variables
        AttackInterface iSkill;

        //Initializing local variables
        iSkill = skillObject.GetComponent(typeof(AttackInterface)) as AttackInterface;

        iSkill.ExecuteAttack();
    }

    public void PutSkillOnQueue(GameObject skill)
    {
        skillQueue.Enqueue(skill);
    }

    public void SkillDone()
    {
        AttackInterface iSkill;

        iSkill = skillObject.GetComponent(typeof(AttackInterface)) as AttackInterface;

        iSkill.ManualStartCoolDown();

        Debug.Log("Skill has finished animating...");

        skillQueue.Dequeue();

        EventManager.TriggerEvent("EndState");
    }

    #endregion

    #region Helper Methods

    void TriggerSkill()
    {
        //if(!skillIsExecuting)
        //{
        ExecuteSkill();
            //skillIsExecuting = true;
        //}
    }

    #endregion
}
