using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour {
    public int damage;
    public int attackerId;
    public bool isMine;

    List<int> playersChecked = new List<int>();

    private void OnTriggerEnter (Collider other) {
        Debug.Log(damage);  
        if(other.CompareTag("Player") && isMine) {
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);
            
            if (IsPlayerAlreadyHit(player.id)) {
                player.photonView.RPC("LaunchPlayer", player.photonPlayer, this.gameObject.transform.position, 10.0f);

                if(player.id != attackerId) {
                    playersChecked.Add(player.id);
                    player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
                }
                else {
                    player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage / 4);
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
