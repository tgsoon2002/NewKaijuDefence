using UnityEngine;
using System.Collections;

public enum AttackType
{
    TARGETED,
    AOE,
    SPECIAL
}

public class BaseAttackAction
{
    #region Data Members

    private string attackName;
    private float attackCoolDown;
    private float attackDamageValue;
    private float attackRange;
    private AttackType type;

    #endregion

    #region Constructor

    public BaseAttackAction(string _name, float _cd, float _value, float _range, AttackType _type)
    {
        attackName = _name;
        attackCoolDown = _cd;
        attackDamageValue = _value;
        attackRange = _range;
        type = _type;
    }
   
    #endregion

    #region Setters & Getters

    public string ATTACK_NAME_VALUE
    {
        get { return attackName; }
        set { attackName = value; }
    }

    public float ATTACK_DMG_VALUE
    {
        get { return attackDamageValue; }
        set { attackDamageValue = value; }
    }

    public float ATTACK_RANGE_VALUE
    {
        get { return attackRange; }
        set { attackRange = value; }
    }

    public float ATTACK_CD_VALUE
    {
        get { return attackCoolDown; }
        set { attackCoolDown = value; }
    }

    public AttackType ATTACK_TYPE_ENUM
    {
        get { return type; }
    }

    #endregion
}