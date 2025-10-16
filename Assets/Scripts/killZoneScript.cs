using UnityEngine;

public class killZoneScript : MonoBehaviour
{
    [SerializeField] private PlayerSpawnScript playerSpawner;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Entered KillZone");
            playerSpawner.ResetPlayer();
        }
        else
        {
            Debug.Log(other + " Entered KillZone");
        }
    }
}
