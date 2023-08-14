using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public int playerNo;
    private static Dictionary<int, MoneyManager> instances = new Dictionary<int, MoneyManager>();
    public Text text;

    public float reservedMoney;

    [SerializeField]
    private float money;

    [SerializeField]
    private float baseIncome;

    [SerializeField]
    private float baseInterval;

    private float time = 0;

    public float Money { private set { money = value; } get { return money; } }

    public static MoneyManager Instance(int playerNo) => instances[playerNo];
    //{
    //    return instances[playerNo];
    //}

    private void Awake()
    {
        money = GameStats.Instance.GetPlayer(playerNo).GetStartGold();
        instances.Add(playerNo, this);
        UpdateText();
    }

    private void FixedUpdate()
    {
        if (time >= baseInterval)
        {
            Instance(playerNo).AddMoney(baseIncome);
            time = 0;
        }

        time += Time.deltaTime;
    }

    //public static MoneyManager Instance(int playerNo) 
    //{
    //    return instances[playerNo];
    //}

    public void AddMoney(float quantity)
    {
        if (quantity < 0)
            throw new Exception("AddMoney(quantity<0)");

        Money += quantity;

        if (Money == float.NegativeInfinity)
            Debug.Log("NegInf");
        else if (Money == float.PositiveInfinity)
            Debug.Log("PosInf");

        UpdateText();        
    }

    public bool ReserveMoney(float quantity)
    {
        if (reservedMoney != 0) return false; 

        reservedMoney = quantity;
        Money -= quantity;

        UpdateText();

        return true;
    }

    public void PayReservedMoney()
    {
        reservedMoney = 0;

        UpdateText();
    }

    public void ReturnReservedMoney()
    {
        Money += reservedMoney;
        reservedMoney = 0;

        UpdateText();
    }

    public bool WithdrawMoney(float quantity)
    {
        if (quantity < 0)
            throw new Exception("WithdrawMoney(quantity<0)");

        if (Money < quantity)
            return false;

        Money -= quantity;

        if (Money == float.NegativeInfinity)
            Debug.Log("NegInf");
        else if (Money == float.PositiveInfinity)
            Debug.Log("PosInf");

        UpdateText();
        return true;
    }


    void UpdateText()
    {
        if (text == null)
            return;

        text.text = Money.ToString("0") + "с";
    }

    private void OnDestroy()
    {
        instances.Clear();
    }

}
