using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _explorationCanvas, _combatCanvas;
    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
    }

    private void Init()
    {
        _combatCanvas.alpha = 0f;
    }

    private void OnEnable()
    {
        EventManager.Subscribe<OnBattlingCombatMessage>(OnCombatBattling);
    }

    void OnDisable()
    {
        EventManager.Subscribe<OnBattlingCombatMessage>(OnCombatBattling);
    }

    private void OnCombatBattling(OnBattlingCombatMessage message)
    {
        _combatCanvas.alpha = message.State == CombatState.END ? 0f : 1f;
    }
}