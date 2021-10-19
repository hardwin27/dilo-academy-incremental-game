﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    [Range(0f, 1f)] public float AutoCollectPercecntage = 0.1f;
    public ResourceConfig[] ResourceConfigs;
    public Sprite[] ResourceSprites;
    public Transform ResourceParent;
    public ResourceController ResourcePrefarbs;
    public TapText TapTextPrefab;

    public Transform CoinIcon;
    public Text GoldInfo;
    public Text AutoCollectInfo;

    private List<ResourceController> _activeResources = new List<ResourceController>();
    private List<TapText> _tapTextPool = new List<TapText>();
    private float _collectSecond;
    public double TotalGold { get; private set; }

    private void AddAllResources()
    {
        bool showResources = true;
        foreach(ResourceConfig config in ResourceConfigs)
        {
            GameObject obj = Instantiate(ResourcePrefarbs.gameObject, ResourceParent, false);
            ResourceController resource = obj.GetComponent<ResourceController>();

            resource.SetConfig(config);
            obj.gameObject.SetActive(showResources);
            if(showResources && !resource.isUnlocked)
            {
                showResources = false;
            }
            _activeResources.Add(resource);
        }
    }

    public void AddGold(double value)

    {
        TotalGold += value;
        GoldInfo.text = $"Gold: { TotalGold.ToString("0") }";
    }

    private void CollectPerSecond()
    {
        double output = 0;
        foreach(ResourceController resource in _activeResources)
        {
            if(resource.isUnlocked)
            {
                output += resource.GetOutput();
            }
        }

        output *= AutoCollectPercecntage;
        //ToString("F1") for 1 decimal place
        AutoCollectInfo.text = $"Auto Collect: { output.ToString("F1") } / second";
        AddGold(output);
    }

    private TapText GetOrCreateTapText()
    {
        TapText tapText = _tapTextPool.Find(t => !t.gameObject.activeSelf);
        if(tapText == null)
        {
            tapText = Instantiate(TapTextPrefab).GetComponent<TapText>();
            _tapTextPool.Add(tapText);
        }

        return tapText;
    }
    
    public void CollectByTap(Vector3 tapPosition, Transform parent)
    {
        double output = 0;
        foreach(ResourceController resource in _activeResources)
        {
            if(resource.isUnlocked)
            {
                output += resource.GetOutput();
            }
        }

        TapText tapText = GetOrCreateTapText();
        tapText.transform.SetParent(parent, false);
        tapText.transform.position = tapPosition;

        tapText.Text.text = $"+{ output.ToString("0") }";
        tapText.gameObject.SetActive(true);
        CoinIcon.transform.localScale = Vector3.one * 1.75f;

        AddGold(output);
    }

    private void CheckResourceCost()
    {
        foreach(ResourceController resource in _activeResources)
        {
            bool isBuyable = TotalGold >= resource.GetUpgradeCost();
            if(resource.isUnlocked)
            {
                isBuyable = TotalGold >= resource.GetUpgradeCost();
            }
            else
            {
                isBuyable = TotalGold >= resource.GetUnlockCost();
            }
            resource.ResourceImage.sprite = ResourceSprites[isBuyable ? 1 : 0];
        }
    }

    public void ShowNextResourcec()
    {
        foreach(ResourceController resource in _activeResources)
        {
            if(!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive(true);
                break;
            }
        }
    }

    private void Start()
    {
        AddAllResources();
    }

    private void Update()
    {
        _collectSecond += Time.unscaledDeltaTime;
        if(_collectSecond >= 1f)
        {
            CollectPerSecond();
            _collectSecond = 0f;
        }

        CheckResourceCost();

        CoinIcon.transform.localScale = Vector3.LerpUnclamped(CoinIcon.transform.localScale, Vector3.one * 2f, 0.15f);
        CoinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
    }
}
//Serializable so it can be serialize and value can be set from inspector
[System.Serializable]
public struct ResourceConfig
{
    public string Name;
    public double UnlockCost;
    public double UpgradedCost;
    public double Output;
}
