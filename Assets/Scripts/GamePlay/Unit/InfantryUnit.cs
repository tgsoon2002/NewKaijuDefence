using UnityEngine;
using System.Collections;

public class InfantryUnit : PlayerUnit
{

    #region member
    public ParticleSystem muzzle;
    public Transform Turret;
    public GameObject barrelEnd;
    public Transform healthBar;
    public float fireRateTimer = 0;
    public Animator unitAnimator;
    public GameObject TempBullet;


    #endregion
    #region built-in Method

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Danger") && other.CompareTag("EnemyLeg"))
        {
            base.TakeDamage(1);
            healthBar.localScale = new Vector3(PercentHealth(), 1.0f, 1.0f);
        }

        if (other.CompareTag("OutOfBound"))
        {
            this.TakeDamage(base.Unit_Max_HP);
        }
    }

    protected override void Start()
    {
        base.Start();
        Unit_AngleAimLimit = 60.0f;
        base.Unit_Muzzle = muzzle;
        base.Unit_Animator = unitAnimator;
        base.Unit_ObjectPhysics = gameObject.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        fireRateTimer += Time.deltaTime;
    }
    #endregion

    #region Main Method

    public override void Move(float direction)
    {
        base.Move(direction);
    }

    public override void Shot()
    {
        if (fireRateTimer >= base.Unit_FireRate)
        {
            base.Shot();
            GameObject bullet = Instantiate(TempBullet, barrelEnd.transform.position, barrelEnd.transform.rotation) as GameObject;
            bullet.GetComponent<Rigidbody>().AddForce(100.0f * barrelEnd.transform.forward);
            fireRateTimer = 0;
        }
    }

    public override void Aim(int direction)
    {
        Unit_CurentAngle = Turret.eulerAngles.z;
        direction *= -1;
        if (
            (direction >= 0 && Unit_CurentAngle <= Unit_AngleAimLimit) ||
            (direction <= 0 && Unit_CurentAngle > 1))
        {

            Turret.Rotate(Vector3.forward * direction);
        }
    }



    #endregion

}
