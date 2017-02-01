using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Enum for our AI states 
/// </summary>
public enum AIState
{
    MOVING,
    SPRINTING,
    CHECKING_SKILL_DISTANCE,
    GETTING_SKILL,
    EXECUTING_SKILL,
    ATTACKING,
    DEFENDING,
    STAGGERED
}

public class EnemyUnitAI : MonoBehaviour
{
    #region Data Members
    
    //The Golem this AI is going to be controlling
    private Golem golemObject;

    //Declaring a delegate that will handle states
    //private delegate void HandleState(AIState s);
    //private HandleState runState;

    //Object reference to the Stack based Finite State
    //Machine that this AI class will be using.
    //We will be using the delegate as the type that 
    //the FSM will be using.
    private GolemFiniteStateMachine<AIState> golemBrain;

    //Object reference to the SkillManager class. Will be
    //responsible of spitting out Skills for this AI to
    //use for the attacking state.
    private SkillManager skillHandler;

    //List containing player unit gameobjects that are 
    //within this golem's sight range
    List<GameObject> units;

    //List of floats that will contain the squared distances
    //of each player gameobject relative to the golem.
    //Should already be sorted in ascending order.
    List<float> unitSquaredDistances;

    //Determines which direction the golem will move.
    private float moveDirection;

    //This boolean is to be used to check if a state
    //is done running.
    private bool stateDone;

    //This boolean will be in charge of making the golem
    //sprinting for x-amount of time.
    private bool isSprinting;
    private bool sprintCoolDown;

    //This boolean will check if a skill has already
    //been picked.
    private bool skillPicked;

    #endregion

    #region Setters & Getters

    #endregion

