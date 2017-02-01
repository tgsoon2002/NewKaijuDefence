using UnityEngine;
using System.Collections;

public class Huitzl : Golem 
{
	// Use this for initialization
	protected override void Start () 
    {
        base.Start();
        base.Unit_Type = UnitType.GOLEM;
		base.Unit_Is_Moving = true;
        base.Golem_Default_Move_Range = 10.0f;
	}
	
    protected override void MoveThisUnit(float position)
    {
        base.MoveThisUnit(position);
    }

    protected override void GolemDefendPosition()
    {
        base.GolemDefendPosition();
    }
}
