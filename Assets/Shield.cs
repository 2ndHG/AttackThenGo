using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Unit
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
                Vector2 attackPos = position + parameters[0];
                Unit[] units = GameManager.instance.GetCellUnits(attackPos);

                //���Units�̧������ߤF
                foreach (Unit u in units)
                {
                    if (u != null)
                        GameManager.instance.RemoveUnit(u, attackPos);
                }
                break;
            //��¦����
            case 1:
                BasicMove(parameters[0]);
                break;
            case 999:
                if(isCovered)
                {
                    SetToExposed();
                    GameManager.instance.CheckCollision(position);
                }
                else
                {
                    BasicDie();
                }
                break;
        }
    }
}
