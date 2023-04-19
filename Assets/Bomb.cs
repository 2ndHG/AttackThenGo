using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Unit
{
    public override void ShowAvailablePosition(int actionIndex)
    {
        switch (actionIndex)
        {
            case 0:
                ShowMovingPosition(actionIndex);
                break;
        }
    }
    public override void InsertAction(int actionIndex, Vector2 diffPos)
    {
        if (actionIndex == 1)
            actionIndex = 0;
        Action a = new Action();
        a.from = this;
        a.index = actionIndex;
        a.parameters = new Vector2[1];
        switch (actionIndex)
        {
            //基礎移動
            case 0:
                a.parameters[0] = position + diffPos;//目標位置
                a.type = "moving";
                break;
        }
        GameManager.instance.InsertAcition(a);
    }
    public void Explode(Vector2 pos)
    {
        for(int i=-2; i<=2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                Vector2 p = new Vector2(i, j) + pos;
                if (GameManager.IsInSmallBoundary(p) && Mathf.Abs(i) + Mathf.Abs(j) <= 2)
                {
                    //Debug.Log("引爆" + p);
                    Unit[] cell = GameManager.instance.GetCellUnits(p);
                    int k = 0;
                    while (k < cell.Length && cell[k] != null)
                    {
                        GameManager.instance.RemoveUnit(cell[k], p);
                        k++;
                    }
                }
            }
        }
    }
    public override void PerformAction(Unit to, int index, Vector2[] parameters)
    {
        Debug.Log(name + " 行動");
        if (index == 1)
            index = 0;
        switch (index)
        {
            //基礎移動
            case 0:
                GameManager.instance.UnregisterPosition(this, position);
                position = parameters[0];
                GameManager.instance.RegisterPosition(this, position);
                transform.position = GameManager.ToCameraPosition(position);
                break;
            case 2:
                Explode(parameters[0]);
                break;
            case 999:
                Action a = new Action
                {
                    from = this,
                    index = 2,
                    parameters = new Vector2[1] { position },
                    type = "attacking"
                };
                GameManager.instance.InsertAcition(a);
                BasicDie();
                break;
        }
    }
}