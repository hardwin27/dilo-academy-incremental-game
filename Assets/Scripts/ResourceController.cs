﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public Button ResourceButton;
    public Image ResourceImage;
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;

    private ResourceConfig _config;

    private int _level = 1;

    public bool isUnlocked { get; private set; }

    public double GetOutput()
    {
        return _config.Output * _level;
    }

    public double GetUnlockCost()

    {
        return _config.UnlockCost;
    }

    public double GetUpgradeCost()
    {
        return _config.UpgradedCost * _level;
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        ResourceImage.color = isUnlocked ? Color.white : Color.grey;
        ResourceUnlockCost.gameObject.SetActive(!unlocked);
        ResourceUpgradeCost.gameObject.SetActive(unlocked);
    }

    public void UnlockResource()
    {
        double unlockCost = GetUnlockCost();
        if (GameManager.Instance.TotalGold < unlockCost)
        {
            return;
        }
        SetUnlocked(true);
        GameManager.Instance.ShowNextResourcec();
        AchievementController.Instance.UnlockAchievement(AchievementType.UnlockResource, _config.Name);
    }

    public void SetConfig(ResourceConfig config)
    {
        _config = config;

        //ToString("0") makes no decimal
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput().ToString("0") }";
        ResourceUnlockCost.text = $"Unlock Cost\n{ _config.UnlockCost }";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";

        SetUnlocked(_config.UnlockCost == 0);
    }

    public void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();
        if(GameManager.Instance.TotalGold < upgradeCost)
        {
            return;
        }

        GameManager.Instance.AddGold(-upgradeCost);
        _level++;
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput().ToString("0") }";
    }

    private void Start()
    {
        ResourceButton.onClick.AddListener(() =>
        {
            if(isUnlocked)
            {
                UpgradeLevel();
            }
            else
            {
                UnlockResource();
            }
        });
    }
}

