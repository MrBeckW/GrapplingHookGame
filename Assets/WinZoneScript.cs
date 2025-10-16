using UnityEngine;

public class WinZoneScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Entered WinZone");
        }
        else
        {
            Debug.Log(other + " Entered WinZone");
        }
    }
}
