using UnityEngine;
using Zenject;

public class CombatSceneInstaller : MonoInstaller {
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private PlayerCombatant _playerCombatant;
    public override void InstallBindings()
    {
        Container.Bind<GameManager>().FromInstance(_gameManager).AsSingle();
        Container.Bind<ICombatant>().FromInstance(_playerCombatant).AsSingle();
    }
}