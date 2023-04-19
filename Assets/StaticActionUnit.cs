using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticActionUnit : Unit
{
    [SerializeField] Vector2[] checkCollisionPos;
    [SerializeField] int posCount = 0;
    [SerializeField] Action collidingAction;
    bool haveAction;

    private void Start()
    {
        collidingAction = null;
        haveAction = false;
    }
    public void AddNewCollidingPos(Vector2 pos)
    {
        if (haveAction == false)
        {
            collidingAction = new Action
            {
                from = this,
                index = 0,
                parameters = new Vector2[] { pos },
                type = "colliding"
            };
            checkCollisionPos = new Vector2[10];
            //Debug.Log("插入" + Random.Range(0, 100));
            GameManager.instance.InsertAcition(collidingAction);
            
            haveAction = true;
        }
        bool unique = true;
        foreach(Vector2 existPos in checkCollisionPos)
        {
            if (existPos.x == pos.x && existPos.y == pos.y)
            {
                unique = false;
                return;
            }
        }

        if (unique)
           checkCollisionPos[posCount++] = pos;
    }
    public override void PerformAction(Unit to, int index, Vector2[] parameters)
    {
        switch (index)
        {
            //檢查碰撞
            case 0:
                for(int p=0; p<posCount; p++)
                {

                    Unit[] cell = GameManager.instance.GetCellUnits(checkCollisionPos[p]);
                    int i = 0;
                    while (i < cell.Length && cell[i] != null)
                        i++;
                    if (i > 1)   //代表有重複的單位
                    {
                        while (--i >= 0)
                            GameManager.instance.RemoveUnit(cell[i], parameters[0]);
                    }
                }
                //移除檢查需求
                collidingAction = null;
                haveAction = false;
                posCount = 0;
                break;
            case 999:
                GameManager.instance.UnregisterPosition(this, position);
                this.transform.position = new Vector3((team) ? -8.5f : 8.5f, 4.5f, 0);
                break;
        }
    }

    private void Update()
    {
    }
}