    #region Build-In Unity Methods

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening("EndState", TriggerEndState);
        EventManager.StartListening("StartState", ForceStartState);
    }

	/// <summary>
	/// 
	/// </summary>
	void OnDisable()
	{
		EventManager.StopListening("EndState", TriggerEndState);
		EventManager.StopListening("StartState", ForceStartState);
	}

    /// <summary>
    /// Called when the object component is enabled
    /// Initializes data members before the first 
    /// Update loop, if and only if the former is 
    /// done first.
    /// </summary>
	void Start() 
    {
        //Getting the reference for our Golem object.
	    golemObject = gameObject.GetComponent<Golem>();

        //Getting the reference for the SkillManager object.
        skillHandler = gameObject.GetComponent<SkillManager>();

        //Instantiating an object for the stack based FSM
        golemBrain = new GolemFiniteStateMachine<AIState>();

        //Initialize the golem's movement direction.
        //Will always move the left side initially.
        moveDirection = -1.0f;

        //Initialize our booleans
        stateDone = false;
        skillPicked = false;
        isSprinting = false;
        sprintCoolDown = false;

        //Instantiate our lists
        units = new List<GameObject>();
        unitSquaredDistances = new List<float>();

        //Telling our delegate which methods to run.
//        runState += MovingState;
//        runState += PickSkillState;
//        runState += SprintingState;
//        runState += CheckSkillDistance;
//        runState += ExecuteSkillState;
//        runState += DefendState;
      
        //Push the default state, which should be the
        //moving state.
        golemBrain.PushState(AIState.MOVING); 
	}

    /// <summary>
    /// Called once per frame
    /// </summary>
	void Update () 
    {
        //Run the current state

        //runState();
		switch (golemBrain.CurrentState()) {
		case AIState.MOVING:
			MovingState(golemBrain.CurrentState());
			break;
		case AIState.GETTING_SKILL:
			PickSkillState(golemBrain.CurrentState());
			break;
		case AIState.SPRINTING:
			SprintingState(golemBrain.CurrentState());
			break;
		case AIState.CHECKING_SKILL_DISTANCE:
			CheckSkillDistance(golemBrain.CurrentState());
			break;
		case AIState.EXECUTING_SKILL:
			ExecuteSkillState(golemBrain.CurrentState());
			break;
		case AIState.DEFENDING:
			DefendState(golemBrain.CurrentState());
			break;

		
		default:
			break;
		}
        if(sprintCoolDown)
        {
            StartCoroutine(SprintCoolDown());
        }
    }



    #endregion

    #region Main Methods

    /// <summary>
    /// Takes care of any behaviors for this golem's
    /// moving state.
    /// </summary>
    /// <param name="s">Enum AIState</param>
    void MovingState(AIState s)
    {
//        //Delegate will check if this is the current state.
//        if(s == AIState.MOVING)
//        {
            //Let's move our golem
            golemObject.Move(moveDirection);

            //Check if a skill has been picked prior
            if(!skillPicked)
            {
                //Get the list of units that are within in the golem's
                //sight range.
                units = CheckUnitsInProximity(golemObject.Golem_Sight_Range);

                //If there is even one unit within the list, it is 
                //time for the golem to pick a skill to use.
                if(units.Count > 0)
                {
                    golemBrain.PushState(AIState.GETTING_SKILL);
                }
            }
        //}
    }

    /// <summary>
    /// Takes care of the sprinting state 
    /// behavior for a golem.
    /// </summary>
    /// <param name="s"></param>
    void SprintingState(AIState s)
    {
//        if(s == AIState.SPRINTING)
//        {
            //Check if the golem is not sprinting yet
            if(!isSprinting)
            {
                //Start the Coroutine
                StartCoroutine(SprintTimer());
            }

            //Change this to sprint
            golemObject.Sprint(moveDirection);

            golemBrain.PopState();

            //Since the golem has already a skill picked,
            //keep checking it 
            golemBrain.PushState(AIState.CHECKING_SKILL_DISTANCE);
        //}
    }

    /// <summary>
    /// This is the state in which the AI will
    /// retrieve an attack fron the skill manager
    /// stack of skills.
    /// </summary>
    /// <param name="s">Enum AIState</param>
    void PickSkillState(AIState s)
    {
        //Delegate will check if this is the current state
//        if(s == AIState.GETTING_SKILL)
//        {
            //This is where a skill object is obtained.
            //PickAction will return a null object if no
            //attacks are in the stack. 
            skillHandler.PickAction();

            //Pop the current state from the stack,
            //since we already picked a skill.
            golemBrain.PopState();

            //If the skill is null, the state will then
            //transition into Defending.
            if(skillHandler.Is_Skill_Picked_Null == true)
            {
                //Switch the state to DEFENDING
                golemBrain.PushState(AIState.DEFENDING);
            }
            //Otherwise, go to the sprinting state
            else if(!sprintCoolDown)
            {
                //Assign this to true now that we have a
                //skill to use.
                skillPicked = true;

                golemBrain.PushState(AIState.SPRINTING);
            }
//        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    void DefendState(AIState s)
    {
        //Pretty self-explanatory...
//        if(s == AIState.DEFENDING)
//        {
            golemObject.Defend();
//        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    void CheckSkillDistance(AIState s)
    {
//        if(s == AIState.CHECKING_SKILL_DISTANCE)
//        {
            //Declaring local variables
            bool inRange = false;

            //Call the helper function to determine how many player unit
            //objects are within the current skill range.
            units = CheckUnitsInProximity(skillHandler.AttackRangeValue());

            if(units.Count > 0)
            {
                unitSquaredDistances = CreateListOfSquaredMagnitudes(units);
                inRange = skillHandler.IsSkillInRange(unitSquaredDistances[0]);
            }

            golemBrain.PopState();

            if(inRange)
            {
                golemBrain.PushState(AIState.EXECUTING_SKILL);
            }
            else if(isSprinting)
            {
                golemBrain.PushState(AIState.SPRINTING);
            }
            //If the above two statements are not met,
            //then go to the default state - moving
            else
            {
                skillPicked = false;
            }
//        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    void ExecuteSkillState(AIState s)
    {
//        if(s == AIState.EXECUTING_SKILL)
//        {
            EventManager.TriggerEvent("TriggerSkill");
//        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    void StunnedState(AIState s)
    {
        if(s == AIState.STAGGERED)
        {
            golemObject.Stun();
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// This helper method will return a list of 
    /// player units that are in the golem's proximity.
    /// </summary>
    /// <returns>List units</returns>
    /// <param name="range">float range</param>
    List<GameObject> CheckUnitsInProximity(float range)
    {
        //Declaring local variables
        List<GameObject> tmpUnitList;
        Collider[] colliderArray;

        //Initializing local variables
        tmpUnitList = new List<GameObject>();

        //OverlapSphere is a method within Unity's Physics library.
        //This will create an imaginary sphere, with a given radius, and
        //a Vector 3 position to start. Any gameobjects that will 
        //collide with this 'sphere' will be appended onto the 
        //ArrayList colliderArray.
        colliderArray = Physics.OverlapSphere(gameObject.transform.position,
                                              range);

        //Parse through that ArrayList and get the objects with the 
        //tag we want.
        for(int i = 0; i < colliderArray.Length; i++)
        {
            if(colliderArray[i].gameObject.tag == "Unit")
            {
                tmpUnitList.Add(colliderArray[i].gameObject);
            }
        }

        //Return the list, which may or may not contain
        //any objects.
        return tmpUnitList;
    }

    /// <summary>
    /// Calculates the squared magnitude of each
    /// object in the List parameter between said
    /// object and the enemy golem.
    /// </summary>
    /// <param name="units">List units</param>
    /// <returns>List distance</returns>
    List<float> CreateListOfSquaredMagnitudes(List<GameObject> units)
    {
        //Declaring local variables
        List<float> squaredDistances;
        Vector3 tmpVec;
        float tmpVal;

        //Initializing local variables
        squaredDistances = new List<float>();

        //Loop through the units list
        for(int i = 0; i < units.Count; i++)
        {
            //Create a Vector3 object between the unit and the golem.
            tmpVec = units[i].transform.position - gameObject.transform.position;

            //Invoke SqrMagnitude to get the value we want.
            tmpVal = Vector3.SqrMagnitude(tmpVec);

            //Append the calculated distance in the list. 
            squaredDistances.Add(tmpVal);

            //Reinitialize our local variables before it loops back.
            tmpVec = Vector3.zero;
            tmpVal = 0.0f;
        }

        //List.OrderBy can only be called if we used Linq in our
        //pre-processing directives. This method will take in a lambda
        //expression and sort the values in the list in ascending order.
        squaredDistances = squaredDistances.OrderBy(o => o).ToList();

        //Return the list.
        return squaredDistances;
    }

    /// <summary>
    /// 
    /// </summary>
    void ForceStartState()
    {
        if(golemBrain.CurrentState() != AIState.MOVING)
        {
            golemBrain.PopState();
        }

        golemBrain.PushState(AIState.STAGGERED);
    }

    /// <summary>
    /// Called by classes outside the scope of this one
    /// in order to tell the AI to end the current state
    /// it is running.
    /// </summary>
    void TriggerEndState()
    {
        stateDone = true;

        switch(golemBrain.CurrentState())
        {
            case AIState.EXECUTING_SKILL:

                skillPicked = false;
                golemBrain.PopState();

                break;

            case AIState.DEFENDING:

                golemBrain.PopState();

                break;

            case AIState.STAGGERED:

                golemBrain.PopState();
               // isStunned = false;

                break;
        }
    }
    
    /// <summary>
    /// This coroutine will be in charge of telling
    /// how long the golem will be sprinting.
    /// </summary>
    /// <returns></returns>
    IEnumerator SprintTimer()
    {
        //Setting the sprint value to true
        isSprinting = true;

        //Placeholder value for the sprint timer
        yield return new WaitForSeconds(5.0f);

        //After the timer's done
        isSprinting = false;
        sprintCoolDown = true;
    }

    /// <summary>
    /// This coroutine will be in charge of teling
    /// the golem how long before it can sprint
    /// once more.
    /// </summary>
    /// <returns></returns>
    IEnumerator SprintCoolDown()
    {
        sprintCoolDown = true;

        //Placeholder for the cooldown of the golem's sprint
        yield return new WaitForSeconds(5.0f);

        sprintCoolDown = false;
    }

    #endregion
}
