using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public IUserInput pi;
    public float horizontalSpeed = 100.0f;
    public float verticalSpeed = 70.0f;
    public float cameraDampValue = 0.1f;
    public Image lockDot;
    public bool lockState;
    public bool isAI = false;

    private GameObject playerHandle;
    private GameObject cameraHandle;
    private float tempEulerX;
    [HideInInspector]
    public GameObject model;
    private GameObject camera;
    private Vector3 cameraDampVelocity;
    [SerializeField]
    private LockTarget lockTarget;

    void Start(){
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 20;
        ActorController ac = playerHandle.GetComponent<ActorController>();
        model = ac.model;
        pi = ac.pi;

        if (!isAI){
            camera = Camera.main.gameObject;
            lockDot.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        lockState = false;
    }
    


    // Update is called once per frame
    void FixedUpdate(){
        if (lockTarget == null){

            Vector3 tempModelEuler = model.transform.eulerAngles;

            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);
            tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

            model.transform.eulerAngles = tempModelEuler;
        }
        else{
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform);
        }

        if (!isAI){
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position, ref cameraDampVelocity, cameraDampValue);
            //camera.transform.eulerAngles = transform.eulerAngles;
            camera.transform.LookAt(cameraHandle.transform);
        }
        
    }

    void Update(){
        if(lockTarget != null){
            if (!isAI){
                lockDot.rectTransform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position + new Vector3(0, lockTarget.halfHeight, 0));
            }
            
            if(Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 10f){
                LockProcessA(null, false, false, isAI);
            }

            if(lockTarget.am != null && lockTarget.am.sm.isDie){
                LockProcessA(null, false, false, isAI);
            }
        }
    }

    private void LockProcessA(LockTarget _lockTarget, bool _lockDotEnable, bool _lockState, bool _isAI){
        lockTarget = _lockTarget;
        if (!_isAI){
            lockDot.enabled = _lockDotEnable;
        }
        lockState = _lockState;
    }

    public void LockUnlock(){
        //if(lockTarget == null)
        //{
            Vector3 modelOrigin1 = model.transform.position;
            Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0);
            Vector3 boxCenter = modelOrigin2 + model.transform.forward * 5.0f;

            Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f), model.transform.rotation, LayerMask.GetMask(isAI ? "Player" : "Enemy"));
        if(cols.Length == 0){
            LockProcessA(null, false, false, isAI);
        }
        else{
            foreach(var col in cols){
                if(lockTarget != null && lockTarget.obj == col.gameObject){
                    LockProcessA(null, false, false, isAI);
                    break;
                }
                LockProcessA(new LockTarget(col.gameObject, col.bounds.extents.y), true, true, isAI);
                break;
            }
        }
    }

    private class LockTarget{
        public GameObject obj;
        public float halfHeight;
        public ActorManager am;

        public LockTarget(GameObject _obj, float _halfHeight){
            obj = _obj;
            halfHeight = _halfHeight;
            am = _obj.GetComponent<ActorManager>();
        }
    }
}
