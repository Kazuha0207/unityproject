using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CapsuleCollider))]

public class BattleManager : IActorManagerInterface{
    //public ActorManager am;
    
    private CapsuleCollider defCol;
    
    void Start(){
        defCol = GetComponent<CapsuleCollider>();
        defCol.center = Vector3.up * 1.0f;
        defCol.height = 2.0f;
        defCol.radius = 0.5f;
        defCol.isTrigger = true;
    }
    
    void OnTriggerEnter(Collider col){
        WeaponController targetWc = col.GetComponentInParent<WeaponController>();
        if(targetWc == null){
            return;
        }

        GameObject attacker = targetWc.wm.am.gameObject;
        GameObject receiver = am.gameObject;

        Vector3 attackingDir = receiver.transform.position - attacker.transform.position;
        Vector3 counterDir = attacker.transform.position - receiver.transform.position;

        float attackingAngle1 = Vector3.Angle(attacker.transform.forward, attackingDir);
        float counterAngle1 = Vector3.Angle(receiver.transform.forward, counterDir);
        float counterAngle2 = Vector3.Angle(attacker.transform.forward, receiver.transform.forward);

        bool attackValid = (attackingAngle1 < 45) ;
        bool counterValid = (counterAngle1 < 45 && Mathf.Abs(counterAngle2 - 180) < 30);


        if (col.tag == "Weapon"){
            am.TryDoDamage(targetWc, attackValid, counterValid);
        }
    }
}
