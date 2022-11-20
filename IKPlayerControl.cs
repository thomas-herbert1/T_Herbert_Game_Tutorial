using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKPlayerControl : MonoBehaviour
{
    private Animator playerAnimator;

    private Terrain terrain;

    public float footToGroundDistance;

    private Vector3 leftFootPos;
    private Vector3 rightFootPos;

    public LayerMask playerMask;
    
    
    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        terrain = Terrain.activeTerrain;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (playerAnimator)
        {
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, playerAnimator.GetFloat("LeftFootWeight"));
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, playerAnimator.GetFloat("LeftFootWeight"));
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightFoot, playerAnimator.GetFloat("RightFootWeight"));
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightFoot, playerAnimator.GetFloat("RightFootWeight"));
            
            // Left Foot
            leftFootPos = playerAnimator.GetIKPosition(AvatarIKGoal.LeftFoot);
            leftFootPos.y = terrain.SampleHeight(leftFootPos) + footToGroundDistance;
            playerAnimator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPos);
            RaycastHit hitLeft;
            if (Physics.Raycast(leftFootPos, Vector3.down, out hitLeft, playerMask))
            {
                playerAnimator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hitLeft.normal));
            }
            
            
            // Right Foot
            rightFootPos = playerAnimator.GetIKPosition(AvatarIKGoal.RightFoot);
            rightFootPos.y = terrain.SampleHeight(rightFootPos) + footToGroundDistance;
            playerAnimator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPos);
            RaycastHit hitRight;
            if (Physics.Raycast(rightFootPos, Vector3.down, out hitRight, playerMask))
            {
                playerAnimator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hitRight.normal));
            }
        }
    }
}
