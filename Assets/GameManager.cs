using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 cameraDiff;

    public int turnCount;
    public Player playerGreen, playerOrange;    //true is green
    public StaticActionUnit staticActionUnit;

    public UnitSet[] unitSets;
    Unit[][][] field;
    int[] deployTurn = { 1, 2, 3, /*5, 7, 9*/};
    List<Phase> phaseList = new List<Phase>();
    Queue<Action> nextActionQueue = new Queue<Action>();
    public int phaseIndex;
    Queue<int> UnitRarity = new Queue<int>();

    //玩家選擇
    public GameObject[] selectors;
    bool selectingAction = false, selectingPosition = false;
    Unit selectedUnit = null;
    int actionIndex = -1;
    public static Vector2 ToCameraPosition(Vector2 pos)
    {
        return pos - new Vector2(5.5f, 5.5f);
    }
    public static bool IsInBigBoundary(Vector2 pos)
    {
        return (pos.x > -1) && (pos.x < 12)  && (pos.y > 0) && (pos.y < 11);
    }
    public static bool IsInSmallBoundary(Vector2 pos)
    {
        return (pos.x > 0) && (pos.x < 11) && (pos.y > 0) && (pos.y < 10);
    }
    struct Phase
    {
        public Phase(string t, Queue<Action> a)
        {
            type = t;
            actions = a;
        }
        public string type;    //移動或攻擊 moving, attacking
        public Queue<Action> actions;
    };
    IEnumerator PerformPhase(Phase phase)
    {
        while(phase.actions.Count != 0)
        {
            phase.actions.Dequeue().Perform();
        }
        yield return new WaitForSeconds(3);
    }

    public void RemoveUnit(Unit u, Vector2 pos)
    {
        Action a = new Action();
        a.type = "killing";
        a.from = u;
        a.index = 999;
        InsertAcition(a);
    }
    public Unit[] GetCellUnits(Vector2 pos)
    {
        Unit[] cell = field[Mathf.FloorToInt(pos.x)][Mathf.FloorToInt(pos.y)];
        return cell;
    }

    public void RegisterPosition(Unit u, Vector2 pos)
    {
        Unit[] cell = field[Mathf.FloorToInt(pos.x)][Mathf.FloorToInt(pos.y)];
        int i = 0;
        while (cell[i] != null)
            i++;
        cell[i] = u;
        CheckCollision(pos);
        //i--;
        //bool removedU=false;
        //移除重疊的棋子 =>>>現已交給staticActionUnit處理
        //while (i >= 0)
        //{
        //    if (cell[i] != null)
        //    {
        //        RemoveUnit(cell[i], pos);
        //        if (!removedU)
        //        {
        //            RemoveUnit(u, pos);
        //            removedU = !removedU;
        //        }
        //    }
        //    i--;
        //}


    }
    public void UnregisterPosition(Unit u, Vector2 pos)
    {
        Unit[] cell = field[Mathf.FloorToInt(pos.x)][Mathf.FloorToInt(pos.y)];
        int i = 0;
        while (i < cell.Length && cell[i] != u )
            i++;
        if (i == cell.Length)
            return;             //unit早已不存在
        //把後面的Unit移到前一位補上
        while(i < cell.Length-1)
        {
            cell[i] = cell[i + 1];
            i++;
        }
        cell[i] = null;
    }

    public void CheckCollision(Vector2 pos)
    {
        staticActionUnit.AddNewCollidingPos(pos);
    }
    void SetUnitsRarity(int[] raritySet)
    {
        foreach (int r in raritySet)
            UnitRarity.Enqueue(r);
    }
    
    void Deploy(int rarity)
    {
        UnitSet greenSet = Instantiate(unitSets[rarity - 1]);
        UnitSet orangeSet = Instantiate(unitSets[rarity - 1]);

        greenSet.transform.position = new Vector3(-8, -3.5f, 0);
        orangeSet.transform.position = new Vector3(8, -3.5f, 0);

        greenSet.SetUnitsTeam(true);
        orangeSet.SetUnitsTeam(false);
    }
    
    public void DeployAcition(DeploymentAction action)
    {
        Action nextTurnMove = new Action();

        foreach(Phase p in phaseList)
        {
            if(p.type == "deploying")
            {
                p.actions.Enqueue(action);

                nextTurnMove.type = "moving";
                nextTurnMove.index = 1; //基礎移動
                nextTurnMove.from = action.from;
                Vector2 pos = action.parameters[0];
                Debug.Log(action.parameters[0]);
                if (pos.x == 0)
                    pos.x++;
                if (pos.x == 11)
                    pos.x--;
                if (pos.y == 0)
                    pos.y++;
                if (pos.y == 10)
                    pos.y--;
                nextTurnMove.parameters = new Vector2[] { pos };
                InsertActionNext(nextTurnMove);
                return;
            }
        }
    }
    public void InsertAcition(Action action)
    {
        if(action.type == "killing")
        {
            if(phaseList.Count-1 == phaseIndex || phaseList[phaseIndex+1].type != "killing")
            {
                Phase p = new Phase("killing", new Queue<Action>());
                phaseList.Insert(phaseIndex + 1, p);
            }
            phaseList[phaseIndex + 1].actions.Enqueue(action);
            return;
        }
        if (action.type == "colliding")
        {
            if (phaseList.Count - 1 == phaseIndex || phaseList[phaseIndex + 1].type != "colliding")
            {
                //Debug.Log("新階段" + phaseIndex);
                Phase p = new Phase("colliding", new Queue<Action>());
                phaseList.Insert(phaseIndex + 1, p);
            }
            phaseList[phaseIndex + 1].actions.Enqueue(action);
            return;
        }
        
        for (int i=phaseIndex; i<phaseList.Count; i++)
        {
            if (phaseList[i].type == action.type)
            {
                phaseList[i].actions.Enqueue(action);
                return;
            }
        }
        if (action.type == "attacking")
        {
            Phase p = new Phase(action.type, new Queue<Action>());
            phaseList.Insert(phaseIndex + 1, p);
            phaseList[phaseIndex + 1].actions.Enqueue(action);
            return;
        }
    }
    public void InsertActionNext(Action action)
    {
        nextActionQueue.Enqueue(action);
    }
    IEnumerator TurnStart()
    {
        turnCount++;
        Debug.Log(turnCount + ": 回合開始!");
        //=============== 回合開始 ===============
        
        phaseIndex = 0;
        phaseList.Clear();
        phaseList.Add(new Phase("attacking", new Queue<Action>()));
        phaseList.Add(new Phase("moving", new Queue<Action>()));

        //=============== 準備階段 ===============
        //      =============== 上回效果執行 ===============
        while (nextActionQueue.Count > 0)
            InsertAcition(nextActionQueue.Dequeue());
        //      =============== 部屬 ===============
        if (Array.IndexOf(deployTurn, turnCount) != -1)
        {
            phaseList.Insert(0, new Phase("deploying", new Queue<Action>()));
            playerGreen.thinkingCount++;
            playerOrange.thinkingCount++;
            int r = UnitRarity.Dequeue();
            Deploy(r);
        }
        while (playerGreen.thinkingCount != 0 || playerOrange.thinkingCount != 0)
            yield return null;
        //      =============== 動作 ===============
        playerGreen.thinkingCount++;
        playerOrange.thinkingCount++;
        while (playerGreen.thinkingCount != 0 || playerOrange.thinkingCount != 0)
            yield return null;
        //=============== 動作開始 ===============
        Debug.Log(turnCount + ": 動作開始 " + phaseList.Count);
        
        while(phaseIndex < phaseList.Count)
        {
            Queue<Action> actions = phaseList[phaseIndex].actions;
            Debug.Log(phaseIndex + phaseList[phaseIndex].type + "行動數量為: " + actions.Count);
            //yield return new WaitForSeconds(1f);
            bool needToWait=false;
            while (actions.Count > 0)
            {
                actions.Dequeue().Perform();
                needToWait = true;
            }
                
            phaseIndex++;
            if(needToWait)
                yield return new WaitForSeconds(1f);
        }
        //=============== 攻擊 ===============

        //=============== 攻擊結束 ===============

        //=============== 移動 ===============

        //=============== 移動結束 ===============
        yield return null;
        Debug.Log(turnCount + ": 回合結束!");
        
    }
    IEnumerator MatchStart()
    {
        while(true)
            yield return StartCoroutine(TurnStart());
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();

        instance = this;
        turnCount = 0;
        field = new Unit[12][][];
        for (int i = 0; i < 12; i++)
        {
            field[i] = new Unit[11][];
            for (int j = 0; j < 11; j++)
                field[i][j] = new Unit[4];
        }

        SetUnitsRarity(new int[] {1, 1, 2, 3, 4, 5});
        StartCoroutine(MatchStart());
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition) - cameraDiff;

            Vector2 choosePos = new Vector2(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));

            
            if (!IsInSmallBoundary(choosePos))
                return;                     //點擊了場外

            Unit[] cell = field[Mathf.FloorToInt(choosePos.x)][Mathf.FloorToInt(choosePos.y)];
            int i = 0;
            
            while (i < cell.Length && cell[i] == null)
                i++;
            //選擇單位
            if (!selectingAction && i != cell.Length)
            {
                selectedUnit = cell[i];
                if ((selectedUnit.team && playerGreen.thinkingCount > 0)|| (!selectedUnit.team && playerOrange.thinkingCount > 0))
                {
                    //秀出選擇行動
                    selectedUnit.ShowOptions();
                    selectingAction = true;
                }
            }
            //選擇action施放位置
            if (selectingPosition)
            {
                Vector2 diffPos = (choosePos - selectedUnit.position);
                //if (Array.IndexOf( selectedUnit.actionOptions[actionIndex].legelPositions,
                //    diffPos) > -1)
                if(selectedUnit.CheckPossition(actionIndex, choosePos))
                {
                    selectedUnit.InsertAction(actionIndex, diffPos);
                    if (selectedUnit.team)
                        playerGreen.thinkingCount--;
                    else
                        playerOrange.thinkingCount--;
                    //結束選擇
                    selectingPosition = selectingAction = false;
                    selectedUnit.UnshowAvailablePosition();
                    selectedUnit.UnshowOptions();
                    actionIndex = -1;
                }
            }
        }


        if (selectingAction)
        {
            if (Input.anyKeyDown)
            {
                if (selectedUnit.team == true)
                {
                    if (Input.GetKeyDown(KeyCode.Z))
                        actionIndex = 0;
                    else if (Input.GetKeyDown(KeyCode.X))
                        actionIndex = 1;
                    else if (Input.GetKeyDown(KeyCode.C))
                        actionIndex = 2;
                    else if (Input.GetKeyDown(KeyCode.A))
                        actionIndex = 3;
                    else if (Input.GetKeyDown(KeyCode.S))
                        actionIndex = 4;
                    else if (Input.GetKeyDown(KeyCode.D))
                        actionIndex = 5;
                    if(actionIndex != -1)
                    {
                        selectedUnit.ShowAvailablePosition(actionIndex);
                        selectingPosition = true;
                    }
                        
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Keypad1))
                        actionIndex = 0;
                    else if (Input.GetKeyDown(KeyCode.Keypad2))
                        actionIndex = 1;
                    else if (Input.GetKeyDown(KeyCode.Keypad3))
                        actionIndex = 2;
                    else if (Input.GetKeyDown(KeyCode.Keypad4))
                        actionIndex = 3;
                    else if (Input.GetKeyDown(KeyCode.Keypad5))
                        actionIndex = 4;
                    else if (Input.GetKeyDown(KeyCode.Keypad6))
                        actionIndex = 5;
                    if (actionIndex != -1)
                    {
                        selectedUnit.ShowAvailablePosition(actionIndex);
                        selectingPosition = true;
                    }
                }
            }
        }
        
    }
}

