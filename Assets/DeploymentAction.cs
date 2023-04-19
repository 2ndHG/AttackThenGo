using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeploymentAction : Action
{
    /*
    public DeploymentAction(Vector2 pos)
    {
        parameters = new Vector2[1];
        parameters[0] = pos;
    }*/
    public override void Perform()
    {
        Debug.Log("©ñ¸m" + from.gameObject.name + "¦b" + parameters[0]);
        from.position = parameters[0];
        from.transform.position = GameManager.ToCameraPosition(parameters[0]);
        GameManager.instance.RegisterPosition(from, parameters[0]);
    }
}
