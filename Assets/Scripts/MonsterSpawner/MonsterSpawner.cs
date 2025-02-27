using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private Transform _enemiesTransform;
    [SerializeField] private Bounds _spawningArea;

    [SerializeField] private bool _isConstantSpawning = true;
    [SerializeField] private int _spawnInterval, _minInterval, _decrementInterval;
    [SerializeField] private int _maxSpawnedMonster = 10;
    [SerializeField] private int _currentSpawnInterval;
    [SerializeField] private bool _canSpawn = true;

    private List<GameObject> _spawnedMonsters = new();


    private void Awake()
    {
        for (int i = 0; i < _enemiesTransform.childCount; i++)
        {
            _spawnedMonsters.Add(_enemiesTransform.GetChild(i).gameObject);
        }
    }

    private void OnEnable()
    {
        EventManager.Subscribe<OnBattlingCombatMessage>(OnBattlingCombat);
    }


    private void OnDisable()
    {
        EventManager.Unsubscribe<OnBattlingCombatMessage>(OnBattlingCombat);
    }

    private void OnBattlingCombat(OnBattlingCombatMessage message)
    {
        _canSpawn = message.State == CombatState.END;
    }

    private void Start()
    {
        StartMonsterSpawning().Forget();
    }

    private async UniTaskVoid StartMonsterSpawning()
    {
        _currentSpawnInterval = _spawnInterval;

        while (_spawnedMonsters.Count < _maxSpawnedMonster)
        {
            await UniTask.WaitForSeconds(_currentSpawnInterval);
            await UniTask.WaitUntil(() => _canSpawn);
            SpawnMonster();
            if (!_isConstantSpawning && _currentSpawnInterval > _minInterval)
            {
                _currentSpawnInterval = Mathf.Clamp(_currentSpawnInterval - _decrementInterval, _minInterval, _currentSpawnInterval);
            }
        }
    }

    private void SpawnMonster()
    {
        float randomX = Random.Range(_spawningArea.min.x, _spawningArea.max.x);
        float randomY = Random.Range(_spawningArea.min.y, _spawningArea.max.y);
        Vector3 randomPosition = new(randomX, randomY);
        GameObject spawnedMonster = GameObject.Instantiate(GetRandomMonster(), randomPosition, Quaternion.identity, _enemiesTransform);
        _spawnedMonsters.Add(spawnedMonster);
    }

    private GameObject GetRandomMonster()
    {
        return _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
    }
}