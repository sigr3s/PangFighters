﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public enum HazardTarget{
        None,
        Player1,
        Player2
    }
    
    public HazardTarget hazardTarget;
    public float gravity = 20.0F;
    public float speed = 20.0F;
    public float bounceFactor = 1.9f;
    public float scaleFactor = 5f;


    private int HazardLevel = 4;
    private float xSpeed = 0f;
    private float ySpeed = 0f;

    public LayerMask mask;
    public LayerMask Horizontal;
    public LayerMask Vertical;
    public LayerMask Weapon;
    public LayerMask Player;
    private RaycastHit[] hits = new RaycastHit[10];
    private int hitCount = 0;
    private HazardSpawner spawner;
    private bool alive = false;
    
    public void Initialize(HazardSpawner hazardSpawner, int level, Transform t, float dir = 1)
    {
        transform.localScale = level*scaleFactor*Vector3.one;
        transform.position = t.position;
        xSpeed = speed * dir;
        ySpeed = gravity;
        HazardLevel = level;
        spawner = hazardSpawner;
        alive = true;
        
        gameObject.SetActive(true);
    }

    private Vector3 prevPosition = Vector3.zero;

    private void Update() {
        if(!alive) return;

        transform.position += new Vector3(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, 0f);

        hitCount = Physics.SphereCastNonAlloc(transform.position, transform.localScale.x * 0.5f, (prevPosition- transform.position).normalized, hits, 0f, mask, QueryTriggerInteraction.UseGlobal);

        for(int i = 0; i < hitCount; i++){
            var h = hits[i];

            if(h.collider != null && h.collider.gameObject != gameObject){
                

                if(Horizontal == (Horizontal | (1 << h.collider.gameObject.layer))){
                    if(h.collider.gameObject.tag == "Ceiling"){
                        ySpeed = gravity * bounceFactor * Mathf.Sign(h.transform.forward.y);
                    }
                    else{
                        ySpeed = -gravity * bounceFactor * Mathf.Sign(h.transform.forward.y);
                    }


                    transform.position += new Vector3(0, ySpeed * Time.deltaTime, 0f);
                }
                else if( Vertical == (Vertical | (1 << h.collider.gameObject.layer))){
                    xSpeed *= -1;
                    transform.position += new Vector3(2* xSpeed * Time.deltaTime, 0f, 0f);
                }
                else if(Weapon == (Weapon | (1 << h.collider.gameObject.layer))){
                    h.collider.gameObject.SetActive(false);
                    DestroyHazard();
                    Destroy(h.collider.gameObject);
                    return;
                }
                else if(Player == (Player | (1 << h.collider.gameObject.layer))){
                    Debug.Log("Player?");
                    return;   
                }
                else{
                    continue;
                }
            }
        }


        ySpeed = Mathf.Clamp(ySpeed + gravity*Time.deltaTime, gravity, -gravity * 3f);

        prevPosition = transform.position;
    }

    private void DestroyHazard(){
        alive = false;
        gameObject.SetActive(false);
        spawner.HazardDestroyed(HazardLevel, transform);
        Destroy(gameObject);
    }
}