using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DiveBlessing : Blessing
{
    private bool diving;
    private bool landedFromDive;
    private float superJumpWindow = 0.1f; //.1
    private float superJumpTimer = 0f;
    public CinemachineImpulseSource impulseSource;

    public override void OnEquip()
    {
        impulseSource = PlayerController.Instance.impulse;
        diving = false;
        landedFromDive = false;
        superJumpTimer = 0f;
    }

    public override void OnUnequip()
    {
        diving = false;
        landedFromDive = false;
        superJumpTimer = 0f;
    }

    public override void HandleInput()
    {
        if (diving || PlayerController.Instance.isWallSliding) return;

        if (PlayerController.Instance.yAxis < 0 && PlayerController.Instance.DashInput && !PlayerController.Instance.Grounded())
        {
            diving = true;
            PlayerController.Instance.sDiving = true;
            PlayerController.Instance.audsrc.PlayOneShot(PlayerController.Instance.diveSound);
            PlayerController.Instance.anim.SetBool("SDiving",true);
            PlayerController.Instance.rb.velocity = new Vector3(0, -50, 0);
        }

        if (landedFromDive && PlayerController.Instance.JumpJustPressed)
        {
            PlayerController.Instance.rb.velocity = new Vector3(0, 35, 0); // 35
            landedFromDive = false;
            superJumpTimer = 0f;
        }
    }

    public override void Tick()
    {

        if (diving && !PlayerController.Instance.Grounded())
        {
            if (PlayerController.Instance.rb.velocity.y > 0 || PlayerController.Instance.isWallSliding)
            {
                diving = false;
                PlayerController.Instance.sDiving = false;
                PlayerController.Instance.anim.SetBool("SDiving", false);
            }
        }

        if (diving && PlayerController.Instance.Grounded())
        {
            diving = false;
            PlayerController.Instance.sDiving = false;
            landedFromDive = true;
            PlayerController.Instance.audsrc.PlayOneShot(PlayerController.Instance.strongLandingSound);
            impulseSource.GenerateImpulseWithForce(.1f);
            PlayerController.Instance.anim.SetBool("SDiving", false);
            PlayerController.Instance.anim.SetTrigger("SDLanded");
            superJumpTimer = superJumpWindow;
        }

        if (landedFromDive)
        {
            superJumpTimer -= Time.deltaTime;
            if (superJumpTimer <= 0f)
            {
                landedFromDive = false;
            }
        }

        if (diving && PlayerController.Instance.dashed)
        {
            diving = false;
            PlayerController.Instance.sDiving = false;
            PlayerController.Instance.anim.SetBool("SDiving", false);
        }

    }

}
