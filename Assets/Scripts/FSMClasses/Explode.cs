using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : State
{
    public Explode(FSMClasses fsm) : base(fsm) { }
    public override void Update(LineOfSight los, Transform self, Transform target)
    {
        if (los.CanBeSeen(self, target))
        {
            if (!los.CanFlee(self, target))
            {
                fsm.ChangeToFlee();
            }
        }
        else
        {
            fsm.ChangeToPatrol();
        }
    }
}
