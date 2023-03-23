using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public CameraController camcon;
    public IUserInput pi; 
    public float walkSpeed = 2.4f;
    public float runMultiplier = 2.0f;
    public float jumpVelocity = 4.0f;
    public float rollVelocity = 2.5f;

    [Space(10)]
    [Header("===== Friction Settings =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    public Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;
    private Vector3 thrustVec;
    private bool canAttack;
    private bool lockPlanar = false;
    public bool trackDiretion = false;
    private CapsuleCollider col;
    //private float lerpTarget;
    private Vector3 deltaPos;


    public bool leftIsShield = true;

    public delegate void OnActionDelegate();
    public event OnActionDelegate OnAction;

    // Start is called before the first frame update
    void Awake(){
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach(var input in inputs){
            if(input.enabled == true){
                pi = input;
                break;
            }
        }
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        camcon = GetComponentInChildren<CameraController>();
}

    // Update is called once per frame
    void Update(){

        if (pi.lockon){
            camcon.LockUnlock();
        }

        if (camcon.lockState == false){
            anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), ((pi.run) ? 2.0f : 1.0f), 0.5f));
            anim.SetFloat("right", 0);
        }
        else{
            Vector3 localDVec = transform.InverseTransformVector(pi.DVec);
            anim.SetFloat("forward", localDVec.z * ((pi.run) ? 2.0f : 1.0f));
            anim.SetFloat("right", localDVec.x * ((pi.run) ? 2.0f : 1.0f));
        }

        //anim.SetBool("defense", pi.defense);
        
        if(pi.jump || rigid.velocity.magnitude > 5f){
            anim.SetTrigger("roll");
            canAttack = false;
        }
        
        if (pi.jump){
            anim.SetTrigger("jump");
            canAttack = false;
        }
       
        if (pi.rb || pi.lb && (CheckState("ground") || CheckStateTag("attackR") || CheckStateTag("attackL")) && canAttack){
            if (pi.rb){
                anim.SetBool("R0L1", false);
                anim.SetTrigger("attack");
            }
            else if (pi.lb && !leftIsShield){
                anim.SetBool("R0L1", true);
                anim.SetTrigger("attack"); 
            }
        }

        if (pi.rt || pi.lt && (CheckState("ground") || CheckStateTag("attackR") || CheckStateTag("attackL")) && canAttack){
            if (pi.rt){
                
            }
            else{
                if (!leftIsShield){

                }
                else{
                    anim.SetTrigger("counterBack");
                }
            }
        }

        if (pi.action) { 
            OnAction.Invoke();
        }

        if (leftIsShield){
            if (CheckState("ground") || CheckState("blocked")){
                anim.SetBool("defense", pi.defense);
                anim.SetLayerWeight(anim.GetLayerIndex("defense"), 1);
            }
            else{
                anim.SetBool("defense", false);
                anim.SetLayerWeight(anim.GetLayerIndex("defense"), 0);
            }
        }
        else{
            anim.SetLayerWeight(anim.GetLayerIndex("defense"), 0);
        }


        if (camcon.lockState == false){
            if (pi.Dmag > 0.1f){
                model.transform.forward = Vector3.Slerp(model.transform.forward, pi.DVec, 0.3f);
            }
       
            if (lockPlanar == false){
                planarVec = pi.Dmag * model.transform.forward * walkSpeed * ((pi.run) ? runMultiplier : 1.0f);
            }
        }
        else{   
            if(trackDiretion == false){
                model.transform.forward = transform.forward;
            }
            else{
                model.transform.forward = planarVec.normalized;
            }
            if (lockPlanar == false){
                planarVec = pi.DVec * walkSpeed * ((pi.run) ? runMultiplier : 1.0f);
            }              
        }
    }

    void FixedUpdate(){
        rigid.position += deltaPos;
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    public bool CheckState(string stateName, string layerName = "Base Layer"){
        int layerIndex = anim.GetLayerIndex(layerName);
        return anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        
    }

    public bool CheckStateTag(string tagName, string layerName = "Base Layer"){
        int layerIndex = anim.GetLayerIndex(layerName);
        return anim.GetCurrentAnimatorStateInfo(layerIndex).IsTag(tagName);
        
    }

    public void OnJumpEnter(){
        thrustVec = new Vector3 (0, jumpVelocity, 0);
        pi.inputEnabled = false;
        lockPlanar = true;
        trackDiretion = true;
    }

    public void IsGround(){
        anim.SetBool("isGround", true);
    }

    public void IsNotGround(){
        anim.SetBool("isGround", false);
    }

    public void OnGroundEnter(){
        pi.inputEnabled = true;
        lockPlanar = false;
        canAttack = true;
        col.material = frictionOne;
        trackDiretion = false;
    }

    public void OnGroundExit(){
        col.material = frictionZero;
    }

    public void OnFallEnter(){
        pi.inputEnabled = false;
        lockPlanar = true;
    }

    public void OnRollEnter(){
        thrustVec = new Vector3(0, rollVelocity, 0);
        pi.inputEnabled = false;
        lockPlanar = true;
        trackDiretion = true;
    }

    public void OnJabEnter(){
        pi.inputEnabled = false;
        lockPlanar = true;
    }

    public void OnJabUpdate(){
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity");
    }

    public void OnAttack1hAEnter(){
        pi.inputEnabled = false;
        //lockPlanar = true;
        //lerpTarget = 1.0f;
    }

    public void OnAttack1hAUpdate(){
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        //float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("attack"));
        //currentWeight = Mathf.Lerp(currentWeight, lerpTarget, 0.1f);
        //anim.SetLayerWeight(anim.GetLayerIndex("attack"), currentWeight);
    }

    public void OnAttackExit(){
        model.SendMessage("WeaponDisable");
    }


    public void OnHitEnter(){
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }

    public void OnDieEnter(){
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }

    public void OnBlockedEnter(){
        pi.inputEnabled = false;
    }

    public void OnStunnedEnter(){
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
    }

    public void OnCounterBackEnter(){
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
    }

    public void OnCounterBackExit(){
        model.SendMessage("CounterBackDisable");
    }

    public void OnLockEnter() {
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }

    public void OnUpdateRM(object _deltaPos){
        if (CheckState("attack1hC")){ 
            deltaPos += (0.8f * deltaPos + 0.2f * (Vector3)_deltaPos);
        }
    }

    public void IssueTrigger(string triggerName){
        anim.SetTrigger(triggerName);
    }
    
    public void SetBool(string boolName, bool value){
        anim.SetBool(boolName, value);
    }

}
