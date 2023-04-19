using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Unit
{

    IEnumerator Setting()
    {
        yield return new WaitForSeconds(1);
        Vector2 position;
        if (team)
            position = new Vector2(6, 1);
        else
            position = new Vector2(5, 9);
        this.position = position;
        GameManager.instance.RegisterPosition(this, position);
        transform.position = GameManager.ToCameraPosition(position);
        yield return null;
    }

    public override void ShowAvailablePosition(int actionIndex)
    {
        switch(actionIndex)
        {
            case 0:
                ShowAttackingPosition(actionIndex);
                break;
            case 1:
                ShowMovingPosition(actionIndex);
                break;
        }
    }

    public override void InsertAction(int actionIndex, Vector2 diffPos)
    {
        Action a = new Action();
        a.from = this;
        a.index = actionIndex;
        a.parameters = new Vector2[1];
        switch (actionIndex)
        {
            //��¦����
            case 0:
                a.parameters[0] = diffPos;
                a.type = "attacking";
                break;
            //��¦����
            case 1:
                a.parameters[0] = position + diffPos;//�ؼЦ�m
                a.type = "moving";
                break;
        }
        GameManager.instance.InsertAcition(a);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Setting());
    }

    public override void PerformAction(Unit to, int index, Vector2[] parameters)
    {
        Debug.Log(name + " ���");
        switch(index)
        {
            //��¦����
            case 0:
                Vector2 attackPos = position + parameters[0];
                Unit[] units = GameManager.instance.GetCellUnits(attackPos);
               
                //���Units�̧������ߤF
                foreach (Unit u in units)
                {
                    if(u != null)
                        GameManager.instance.RemoveUnit(u, attackPos);
                }
                break;
            //��¦����
            case 1:
                if (dead)
                    return;
                GameManager.instance.UnregisterPosition(this, position);
                position = parameters[0];
                GameManager.instance.RegisterPosition(this, position);
                transform.position = GameManager.ToCameraPosition(position);
                break;
            case 999:
                BasicDie();
                break;
        }
    }

}
