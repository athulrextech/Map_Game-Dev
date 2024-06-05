using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    private GameManager _gameManager;

    public GameManager GameManager { get { return _gameManager; } }

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }


}
