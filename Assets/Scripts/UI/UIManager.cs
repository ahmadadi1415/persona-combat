using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _explorationCanvas, _combatCanvas, _gameOverCanvas;
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
        _gameOverCanvas.alpha = 0f;
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

    private void OnGameOver()
    {
        _gameOverCanvas.alpha = 1f;
    }

    private void OnGameStarted()
    {
        _gameOverCanvas.alpha = 0f;
    }
}