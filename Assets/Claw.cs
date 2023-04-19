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
            case 0: //全方位攻擊
                ShowAttackingPosition(actionIndex);
                break;
            case 1:
                ShowMovingPosition(actionIndex);
                break;
            case 2: //全方位移動
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
            //基礎攻擊
            case 0:
                a.parameters[0] = diffPos;
                a.type = "attacking";
                break;
            //基礎移動
            case 1:
                a.parameters[0] = position + diffPos;//目標位置
                a.type = "moving";
                break;
            //全方位攻擊
            case 2:
                a.parameters[0] = position + diffPos;//目標位置
                a.type = "moving";
                break;
        }
        GameManager.instance.InsertAcition(a);
    }

    public override void PerformAction(Unit to, int index, Vector2[] parameters)
    {
        Debug.Log(name + " 行動");
        switch (index)
        {
            //基礎攻擊
            case 0:
                //揭露自己
                SetToExposed();
                Vector2 attackPos = position + parameters[0];
                Unit[] units = GameManager.instance.GetCellUnits(attackPos);

                //對於Units們攻擊成立了
                foreach (Unit u in units)
                {
                    if (u != null)
                        GameManager.instance.RemoveUnit(u, attackPos);
                }
                break;
            //基礎移動
            case 1:
                GameManager.instance.UnregisterPosition(this, position);
                position = parameters[0];
                GameManager.instance.RegisterPosition(this, position);
                transform.position = GameManager.ToCameraPosition(position);
                break;
            //全方位移動
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
