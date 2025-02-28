using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class PlayerTurnInputUIController : MonoBehaviour
{
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Button _attackButton, _defendButton, _spellButton, _runButton;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Transform _attackButtonsHolder, _spellButtonsHolder, _combatantButtonHolder;

    private Dictionary<ICombatant, Button> _combatantButtons = new();
    private List<ICombatant> _battlingCombatant = new();
    private ICombatant _playerCombatant;
    private ICombatMove _choosenMove;

    [Inject]
    private void Construct(ICombatant playerCombatant)
    {
        _playerCombatant = playerCombatant;
    }

    private void Awake()
    {
        _attackButtonsHolder.gameObject.SetActive(false);
        _spellButtonsHolder.gameObject.SetActive(false);
        _combatantButtonHolder.gameObject.SetActive(false);
        InitCombatMoveSelectionButton();
    }

    private void InitCombatMoveSelectionButton()
    {
        foreach (ICombatMove combatMove in _playerCombatant.CombatMoves)
        {
            Transform moveHolder;
            switch (combatMove.MoveType)
            {
                case MoveType.ATTACK:
                    moveHolder = _attackButtonsHolder;
                    break;
                case MoveType.SPELL:
                    moveHolder = _spellButtonsHolder;
                    break;
                default:
                    moveHolder = null;
                    continue;
            }

            InitButton(moveHolder, () =>
            {
                _choosenMove = combatMove;
                _combatantButtonHolder.gameObject.SetActive(true);
                _descriptionText.text = combatMove.Description;
                UpdateCombatantButtonState();
            }, combatMove.MoveName);
        }
    }

    private void OnEnable()
    {
        EventManager.Subscribe<OnCombatStartedMessage>(OnCombatStarted);
        EventManager.Subscribe<OnWaitingPlayerTurnInputMessage>(OnWaitingPlayerTurnInput);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnCombatStartedMessage>(OnCombatStarted);
        EventManager.Unsubscribe<OnWaitingPlayerTurnInputMessage>(OnWaitingPlayerTurnInput);
    }

    private void Start()
    {
        _attackButton.onClick.RemoveAllListeners();
        _spellButton.onClick.RemoveAllListeners();
        _defendButton.onClick.RemoveAllListeners();
        _runButton.onClick.RemoveAllListeners();

        _attackButton.onClick.AddListener(OnAttackButtonClicked);
        _spellButton.onClick.AddListener(OnSpellButtonClicked);
        _defendButton.onClick.AddListener(OnDefendButtonClicked);
        _runButton.onClick.AddListener(OnRunButtonClicked);
    }

    private void OnCombatStarted(OnCombatStartedMessage message)
    {
        _descriptionText.text = string.Empty;
        _battlingCombatant.Clear();
        _battlingCombatant = message.enemies;
        _battlingCombatant.Add(message.player);

        InitCombatantSelectionButton();
    }

    private Button InitButton(Transform parent, UnityAction onClick, string text)
    {
        GameObject buttonObject = GameObject.Instantiate(_buttonPrefab, parent);
        Button button = buttonObject.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);

        TextMeshProUGUI buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = text;

        return button;
    }

    private void InitCombatantSelectionButton()
    {
        _combatantButtons.Clear();
        foreach (Transform child in _combatantButtonHolder)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        foreach (ICombatant combatant in _battlingCombatant)
        {
            Button button = InitButton(_combatantButtonHolder, () => NotifyChoosenMove(_choosenMove, combatant), combatant.Name);
            _combatantButtons.Add(combatant, button);
        }
    }

    private void OnWaitingPlayerTurnInput(OnWaitingPlayerTurnInputMessage message)
    {
        _attackButton.interactable = message.IsTurnInputAllowed;
        _defendButton.interactable = message.IsTurnInputAllowed;
        _spellButton.interactable = message.IsTurnInputAllowed;
        _runButton.interactable = message.IsTurnInputAllowed;
    }

    private void UpdateCombatantButtonState()
    {
        foreach (KeyValuePair<ICombatant, Button> item in _combatantButtons)
        {
            bool isInteractable = false;
            switch (_choosenMove.TargetType)
            {
                case TargetType.SELF:
                    // DO: Activate player button only if the target is self
                    isInteractable = item.Key.Name == "Player";
                    break;
                case TargetType.ALLY:
                    isInteractable = item.Key.Name == "Player";
                    break;
                case TargetType.ENEMY:
                    isInteractable = item.Key.Name != "Player";
                    break;
                case TargetType.ANY:
                    isInteractable = true;
                    break;

            }
            item.Value.interactable = isInteractable;
        }
    }

    private void OnAttackButtonClicked()
    {
        // DO: Show list of button to show the attack moves
        _descriptionText.text = string.Empty;
        _attackButtonsHolder.gameObject.SetActive(true);
        _spellButtonsHolder.gameObject.SetActive(false);
        _combatantButtonHolder.gameObject.SetActive(false);
    }

    private void OnSpellButtonClicked()
    {
        // DO: Show list of button to show the spell moves
        _descriptionText.text = string.Empty;
        _attackButtonsHolder.gameObject.SetActive(false);
        _spellButtonsHolder.gameObject.SetActive(true);
        _combatantButtonHolder.gameObject.SetActive(false);
    }

    private void OnDefendButtonClicked()
    {
        // DO: Choose the first defense move
        _descriptionText.text = string.Empty;
        NotifyChoosenMove(_playerCombatant.CombatMoves.First(move => move.MoveType == MoveType.DEFEND), _playerCombatant);
    }

    private void OnRunButtonClicked()
    {
        // DO: Choose the first run move
        _descriptionText.text = string.Empty;
        NotifyChoosenMove(_playerCombatant.CombatMoves.First(move => move.MoveType == MoveType.RUN), _playerCombatant);
    }

    private void NotifyChoosenMove(ICombatMove choosenCombatMove, ICombatant target)
    {
        EventManager.Publish<OnPlayerMoveChoosenMessage>(new() { Move = choosenCombatMove });
    }
}