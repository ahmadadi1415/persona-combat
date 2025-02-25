using UnityEngine;

public enum GameState { EXPLORATION, COMBAT }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [field: SerializeField] public GameState CurrentGameState { get; private set; } = GameState.EXPLORATION;
    
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
        }
    }

}