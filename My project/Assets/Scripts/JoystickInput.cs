using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickInput : IUserInput{

    [Header("===== Joysitck Settings =====")]
    public string axisX = "axisX";
    public string axisY = "axisY";
    public string axisJright = "axis3";
    public string axisJup = "axis5";
    public string btnA = "btn0";
    public string btnB = "btn1";
    public string btnC = "btn2";
    public string btnD = "btn3";
    public string btnLB = "btn4";
    public string btnLT = "btn6";
    public string btnRB = "btn5";
    public string btnRT = "btn7";
    public string btnJstick = "btn11";

    public MyButton buttonA = new MyButton();
    public MyButton buttonB = new MyButton();
    public MyButton buttonC = new MyButton();
    public MyButton buttonD = new MyButton();
    public MyButton buttonLB = new MyButton();
    public MyButton buttonLT = new MyButton();
    public MyButton buttonRB = new MyButton();
    public MyButton buttonRT = new MyButton();
    public MyButton buttonJstick = new MyButton();
    //[Header("===== Output signals =====")]

    //[Header("===== Others =====")]


    void Update(){
        buttonA.Tick(Input.GetButton(btnA));
        buttonB.Tick(Input.GetButton(btnB));
        buttonC.Tick(Input.GetButton(btnC));
        buttonD.Tick(Input.GetButton(btnD));
        buttonLB.Tick(Input.GetButton(btnLB));
        buttonLT.Tick(Input.GetButton(btnLT));
        buttonRB.Tick(Input.GetButton(btnRB));
        buttonRT.Tick(Input.GetButton(btnRT));
        buttonJstick.Tick(Input.GetButton(btnJstick));

        //print(buttonA.IsExtending || buttonA.IsPressing);

        Jup = -1 * (Input.GetAxis(axisJup));
        Jright = (Input.GetAxis(axisJright));

        targetDup = (Input.GetAxis(axisY));
        targetDright = (Input.GetAxis(axisX));

        if (inputEnabled == false){
            targetDup = 0;
            targetDright = 0;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref vilocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref vilocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt(Dup2 * Dup2 + Dright2 * Dright2);
        DVec = Dright2 * transform.right + Dup2 * transform.forward;

        //if(Dmag<0.1f && buttonA.Ispressing) {
        //    buttonA.RewindDelayTimer();
        //}
        run = (buttonA.IsPressing && !buttonA.IsDelaying) || buttonA.IsExtending;        
        jump = buttonA.OnPressed && buttonA.IsExtending;
        roll = buttonA.OnReleased && buttonA.IsDelaying;
        action = buttonC.OnPressed;
        
        defense = buttonLB.IsPressing;
        //attack = buttonC.OnPressed;
        rb = buttonRB.OnPressed;
        rt = buttonRT.OnPressed;
        lb = buttonLB.OnPressed;
        lt = buttonLT.OnPressed;
        lockon = buttonJstick.OnPressed;
    }

    //private Vector2 SquareToCircle(Vector2 input){
    //    Vector2 output = Vector2.zero;
    //    output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
    //    output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

    //    return output;
    //}

}
