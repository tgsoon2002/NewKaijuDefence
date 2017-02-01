using UnityEngine;
using System.Collections;

public enum BasePlayerUnitType
{
    AIR,
    TANK,
    ARTILLERY,
    INFANTRY
}

public abstract class PlayerUnit : Unit
{
    #region Data Members

    Transform barrel;

    //Reference to this player unit's Animator
    private Animator unitAnim;

    //Adding a rigidbody to this player unit
    private Rigidbody2D objectPhysics;

    //The player unit's base unit type
    [SerializeField]
    private BasePlayerUnitType unitType;

    //Boolean that will be set to true or false,
    //depending if this unit is shooting.
    private bool isShooting;
    private bool enemyInRange;
    //Checks if this unit just spawned.
    private bool freshlySpawned;

    //This value checks if this gameobject
    //is currently being controlled by the player.
    [SerializeField]
    private bool isControlled;


    //This value will be firing angle of the unit.
    private float angleOfBarrel;

    //In charge of the max. angle limit the player
    //can aim the barrel of this unit.
    private float angleAimLimit;

    //show how much the damage the bullet does to golem
    private int power;

    //how often the unit shoot.
    [SerializeField]
    private float fireRate;
    //This value wunitTypepe used by the GameManager, to

    //determined which unit is which in the scene.
    private int unitCost;

    private ParticleSystem muzzleFlash;



    //private bool isFocused = false;

    #endregion

    #region Setters & Getters

    public int UnitCost
    {
        get { return unitCost; }
    }

    public bool Unit_Moving
    {
        get { return base.Unit_Is_Moving; }
        set { base.Unit_Is_Moving = value; }
    }

    protected ParticleSystem Unit_Muzzle
    {
        set { muzzleFlash = value; }
    }

    protected Animator Unit_Animator
    {
        set { unitAnim = value; }
    }

    public Rigidbody2D Unit_ObjectPhysics
    {
        get { return objectPhysics; }
        set { objectPhysics = value; }
    }

    public float Unit_FireRate
    {
        get { return fireRate; }
        set { fireRate = value; }
    }

    public void Set_UnitInfo(int PW, int HP, float MV, float FR, int Cost)
    {
        unitCost = Cost;
        power = PW;
        base.Unit_Max_HP = HP; 
        base.Unit_Current_HP = HP;
        fireRate = FR;
        base.Unit_Move_Speed = MV;

    }

    public bool Unit_Is_Controlled_Manually
    {
        get { return isControlled; }
        set { isControlled = value; }
    }

    public BasePlayerUnitType Base_Unit_Type
    {
        get { return unitType; }
        set { unitType = value; }
    }

    #endregion

    #region Built-in Unity Methods

    // Use this for initialization
    protected virtual void Start()
    {
        enemyInRange = false;
        Unit_Current_HP = Unit_Max_HP;
        //Initializing our booleans
        isControlled = false;
        base.Unit_Is_Moving = false;
    }

    void FixedUpdate()
    {
        if (isControlled)
        {
            Shot();
        }
    }

    // Update is called once per frame
    void Update()
    {
//		if (this.transform.position.x - CombatSceneManager.instance.mainEnemy.transform.position.x <= ) {
//			
//		}
        enemyInRange = true;
    }

    #endregion

    #region Main Methods

    // reduce health of unit in parent class. check if health is below or equal 0, kill unit.
    public void DamageUnit(int damage)
    {
        base.TakeDamage(damage);
        if (base.Unit_Current_HP <= 0)
        {
            //Kill this unit
            KillThisUnit();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    public virtual void Move(float direction)
    {	
        if (direction != 0) // if unit move, add velocity and change animation moving to true
        {
            objectPhysics.velocity = new Vector2(direction * base.Unit_Move_Speed,
                objectPhysics.velocity.y);
            unitAnim.SetBool("Moving", true);
        }
        else  // else set animation moving to false
        {
            unitAnim.SetBool("Moving", false);
        }
    }

    public virtual void Aim(int direction)
    {

    }

    //================Shooting Method====================
    public virtual void Shot()
    {
        muzzleFlash.Stop();
        muzzleFlash.Play();
        //Set the animation
        if (!unitAnim.GetBool("Shooting"))
        {
            unitAnim.SetBool("Shooting", true);
        }
        unitAnim.Play("Shooting", -1, 0f);
        // Make shooting sound
        gameObject.GetComponent<AudioSource>().Play();
        Invoke("ResetShooting", 0.1f);

        GameObject newUnit = Instantiate(Resources.Load("Bullet"), barrel.position, barrel.rotation) as GameObject;
        newUnit.GetComponent<Rigidbody2D>().AddForce(900.0f * barrel.right);

    }

    void ResetShooting()
    {
        unitAnim.SetBool("Shooting", false);
    }

    #endregion

    #region Helper Methods

    public virtual void KillThisUnit()
    {
        unitAnim.SetBool("UnitDie", true);
        //CombatSceneManager.instance.RemoveUnitFromList(this.gameObject);
    }



    protected virtual float Unit_AngleAimLimit
    {
        get { return angleAimLimit; }
        set { angleAimLimit = value; }
    }

    protected virtual float Unit_CurentAngle
    {
        get { return angleOfBarrel; }
        set { angleOfBarrel = value; }
    }

    #endregion
}
