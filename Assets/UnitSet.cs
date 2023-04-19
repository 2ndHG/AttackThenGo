using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitSet : MonoBehaviour
{
    public int rarity;
    public Unit[] units;
    public bool team;
    public Vector2[] legalPositions;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 cameraDiff;
    [SerializeField] private bool waitingPick, choosingPos;
    int picked;
    Vector2 place;

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        waitingPick = true;
        //Debug.Log("產生棋子組");
    }
    public void SetUnitsTeam(bool t)
    {
        //Debug.Log("設定隊伍"+t.ToString());
        team = t;
        picked = -1;
        if(!team)
        {
            for(int i=0; i<legalPositions.Length; i++)
            {
                legalPositions[i].y = 10 - legalPositions[i].y;
            }
            foreach(Unit u in units)
            {
                u.SetTeam(false);

            }
        }
    }
    public void Update()
    {
        if(waitingPick)
        {
            if(team)
            {
                if(Input.GetKeyDown(KeyCode.Z))
                {
                    picked = 0;
                } 
                else if( Input.GetKeyDown(KeyCode.X))
                {
                    picked = 1;
                }
                else if( Input.GetKeyDown(KeyCode.C))
                {
                    picked = 2;
                }
                if (picked != -1)
                {
                    waitingPick = false;
                    choosingPos = true;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Keypad1))
                {
                    picked = 0;
                }
                else if (Input.GetKeyDown(KeyCode.Keypad2))
                {
                    picked = 1;
                }
                else if (Input.GetKeyDown(KeyCode.Keypad3))
                {
                    picked = 2;
                }
                if (picked != -1)
                {
                    waitingPick = false;
                    choosingPos = true;
                }
            }
        } 
        else if (choosingPos)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            { 
                Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition) - cameraDiff;

                Vector2 choosePos = new Vector2(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
                //Debug.Log("已選擇" + choosePos.ToString());
                if (Array.IndexOf(legalPositions, choosePos) > -1)
                {
                    units[picked].transform.parent = null;
                    units[picked].SetToCovered();
                    units[picked].transform.position = transform.position;
                    Debug.Log("玩家" + team+ "已選擇位置: " + choosePos.ToString());
                    DeploymentAction dA = new DeploymentAction
                    {
                        from = units[picked],
                        parameters = new Vector2[] { choosePos }
                    };
                    GameManager.instance.DeployAcition(dA);

                    if (team)
                        GameManager.instance.playerGreen.thinkingCount--;
                    else
                        GameManager.instance.playerOrange.thinkingCount--;

                    Destroy(gameObject);
                }
                else
                    Debug.Log("不合法的位置");
            }
                
        }
    }
}
