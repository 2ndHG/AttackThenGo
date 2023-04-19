using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : Unit
{
    public override bool CheckPossition(int actionIndex, Vector2 pos)
    {
        Unit[] cell;
        bool overlapping;
        switch (actionIndex)
        {
            case 0:
                return (GameManager.IsInSmallBoundary(pos));
            case 1:
                return CheckMovingPosition(actionIndex, pos);
            case 2:
                return CheckMovingPosition(actionIndex, pos);
        }
        return false;
    }
    public override void ShowAvailablePosition(int actionIndex)
    {
        switch (actionIndex)
        {
            case 0: //��������
                ShowAttackingPosition(actionIndex);
                break;
            case 1:
                ShowMovingPosition(actionIndex);
                break;
            case 2: //����첾��
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
            //��������
            case 2:
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
                GameManager.instance.UnregisterPosition(this, position);
                position = parameters[0];
                GameManager.instance.RegisterPosition(this, position);
                transform.position = GameManager.ToCameraPosition(position);
                break;
            //����첾��
            case 2:
                SetToExposed();
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
