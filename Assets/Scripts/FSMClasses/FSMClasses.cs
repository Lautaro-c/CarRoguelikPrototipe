using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class FSMClasses : MonoBehaviour
{
    public State currentState {  get; private set; }
    public Patrol patrol;
    public Flee flee;
    public Explode explode;
    private void Awake()
    {
        patrol = new Patrol(this);
        flee = new Flee(this);
        explode = new Explode(this);
        currentState = patrol;
    }

    public void UpdateState(LineOfSight los, Transform self, Transform target)
    {
        currentState.Update(los, self, target);
    }

    public void ChangeToPatrol()
    {
        ChangeState(patrol);
    }

    public void ChangeToFlee()
    {
        ChangeState(flee);
    }

    public void ChangeToExplode()
    {
        ChangeState(explode);
    }

    public string GetClassName()
    {
        return currentState.GetType().Name;
    }

    private void ChangeState(State state)
    {
        if (currentState == state)
        {
            return;
        }
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }
}
