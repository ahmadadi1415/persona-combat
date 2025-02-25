using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnInputUIController : MonoBehaviour
{
    [SerializeField] private Button _attackButton, _defendButton, _spellButton, _runButton;

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

    private void OnAttackButtonClicked()
    {
        // DO: Notify the PlayerCombatant to set the attack move
        MoveData attackMove = new()
        {
            MoveType = MoveType.ATTACK,
            Power = 1
        };
        NotifyChoosenMove(attackMove);
    }

    private void OnSpellButtonClicked()
    {
        // DO: Notify the PlayerCombatant to set the spell move
        MoveData spellMove = new()
        {
            MoveType = MoveType.SPELL,
            Power = 10f
        };
        NotifyChoosenMove(spellMove);
    }

    private void OnDefendButtonClicked()
    {
        // DO: Notify the PlayerCombatant to set the defend move
        MoveData spellMove = new()
        {
            MoveType = MoveType.DEFEND,
            Power = 0.25f
        };
        NotifyChoosenMove(spellMove);

    }

    private void OnRunButtonClicked()
    {
        // DO: Notify the PlayerCombatant to set the run move
        MoveData spellMove = new()
        {
            MoveType = MoveType.RUN,
            Power = 0
        };
        NotifyChoosenMove(spellMove);
    }

    private void NotifyChoosenMove(MoveData moveData)
    {
        EventManager.Publish<OnPlayerMoveChoosenMessage>(new() { Move = moveData });
    }
}