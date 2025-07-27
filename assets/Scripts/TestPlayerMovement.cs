using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    private PlayerMovement playerMovement;
    
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            Debug.Log("PlayerMovement найден!");
            Debug.Log("IsDashing доступен: " + playerMovement.IsDashing());
        }
        else
        {
            Debug.LogError("PlayerMovement НЕ найден!");
        }
    }
} 