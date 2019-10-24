﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : MonoBehaviour
{
    //General:
    private new string name;
    private int id;
    private float reloadTime;
    private float fireRate;
    private float spread;
    private float range;
    private int ammo;
    private int clipSize;
    private int clip;
    private FireType function;

    //References:
    private GameObject gunObject;

    //Public Properties:
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    public float ReloadTime
    {
        get { return reloadTime; }
        set { reloadTime = value; }
    }

    public float FireRate
    {
        get { return fireRate; }
        set { fireRate = value; }
    }

    public float Spread
    {
        get { return spread; }
        set { spread = value; }
    }

    public float Range
    {
        get { return range; }
        set { range = value; }
    }

    public int ClipSize
    {
        get { return clipSize; }
        set { clipSize = value; }
    }

    public int Clip
    {
        get { return clip; }
        set { clip = value; }
    }

    public int Ammo
    {
        get { return ammo; }
        set { ammo = value; }
    }

    public FireType Function
    {
        get { return function; }
        set { function = value; }
    }

    public GameObject Gun
    {
        get { return gunObject; }
        set { gunObject = value; }
    }

    public void Shoot(Camera playerCam, GameObject gun)
    {
        switch (function)
        {
            case FireType.Hitscan:
                Vector3 mousePosition = playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                Debug.DrawRay(mousePosition, playerCam.transform.forward * range, Color.red, 5f);
                if (Physics.Raycast(mousePosition, playerCam.transform.forward, out hit, range))
                {
                    if (hit.collider.tag == "Enemy")
                    {
                        hit.collider.gameObject.GetComponent<Enemy>().Death();
                    }
                }
                break;
            case FireType.Projectile:
                GameObject bullet = Resources.Load<GameObject>("Prefabs/Bullet");
                Instantiate(bullet, transform.position, Quaternion.identity);
                Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
                bulletRigid.AddForce(Vector3.forward * 100f, ForceMode.Impulse);
                break;
            case FireType.Entity:

                break;
            case FireType.Shotgun:

                break;
        }
    }
}

public enum FireType
{
    Hitscan,
    Projectile,
    Entity,
    Shotgun
}