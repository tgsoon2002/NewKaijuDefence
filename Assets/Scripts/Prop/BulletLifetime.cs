using UnityEngine;
using System.Collections;

public class BulletLifetime : MonoBehaviour
{
    private float lifetime = 5.0f;
    public int bulletDmg = 3;
    //float smooth;

    //Vector3 prevPosition;
    //Vector3 currentPosition;
    // Use this for initialization

    public Vector3 position;
    public float angleInDeg;
    public float speed;
    private float angle;
    float Xspeed;
    float Yspeed;
    float gravity = 0.0098f;
    float second;
    float time;
    float totalangle;
    public float rotatespeed;
    Vector3 zaxis;
    Vector3 angel;
    //======================
    void Start()
    {
        /*
		angleInDeg= transform.eulerAngles.z;
		Destroy (this.gameObject, lifetime);
		//prevPosition = this.transform.position;
		//currentPosition = this.transform.position;

		//====================
		speed = 0.4f;
		angle =   Mathf.Deg2Rad*angleInDeg;
		Xspeed = speed * Mathf.Cos(angle) ;
		Yspeed =  speed * Mathf.Sin(angle);
		position = transform.position;
		//position.y = transform.position.y;
		

		//calculation for how long does it gonna take.
		time = speed / gravity;
		totalangle = angle+ Mathf.Deg2Rad * 180;
		*/
        //rotatespeed = 1.0f; //9.0f* time/totalangle;//20f;
        //angel = transform.rotation.eulerAngles;
		
        zaxis.x = 0;
        zaxis.y = 0;
        zaxis.z = -1;

    }



    public void Update()
    {
        transform.Rotate(zaxis, rotatespeed * Time.deltaTime);
    }

    //=====================================================================
    // even when bullet hit the head.
    void OnTriggerEnter(Collider target)
    {
        // if target hit an enemy then they take damage
        if (target.CompareTag("Enemy"))
        {	
            target.GetComponent<Golem>().OnHit(bulletDmg);
        }
        // on top of take damage, if enemy part is weak point, it stun
        if (target.CompareTag("EnemyStunPart"))
        {	
            //	target.GetComponent<StunHitScipt>().getHit();
        }
        Instantiate(Resources.Load("Explosion"), this.transform.position, this.transform.rotation);

        Destroy(gameObject);

    }

    //=================================
    public void setDamage(int newDmg)
    {
        bulletDmg = newDmg;
    }

}