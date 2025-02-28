using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsUIController : MonoBehaviour
{
    [SerializeField] private GameObject _statsTextPrefab;

    private Dictionary<ICombatant, TextMeshProUGUI> _statsTexts = new();

    private void OnEnable()
    {
        EventManager.Subscribe<OnCombatStartedMessage>(OnCombatStarted);
        EventManager.Subscribe<OnBattlingCombatMessage>(OnBattlingCombat);
        EventManager.Subscribe<OnCombatFinishedMessage>(OnCombatFinished);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnCombatStartedMessage>(OnCombatStarted);
        EventManager.Unsubscribe<OnBattlingCombatMessage>(OnBattlingCombat);
        EventManager.Unsubscribe<OnCombatFinishedMessage>(OnCombatFinished);
    }

    private void OnCombatStarted(OnCombatStartedMessage message)
    {
        Reset();

        InitText(message.Player);
        foreach (ICombatant enemy in message.Enemies)
        {
            InitText(enemy);
        }
    }

    private void OnBattlingCombat(OnBattlingCombatMessage message)
    {
        if (message.State != CombatState.END)
        {
            foreach (KeyValuePair<ICombatant, TextMeshProUGUI> stats in _statsTexts)
            {
                Debug.Log("Update stats from battling");
                UpdateStats(stats.Key, stats.Value);
            }
        }
        else
        {
            Reset();
        }
    }

    private void OnCombatFinished(OnCombatFinishedMessage message)
    {
        Reset();
    }

    private void Reset()
    {
        _statsTexts.Clear();

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }

    private void UpdateStats(ICombatant combatant, TextMeshProUGUI statsText)
    {
        statsText.text = $"{combatant.Name} \nHealth\t:{combatant.Health}/{combatant.MaxHealth} \nPower\t:{combatant.Power} \nDefense\t:{combatant.Defense} \nSpeed\t:{combatant.Speed}(x{combatant.SpeedModifier})";
    }

    private void InitText(ICombatant combatant)
    {
        GameObject textObject = GameObject.Instantiate(_statsTextPrefab, transform);
        TextMeshProUGUI statsText = textObject.GetComponent<TextMeshProUGUI>();
        UpdateStats(combatant, statsText);

        if (!_statsTexts.ContainsKey(combatant)) _statsTexts.Add(combatant, statsText);
    }
}