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
        EventManager.Subscribe<OnTriggerCombatMessage>(OnCombatTriggered);
        EventManager.Subscribe<OnCombatEndedMessage>(OnCombatEnded);
    }

    void OnDisable()
    {
        EventManager.Unsubscribe<OnTriggerCombatMessage>(OnCombatTriggered);
        EventManager.Unsubscribe<OnCombatEndedMessage>(OnCombatEnded);
    }

    private void OnCombatTriggered(OnTriggerCombatMessage message)
    {
        _combatCanvas.alpha = 1f;
    }
    
    private void OnCombatEnded(OnCombatEndedMessage message)
    {
        _combatCanvas.alpha = 0f;
    }
}