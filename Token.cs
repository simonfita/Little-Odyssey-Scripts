using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Token : MonoBehaviour
{
    public Animator anim;
    public TokenRenderer rend;
    public TokenQuips quips;
    public AudioSource stepSrc;
    public Transform holdingPoint;


    public Goods heldGoods;


    public bool canMove = true;
    public bool inSlot { get; private set; }=false;

    public Vector3 lastFramePosition = Vector3.zero;

    private void Update()
    {


        Vector3 velocity = transform.position - lastFramePosition;
        
        //testing speed
        /*if(CompareTag("Player"))
            Debug.Log(gameObject.name+ velocity.magnitude / Time.deltaTime);*/
        
        velocity.Normalize();

        if (velocity.magnitude > 0.2f&&!inSlot)
            StartMovementAnimation();
        else
            StopMovementAnimation();
        
        
        if (!inSlot)
        {
            if (velocity.x < 0)
                rend.SetFlip(true);
            else if (velocity.x > 0)
                rend.SetFlip(false);
        }
        
        lastFramePosition = transform.position;
    }

    public virtual void SetActive(bool active)
    {
        rend.enabled = active;
        foreach (Collider col in  GetComponentsInChildren<Collider>())
        { 
            col.enabled = active;
        }
    
    }


    private void StopMovementAnimation()
    {
        anim.SetFloat("WalkBlend", 0);
        
    }
    private void StartMovementAnimation()
    {
        anim.SetFloat("WalkBlend", 1);

    }
    public void MadeStep()
    {
        stepSrc.Play();
    }

    public bool HoldGoods(Goods goods)
    {
        if (heldGoods != null)
            return false;

        heldGoods = goods;

        heldGoods.transform.parent =holdingPoint;
        heldGoods.transform.localPosition = Vector3.zero;
        heldGoods.transform.localRotation = Quaternion.identity;


        rend.SetHoldsArms(true);

        return true;
    }

    public void LeaveGoods(Stockpile leaveStockpile)
    {
        if (heldGoods == null)
            return;

        if (leaveStockpile.LeaveIn(heldGoods))
        {
            heldGoods = null;
            rend.SetHoldsArms(false);
        }
    }

    public void DestroyGoods()
    {
        if (!heldGoods)
            return;
        Destroy(heldGoods.gameObject);
        heldGoods = null;
        rend.SetHoldsArms(false);
    }

    public void EnterSlot(Transform slotPoint)
    {
        inSlot = true;

        transform.parent = slotPoint;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;


        canMove = false;

        StopMovementAnimation();
    }

    public void ExitSlot(Vector3 exitSpot)
    {
        inSlot = false;
        transform.parent = null;
        canMove = true;
    }
}
