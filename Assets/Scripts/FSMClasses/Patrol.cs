using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : State
{
    public Patrol(FSMClasses fsm) : base(fsm) { }

    public override void Update(LineOfSight los, Transform self, Transform target)
    {
        if(los.CanBeSeen(self, target))
        {
            if(los.CanFlee(self, target))
            {
                fsm.ChangeToExplode();
            }
            else
            {
                fsm.ChangeToFlee();
            }
        }
    }
}
