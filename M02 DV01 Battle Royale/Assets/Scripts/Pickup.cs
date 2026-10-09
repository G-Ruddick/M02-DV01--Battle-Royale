using UnityEngine;
using Photon.Pun;

public enum PickupType { Health, Ammo, Launchpad }

public class Pickup : MonoBehaviour {
    public PickupType type;
    public int value;

    private void OnTriggerEnter(Collider other) {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }

        if (other.CompareTag("Player")) {
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);

            if (type == PickupType.Health) {
                player.photonView.RPC("Heal", player.photonPlayer, value);
            }
            else if (type == PickupType.Ammo) {
                player.activeWeapon.photonView.RPC("GiveAmmo", player.photonPlayer, value);
            }
            else if (type == PickupType.Launchpad) {
                player.photonView.RPC("LaunchPlayer", player.photonPlayer, player.transform.position + Vector3.down, (float)value);
            }

            PhotonNetwork.Destroy(gameObject);
        }
    }

}