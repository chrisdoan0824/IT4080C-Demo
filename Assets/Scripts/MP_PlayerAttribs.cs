using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MP_PlayerAttribs : NetworkBehaviour
{
    public Slider hpBar;
    private float maxHp = 100f;
    private float damageVal = 10f;
    private NetworkVariableFloat currentHp = new NetworkVariableFloat(100f);
    public NetworkVariableBool powerUp = new NetworkVariableBool(false);

    // Update is called once per frame
    void Update()
    {
        hpBar.value = currentHp.Value / maxHp;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && IsOwner)
        {
            Debug.Log("Got hit");
            TakeDamageServerRpc(damageVal);
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.CompareTag("MedKit") && IsOwner)
        {
            Debug.Log("Healed");
            HealDamageServerRpc();
        }
        else if(collision.gameObject.CompareTag("PowerUp") && IsOwner)
        {
            Debug.Log("I got powered up");
            damageVal = damageVal / 2;
            powerUp.Value = true;
        }
    }

    [ServerRpc]
    private void TakeDamageServerRpc(float damage)
    {
        currentHp.Value -= damage;
        if (currentHp.Value < 0)
        {
            Debug.Log("You died");
            Destroy(this.gameObject);
        }
    }

    [ServerRpc]
    private void HealDamageServerRpc()
    {
        currentHp.Value += 20f;
        if(currentHp.Value > maxHp)
        {
            currentHp.Value = maxHp;
        }
    }
}
