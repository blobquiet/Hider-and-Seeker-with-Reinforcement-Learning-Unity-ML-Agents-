using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour
{
    public float rotationSpeed = 8f;
    public float translationSpeed = 8f;
    private Animator anim;
    public float x, y;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        transform.Translate(x*Time.deltaTime*rotationSpeed,0,0);
        transform.Translate(0,0,y*Time.deltaTime*translationSpeed);

        anim.SetFloat("X_speed",x);
        anim.SetFloat("Y_speed",y);
        
    }
}
