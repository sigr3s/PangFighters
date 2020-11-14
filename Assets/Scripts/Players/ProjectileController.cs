﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private GameObject trailGO = null;
    private Vector3 _moveDirection = Vector3.zero;
    public bool alive = true;
    public PlayerID shooter = PlayerID.NP;

    void Start()
    {
        _moveDirection.y = 10.0f;
    }

    void Update()
    {
        transform.Translate(_moveDirection * Time.deltaTime, Space.World);
        trailGO.transform.localScale = new Vector3(trailGO.transform.localScale.x, Mathf.Abs(transform.position.y) - 0.5f, trailGO.transform.localScale.z);
        trailGO.transform.localPosition = new Vector3(trailGO.transform.localPosition.x, -0.5f - (trailGO.transform.localScale.y / 2.0f), trailGO.transform.localPosition.z);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(DataUtility.gameData.isNetworkedGame){
            Debug.LogWarning("Collision for shot not implemented on network");
        }
        else{

            if(DataUtility.gameData.isNetworkedGame){
                Debug.LogWarning("Handle net collisions");
            }
            else{
                if (collision.collider.tag == "Ceiling")
                {
                    alive = false;
                    gameObject.SetActive(false);
                }
                else{
                    // FIXME: Handle player here
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(DataUtility.gameData.isNetworkedGame){
            Debug.LogWarning("Trigger for shot not implemented on network");
        }
        else{
            if(other.gameObject.TryGetComponent<Hazard>(out Hazard h)){   
                if(h.TryDestroyHazard(shooter)){
                    alive = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnDisable() {
        alive = false;
    }
}