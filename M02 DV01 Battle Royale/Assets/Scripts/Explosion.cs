using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour {
    public int damage;
    public int attackerId;
    public bool isMine;

    List<int> playersChecked = new List<int>();

    private void OnTriggerEnter (Collider other) {        
        if(other.CompareTag("Player") && isMine) {
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);
            
            if (IsPlayerAlreadyHit(player.id)) {
                player.photonView.RPC("LaunchPlayer", player.photonPlayer, this.gameObject.transform.position, 10);

                if(player.id != attackerId) {
                    playersChecked.Add(player.id);
                    player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
                }
            }
        }
    }

    private bool IsPlayerAlreadyHit(int playerID) {
        foreach (int id in playersChecked) {
            if (id == playerID) {
                return false;
            }
        }

        return true;
    }
}
