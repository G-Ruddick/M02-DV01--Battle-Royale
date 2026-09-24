using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun {
    [Header("Stats")]
    public float moveSpeed;
    public float jumpForce;

    [Header("Gun")]
    public int curHp;
    public int maxHp;
    public int kills;
    public bool dead;
    private bool flashingDamage;
    public MeshRenderer mr;

    [Header("Components")]
    public Rigidbody rig;
    public int id;
    public Player photonPlayer;
    private int curAttackId;
    public PlayerWeapon weapon;

    private void Update() {
        if (!photonView.IsMine || dead) {
            return;
        }

        Move();

        if (Input.GetKeyDown(KeyCode.Space)) {
            TryJump();
        }
        
        if (Input.GetMouseButtonDown(0)) {
            weapon.TryShoot();
        }
    }

    private void Move() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 dir = (transform.forward * z + transform.right * x) * moveSpeed;
        dir.y = rig.linearVelocity.y;
        rig.linearVelocity = dir;
    }

    private void TryJump() {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 1.5f)) {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    [PunRPC]
    public void Initialize(Player player) {
        id = player.ActorNumber;
        photonPlayer = player;

        GameManager.instance.players[id - 1] = this;

        if (!photonView.IsMine) {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = true;
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerId, int damage) {
        if (dead) {
            return;
        }

        curHp -= damage;
        curAttackId = attackerId;

        photonView.RPC("DamageFlash", RpcTarget.Others);

        if (curHp <= 0) {
            photonView.RPC("Die", RpcTarget.All);
        }
    }

    [PunRPC]
    private void DamageFlash() {
        if (flashingDamage) {
            return;
        }

        StartCoroutine(DamageFlashCoroutine());

        IEnumerator DamageFlashCoroutine() {
            flashingDamage = true;

            Color defaultColor = mr.material.color;
            mr.material.color = Color.red;

            yield return new WaitForSeconds(0.05f);

            mr.material.color = defaultColor;
            flashingDamage = false;
        }
    }

    [PunRPC]
    private void Die() {
        curHp = 0;
        dead = true;
        GameManager.instance.alivePlayers--;
        
        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.CheckWinCondition();
        
        if (photonView.IsMine) {
            if (curAttackId != 0)
                GameManager.instance.GetPlayer(curAttackId).photonView.RPC("AddKill", RpcTarget.All);
            
            GetComponentInChildren<CameraController>().SetAsSpectator();

            rig.isKinematic = true;
            transform.position = new Vector3(0, -50, 0);
        }
    }

    [PunRPC]
    public void AddKill () {
        kills++;
    }
}