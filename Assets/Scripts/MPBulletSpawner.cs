using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class MPBulletSpawner :NetworkBehaviour
{
    public Rigidbody bullet;
    public Transform bulletPOS;
    private float bulletSpeed = 10f;
    


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && IsLocalPlayer)
        {
            FireServerRpc();
        }
    }

    [ServerRpc]

    private void FireServerRpc()
    {
        Debug.Log("Fired weapon");
        Rigidbody bulletClone = Instantiate(bullet, transform.position, transform.rotation);
        bulletClone.velocity = transform.forward * bulletSpeed;
        bulletClone.gameObject.GetComponent<NetworkObject>().Spawn();
        Destroy(bulletClone.gameObject, 3);
    }
}
