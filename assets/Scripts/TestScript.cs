using UnityEngine;

/// <summary>
/// Простой тестовый скрипт для проверки видимости в Unity
/// </summary>
public class TestScript : MonoBehaviour
{
    [SerializeField] private string testMessage = "Тест работает!";
    
    void Start()
    {
        Debug.Log($"TestScript: {testMessage}");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log($"TestScript: Кнопка T нажата! {testMessage}");
        }
    }
} 