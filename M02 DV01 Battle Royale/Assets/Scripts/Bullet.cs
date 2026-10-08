using UnityEngine;

public class Bullet : IProjectile {
    private int damage;
    private int attackerId;
    private bool isMine;

    public override void Initialize (int damage, int attackerId, bool isMine) {
        this.damage = damage;
        this.attackerId = attackerId;
        this.isMine = isMine;
        Destroy(gameObject, 5.0f);
    }

    private void OnTriggerEnter (Collider other) {
        if(other.CompareTag("Player") && isMine) {
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);
            
            if(player.id != attackerId) {
                player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
            }
        }
        
        Destroy(gameObject);
    }
}
