using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyIUserInput : IUserInput{
    // Start is called before the first frame update
    IEnumerator Start(){
        while (true){
            //Dup = 1.0f;
            //Dright = 0f;
            //Jup = 0f;
            //Jright = 1.0f;
            //run = true;
            //yield return new WaitForSeconds(3.0f);
            //Dup = 0f;
            //Dright = 0f;
            //Jup = 0f;
            //Jright = 0f;
            //yield return new WaitForSeconds(1.0f);
            rb = true;
            yield return 0;

        }
    }

    // Update is called once per frame
    void Update(){
        UpdateDmagDvec(Dup, Dright);
    }
}
