using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Rigidbody[] ragdollRbs;
    private Animator animator;

    [Header("Ragdoll Death Settings")]
    [SerializeField] private float dieForce;

    private void Start()
    {
        ragdollRbs = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();

        deactivateRagdoll();
    }

    public void deactivateRagdoll()
    {
        foreach (var rb in ragdollRbs)
        {
            rb.isKinematic = true;
        }

        animator.enabled = true;
    }

    public void activateRagdoll()
    {
        foreach (var rb in ragdollRbs)
        {
            rb.isKinematic = false;
        }

        animator.enabled = false;
    }

    public void addForce (Vector3 force)
    {
        force.y = 2f;

        var rb = animator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();

        rb.AddForce(force * dieForce, ForceMode.VelocityChange);
    }
}
