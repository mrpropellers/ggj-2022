using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MakeAngryGhost : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        GetComponent<Animator>().SetBool("AlwaysAngry", true);
    }

    void OnDisable()
    {
        GetComponent<Animator>().SetBool("AlwaysAngry", false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
