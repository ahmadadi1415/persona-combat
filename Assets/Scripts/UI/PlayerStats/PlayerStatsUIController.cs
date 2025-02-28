using TMPro;
using UnityEngine;

public class PlayerStatsUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerStatsText, _enemyStatsText;

    private void Awake() {
        _enemyStatsText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Subscribe<OnBattlingCombatMessage>(OnBattlingCombat);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnBattlingCombatMessage>(OnBattlingCombat);
    }

    private void OnCombatTriggered(OnTriggerCombatMessage message)
    {
        // ICombatant player = message.CombatCharacters.First(combatant => combatant.Name == "Player");
        // ICombatant enemy = message.CombatCharacters.First(combatant => combatant.Name != "Player");
        // UpdateStats(player, enemy);
    }

    private void OnBattlingCombat(OnBattlingCombatMessage message)
    {
        ICombatant player = message.PlayerCombatant;
        UpdateStats(player);
    }

    private void UpdateStats(ICombatant player)
    {
        _playerStatsText.text = $"{player.Name} \nHealth\t:{player.Health} \nPower\t:{player.Power} \nDefense\t:{player.Defense} \nSpeed\t:{player.Speed}(x{player.SpeedModifier})";
    }
}