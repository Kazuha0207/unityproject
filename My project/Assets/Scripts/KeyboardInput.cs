using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : IUserInput{
    //Variable
    [Header("===== Key settings =====")]
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";

    public string keyA;
    public string keyB;
    public string keyC;
    public string keyD;
    public string keyE;
    public string keyF;
    public string keyG;
    public string keyH;
    public string keyI;
    public string keyJ;
    public string keyK;
    public string keyL;
    public string keyM;

    public string keyJUp;
    public string keyJDown;
    public string keyJRight;
    public string keyJLeft;

    [Header("===== Mouse settings =====")]
    public bool mouseEnable = true;
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;

    //   [Header("===== Output signals =====")]


    // Update is called once per frame
    void Update(){
        //Jup = (Input.GetKey(keyJUp) ? 1.0f : 0) - (Input.GetKey(keyJDown) ? 1.0f : 0);
        //Jright = (Input.GetKey(keyJRight) ? 1.0f : 0) - (Input.GetKey(keyJLeft) ? 1.0f : 0);
        if(mouseEnable == true){
            Jup = Input.GetAxis("Mouse Y") * 3f * mouseSensitivityY;
            Jright = Input.GetAxis("Mouse X") * 2.5f * mouseSensitivityX;
        }
        else{
            Jup = (Input.GetKey(keyJUp) ? 1.0f : 0) - (Input.GetKey(keyJDown) ? 1.0f : 0);
            Jright = (Input.GetKey(keyJRight) ? 1.0f : 0) - (Input.GetKey(keyJLeft) ? 1.0f : 0);
        }
        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0) - (Input.GetKey(keyDown) ? 1.0f : 0);
        targetDright = (Input.GetKey(keyRight) ? 1.0f : 0) - (Input.GetKey(keyLeft) ? 1.0f : 0);

        if (inputEnabled == false){
            targetDup = 0;
            targetDright = 0;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref vilocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref vilocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
        DVec = Dup2 * transform.forward + Dright2 * transform.right;

        run = Input.GetKey(keyA);
        defense = Input.GetKey(keyC);

        bool newJump = Input.GetKey(keyB);

        if(newJump != lastJump && newJump == true){
            jump = true;
        }
        else{
            jump = false;
        }
        lastJump = newJump;

        rb = Input.GetKey(keyD);
        lb = Input.GetKey(keyE);
        rt = Input.GetKey(keyF);
        lt = Input.GetKey(keyG);

        lockon = Input.GetKey(keyH);

        action = Input.GetKey(keyI);



        //bool newAttack = Input.GetKey(keyD);
        //
        //
        //if (newAttack != lastAttack && newAttack == true){
        //    rb = true;
        //}
        //else{
        //    rb = false;
        //}
        //lastAttack = newAttack;
    }
    //private Vector2 SquareToCircle(Vector2 input){
    //    Vector2 output = Vector2.zero;
    //    output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
    //    output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

    //    return output;
    //}
}
