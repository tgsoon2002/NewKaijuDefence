using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface AttackInterface
{
    bool Skill_On_CD
    {
        get;
    }

    AttackType Get_Attack_Type
    {
        get;
    }

    int Number_Of_Targets
    {
        get;
    }

    float Get_Attack_Range
    {
        get;
        set;
    }

    bool Attack_Anim_Done
    {
        get;
        set;
    }

    string Attack_Name
    {
        get;
    }

    float Skill_CD_Value
    {
        get;
        set;
    }

    void ManualStartCoolDown();
    void ExecuteAttack();
    void SetTarget(Vector3 pos);
}
