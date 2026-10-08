using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class Rocket : IProjectile {
    public Explosion explosion;
    public bool explode = false;

    public override void Initialize (int damage, int attackerId, bool isMine) {
        explosion.damage = damage;
        explosion.attackerId = attackerId;
        explosion.isMine = isMine;
        explosion.gameObject.SetActive(false);
        Destroy(gameObject, 10.0f);
    }

    private void OnTriggerEnter (Collider other) {
        photonView.RPC("RunEnumerator", RpcTarget.All);
        // RunEnumerator();
        Destroy(rig);
    }

    [PunRPC]
    private void RunEnumerator() {
        StartCoroutine(Expand());
    }

    private IEnumerator Expand() {
        explosion.gameObject.SetActive(true);

        while (explosion.transform.localScale.x < 3.0f) {
            explosion.transform.localScale += new Vector3(1, 1, 1) * 0.05f;
            yield return 0;
        }

        Destroy(this.gameObject);
    }
}
