using UnityEngine;

public class PlayerMovementSimple : MonoBehaviour
{
    private bool isDashing = false;
    
    public bool IsDashing()
    {
        return isDashing;
    }
    
    public void SetDashing(bool dashing)
    {
        isDashing = dashing;
    }
} 