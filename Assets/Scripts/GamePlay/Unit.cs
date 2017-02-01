using UnityEngine;
using System.Collections;

public enum UnitType
{
    PLAYERUNIT,
    GOLEM
}

public abstract class Unit : MonoBehaviour
{
    #region Data Members

    [SerializeField]
    private string unitName;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private int maxHP;
    [SerializeField]
    private int currentHP;
    [SerializeField]
    private UnitType type;
    [SerializeField]
    private float sightRange;
    [SerializeField]
    private bool isMoving;

    #endregion

    #region Setters & Getters

    protected virtual string Unit_Name
    {
        get { return unitName; }
        set { unitName = value; }
    }

    protected virtual float Unit_Move_Speed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    protected virtual int Unit_Max_HP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    protected virtual int Unit_Current_HP
    {
        get { return currentHP; }
        set { currentHP = value; }
    }

    protected virtual UnitType Unit_Type
    {
        get { return type; }
        set { type = value; }
    }

    protected virtual float Unit_Sight_Range
    {
        get { return sightRange; }
        set { sightRange = value; }
    }

    protected virtual bool Unit_Is_Moving
    {
        get { return isMoving; }
        set { isMoving = value; }
    }

    #endregion

    #region Built-in Unity Methods

    //None

    #endregion

    #region Main Methods

    protected virtual void TakeDamage(int damageValue)
    {
        currentHP -= damageValue;
    }

    protected virtual bool IsUnitDead()
    {
        return (currentHP == 0.0f);
    }

    protected virtual float PercentHealth()
    { 
        return ((float)(currentHP)/(float)(maxHP));
    }

    protected virtual void MoveThisUnit(float position) 
    { 
    
    }

    #endregion
}