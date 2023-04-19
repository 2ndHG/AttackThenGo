using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action
{
    public string type;
    public int index;
    public Unit from, to;
    public Vector2[] parameters;

    public virtual void Perform()
    {
        from.PerformAction(to, index, parameters);
    }
}
