using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    
    public bool isCovered, dead;
    public Vector2 position;
    public bool team;       //green == true
    public bool waitingForPick;

    public Sprite coveredOrange, coveredGreen, exposedOrange, exposedGreen;

    public Action definingAction;

    //顯示行動
    [SerializeField] protected GameObject textPrefab;
    protected string inputListGreen = "zxcasdqwe";
    protected string inputListOrange = "123456789";
    protected GameObject showPosParent;
    protected Queue<GameObject> shownOptions = new Queue<GameObject>();
    [Serializable]
    public class ActionOption 
    {
        //用於方便辨識
        public string name;
        public bool byPosition, byDirection;
        //相對位置
        public Vector2[] legelPositions;

        public virtual bool CheckAvailable(Vector2 pos)
        {
            return true;
        }

    }
    public ActionOption[] actionOptions;
    //每個單位都要override此函數
    public virtual void InsertAction(int actionIndex, Vector2 diffPos)
    {
        Action a = new Action();
        GameManager.instance.InsertAcition(a);
    }

    public void SetTeam(bool t)
    {
        team = t;
        dead = false;
        if (isCovered)
        {
            if(team)
                GetComponent<SpriteRenderer>().sprite = coveredGreen;
            else
                GetComponent<SpriteRenderer>().sprite = coveredOrange;
        }
        else
        {
            if (team)
                GetComponent<SpriteRenderer>().sprite = exposedGreen;
            else
                GetComponent<SpriteRenderer>().sprite = exposedOrange;
        }
    }

    public void WaitForPick()
    {
        if (!team)
            GetComponent<SpriteRenderer>().sprite = exposedOrange;
        waitingForPick = true;
    }
    public virtual void ShowOptions()
    {
        Vector2 p = team ? new Vector2(-1036, 399): new Vector2(1036, 399);
        
        string inputList = team ? inputListGreen : inputListOrange;
        int inputIndex = 0;
        foreach (ActionOption a in actionOptions)
        {
            GameObject textCard = Instantiate(textPrefab);
            textCard.transform.SetParent ( FindObjectOfType<Canvas>().transform);
            textCard.GetComponent<RectTransform>().localPosition = p;
            textCard.GetComponent<RectTransform>().localScale = Vector3.one;
            Text t = textCard.GetComponentInChildren<Text>();
            t.text = inputList[inputIndex++] + ": "+ a.name;

            shownOptions.Enqueue(textCard);//將字卡加入queue

            p.y -= 125;
        }
    }
    public void UnshowOptions()
    {
        while (shownOptions.Count > 0)
            Destroy( shownOptions.Dequeue()); //將字卡移除queue並Destroy
    }

    public virtual bool CheckPossition(int actionIndex, Vector2 pos)
    {
        switch(actionIndex)
        {
            case 0:
                return (GameManager.IsInSmallBoundary(pos));
            case 1:
                return CheckMovingPosition(actionIndex, pos);
        }
        return false;
    }
    public void ShowAttackingPosition(int actionIndex)
    {
        UnshowAvailablePosition();
        showPosParent = new GameObject();
        showPosParent.transform.parent = transform;
        showPosParent.transform.localPosition = Vector3.zero;
        if (actionOptions[actionIndex].byPosition)
            foreach (Vector2 pos in actionOptions[actionIndex].legelPositions)
            {
                if (CheckPossition(actionIndex, pos + position))
                {
                    GameObject s = Instantiate(GameManager.instance.selectors[0], showPosParent.transform);
                    s.transform.localPosition = pos;
                }

            }
    }
    public void ShowMovingPosition(int actionIndex)
    {
        UnshowAvailablePosition();
        showPosParent = new GameObject();
        showPosParent.transform.parent = transform;
        showPosParent.transform.localPosition = Vector3.zero;
        if (actionOptions[actionIndex].byPosition)
            foreach (Vector2 pos in actionOptions[actionIndex].legelPositions)
            {
                if (CheckPossition(actionIndex, pos + position))
                {
                    GameObject s = Instantiate(GameManager.instance.selectors[0], showPosParent.transform);
                    s.transform.localPosition = pos;
                }

            }
    }
    public virtual void ShowAvailablePosition(int actionIndex)
    {
        
    }
    public virtual bool CheckMovingPosition(int actionIndex, Vector2 pos)
    {
        if (Array.IndexOf(actionOptions[actionIndex].legelPositions, pos - position) == -1 || !GameManager.IsInSmallBoundary(pos))
        {
            return false;
        }
        Unit[] cell = GameManager.instance.GetCellUnits(pos);
        bool overlapping = false;
        foreach (Unit u in cell)
            if (u != null)
                overlapping = true;
        if (!overlapping)
            return true;
        return false;
    }
    public void UnshowAvailablePosition()
    {
        if (showPosParent != null)
            Destroy(showPosParent);
    }
    public virtual void SetToCovered()
    {
        GetComponent<SpriteRenderer>().sprite = (team)? coveredGreen: coveredOrange;
        isCovered = true;
    }
    public virtual void SetToExposed()
    {
        GetComponent<SpriteRenderer>().sprite = (team) ? exposedGreen : exposedOrange;
        isCovered = false;
    }
    public void BasicMove(Vector2 where)
    {
        if (dead)
            return;
        GameManager.instance.UnregisterPosition(this, position);
        position = where;
        GameManager.instance.RegisterPosition(this, position);
        transform.position = GameManager.ToCameraPosition(position);
    }
    public void BasicDie()
    {
        SetToExposed();
        GameManager.instance.UnregisterPosition(this, position);
        float y = 4.5f - 0.5f * (team ? GameManager.instance.playerGreen.deadUnitCount++ : 1 * GameManager.instance.playerOrange.deadUnitCount++);
        this.transform.position = new Vector3((team) ? -8.5f : 8.5f, y, 0);
        dead = true;
    }
    public virtual void PerformAction (Unit to, int index, Vector2[] parameters)
    {

    }
}
