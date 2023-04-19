using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Unit
{
    public override void ShowAvailablePosition(int actionIndex)
    {
        switch (actionIndex)
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
                if (diffPos.x == 2 || diffPos.x == -2)
                    diffPos.x /= 2;
                if (diffPos.y == 2 || diffPos.y == -2)
                    diffPos.y /= 2;
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

    public override void PerformAction(Unit to, int index, Vector2[] parameters)
    {
        Debug.Log(name + " ���");
        switch (index)
        {
            //��¦����
            case 0:
                //���S�ۤv
                SetToExposed();
                Vector2 dir = parameters[0].normalized;
                Vector2 attackPos1 = position + dir, attackPos2 = position + dir*2;

                Unit[] units1 = GameManager.instance.GetCellUnits(attackPos1);
                Unit[] units2 = GameManager.instance.GetCellUnits(attackPos2);
                //���Units�̧������ߤF
                foreach (Unit u in units1)
                {
                    if (u != null)
                        GameManager.instance.RemoveUnit(u, attackPos1);
                }
                foreach (Unit u in units2)
                {
                    if (u != null)
                        GameManager.instance.RemoveUnit(u, attackPos1);
                }
                break;
            //��¦����
            case 1:
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
