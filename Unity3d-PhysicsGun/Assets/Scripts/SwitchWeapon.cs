using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwitchWeapon : MonoBehaviour
{
    public GameObject PhyScisGun;
    public GameObject RopeGun;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)&&!Input.GetMouseButton(0))
        {
            RopeGun.SetActive(false);
            PhyScisGun.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.F2)&&!Input.GetMouseButton(0))
        {
            PhyScisGun.SetActive(false);
            RopeGun.SetActive(true);
        }
    }
}
