using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Telescope : Unit
{
    public int exposedIndex = 9999; //�p�G�b��^�X�Q½�}�h�ĪG���i�o��
    public override void SetToExposed()
    {
        base.SetToExposed();
        exposedIndex = GameManager.instance.phaseIndex;
    }
    public override void SetToCovered()
    {
        base.SetToCovered();
        exposedIndex = 9999;
    }
    public override void ShowOptions()
    {
        Vector2 p = team ? new Vector2(-1036, 399) : new Vector2(1036, 399);

        string inputList = team ? inputListGreen : inputListOrange;
        int inputIndex = 0;
        foreach (ActionOption a in actionOptions)
        {
            GameObject textCard = Instantiate(textPrefab);
            textCard.transform.SetParent(FindObjectOfType<Canvas>().transform);
            textCard.GetComponent<RectTransform>().localPosition = p;
            textCard.GetComponent<RectTransform>().localScale = Vector3.one;
            Text t = textCard.GetComponentInChildren<Text>();
            t.text = inputList[inputIndex++] + ": " + a.name;
            if(a.name == "���d")
            {
                if (!isCovered)
                    t.text += "(�L�k�ϥ�)";
            }

            shownOptions.Enqueue(textCard);//�N�r�d�[�Jqueue

            p.y -= 125;
        }
    }
    public override bool CheckPossition(int actionIndex, Vector2 pos)
    {
        if(actionIndex == 2)
        {
            if (!isCovered)
                return false;
            return (GameManager.IsInSmallBoundary(pos));
        }
        //��L��Ӧ�ʨS������
        return base.CheckPossition(actionIndex, pos);
    }
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
            case 2:                 //���d
                UnshowAvailablePosition();
                showPosParent = new GameObject();
                showPosParent.transform.parent = transform;
                showPosParent.transform.localPosition = Vector3.zero;
                if (!isCovered)
                    return;
                foreach (Vector2 pos in actionOptions[actionIndex].legelPositions)
                {
                    if (CheckPossition(actionIndex, pos + position))
                    {
                        GameObject s = Instantiate(GameManager.instance.selectors[0], showPosParent.transform);
                        s.transform.localPosition = pos;
                    }
                }
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
            case 2:
                a.type = "attacking";
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
            case 2:
                if(GameManager.instance.phaseIndex > exposedIndex)
                {
                    Debug.Log("���d�Q���_�F!!");
                    return;
                }
                foreach (Vector2 pos in actionOptions[2].legelPositions)
                {
                    Vector2 targetPos = pos + position;
                    if (GameManager.IsInSmallBoundary(targetPos))
                    {
                        Unit[] cell = GameManager.instance.GetCellUnits(targetPos);
                        int i = 0;
                        while(i < cell.Length && cell[i] != null)
                        {
                            if(cell[i].team != team)
                            {
                                cell[i].SetToExposed();
                            }
                            i++;
                        }
                    }
                    SetToExposed();
                }
                break;
            case 999:
                BasicDie();
                break;
        }
    }
}
