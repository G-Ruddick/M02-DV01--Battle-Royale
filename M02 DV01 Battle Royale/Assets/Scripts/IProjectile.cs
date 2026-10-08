using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class IProjectile : MonoBehaviourPun {
    public Rigidbody rig;

    public virtual void Initialize(int damage, int attackerId, bool isMine) {}
}
