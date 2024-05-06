using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataToStore : MonoBehaviour
{
    public static DataToStore instance;

    public Text coinsCountText;

    public int coinsCount;

    //Singleton pattern
    private void Awake()
    {
        if (instance != null)
        {
           Debug.LogWarning("More than one instance of DataToStore found!");
           return;
        }
        instance = this;
    }

    public void AddCoins(int count)
    {
        coinsCount += count;
        coinsCountText.text = coinsCount.ToString();
    }
}
