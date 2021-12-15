using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public int damage = 10;
    public int range = 100;
    public float reloadTime = 1.0f;
    public bool canShoot = true;
    public Text playerAmmoText;
    [SerializeField] private int bulletAmount;

    private void Start()
    {
        bulletAmount = 10;
        InvokeRepeating("Reload", reloadTime, 0.0f);
    }
    public int BulletAmount
    {
        get { return bulletAmount; }
        set 
        { 
            bulletAmount = value;
            playerAmmoText.text = bulletAmount.ToString();
        }
    }


    public Camera fpsCam;
    public ParticleSystem muzzle;
    public GameObject hitEffect;

    public float timer = 5f;

    void CanShoot()
    {
        if(bulletAmount <= 0)
            canShoot = false;
        else
            canShoot = true;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            if (canShoot == true)
               {
                   Shoot();
               }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
            canShoot=true;
        }
    }
    void Reload()
    {
        BulletAmount = 10;
    }
    void Shoot()
    {
        muzzle.Play();
        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            //Debug.Log(hit.transform.name);

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            GameObject hitGO = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(hitGO, 1);
        }

        BulletAmount--;
        CanShoot();

    }


}
