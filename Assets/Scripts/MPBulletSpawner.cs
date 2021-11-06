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
        if (Input.GetButtonDown("Fire1") && IsOwner)
        {
            FireServerRpc(bulletSpeed, gameObject.GetComponent<MP_PlayerAttribs>().powerUp.Value);
            //FireServerRpc(bulletSpeed);
        }
    }

    [ServerRpc]

    private void FireServerRpc(float speed, bool powerUp, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Fired weapon");
        Rigidbody bulletClone = Instantiate(bullet, transform.position, transform.rotation);
        bulletClone.velocity = transform.forward * bulletSpeed;

        bulletClone.gameObject.GetComponent<NetworkObject>().Spawn();
        Destroy(bulletClone.gameObject, 3);

       if (powerUp)
        {
            Vector3 temp = new Vector3(1, 0, 0);
            bulletPOS.Translate(temp, bulletPOS);
            bulletClone = Instantiate(bullet, bulletPOS.position, transform.rotation);
            bulletClone.velocity = transform.forward * speed;
            bulletClone.gameObject.GetComponent<NetworkObject>().Spawn();
            Destroy(bulletClone.gameObject, 3);

            temp = new Vector3(-2, 0, 0);
            bulletPOS.Translate(temp, bulletPOS);
            bulletClone = Instantiate(bullet, bulletPOS.position, transform.rotation);
            bulletClone.velocity = transform.forward * speed;
            bulletClone.gameObject.GetComponent<NetworkObject>().Spawn();
            Destroy(bulletClone.gameObject, 3);

            temp = new Vector3(1, 0, 0);
            bulletPOS.Translate(temp, bulletPOS);
        }
    }
}
