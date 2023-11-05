using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FabricIK : MonoBehaviour
{
    [Header("Chain Settings")]
    public int chainLength = 3; // Number of spaces between the bone positions.

    // Target (where we aim) and pole (to move along the multiple solutions for one target position).
    [Header("Target / Pole Settings")]
    public Transform target;
    public Transform pole;

    [Header("Solver Settings")]
    public int iterations = 10; // Solver iterations per update.
    public float delta = 0.001f; // Distance to the target when the solver stops.

    // Bones information (to be initialized).
    [Header("Bones Information")]
    public Transform[] bones;
    public Vector3[] bonesPositions;
    public float[] bonesLength;
    public float completeLength;

    // Debug.
    private Vector3[] startingBoneDirectionToNext;
    private Quaternion[] startingBoneRotation;
    private Quaternion startingTargetRotation;
    private Quaternion startingRotationRoot;

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        bones = new Transform[chainLength + 1];
        bonesPositions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];
        completeLength = 0;

        startingBoneDirectionToNext = new Vector3[chainLength + 1];
        startingBoneRotation = new Quaternion[chainLength + 1];
        startingTargetRotation = target.rotation;

        var current = this.transform;
        for (var i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            startingBoneRotation[i] = current.rotation;

            if (i != bones.Length - 1)
            {
                bonesLength[i] = (bones[i + 1].position - current.position).magnitude;
                completeLength += bonesLength[i];
            }

            startingBoneDirectionToNext[i] = (target.position - current.position).normalized;
            current = current.parent;
        }
    }

    void LateUpdate()
    {
        FastIK();
    }

    void FastIK()
    {
        if (target == null)
        {
            Debug.Log("[INFO] No Target selected");
            return;
        }

        if (bonesLength.Length != chainLength)
        {
            Debug.Log("[INFO] Re-initializing bones");
            Init();
        }

        for (int i = 0; i < bones.Length; i++)
        {
            bonesPositions[i] = bones[i].position;
        }

        if (Vector3.Distance(bonesPositions[0], target.position) > completeLength)
        {
            for (int i = 0; i < bonesPositions.Length; i++)
            {
                if (i == 0)
                    bonesPositions[i] = bones[0].position;
                else
                    bonesPositions[i] = bonesPositions[i - 1] + startingBoneDirectionToNext[i - 1] * bonesLength[i - 1];
            }
        }
        else
        {
            for (int ite = 0; ite < iterations; ite++)
            {
                for (int i = bonesPositions.Length - 1; i > 0; i--)
                {
                    if (i == bonesPositions.Length - 1)
                        bonesPositions[i] = target.position;
                    else
                        bonesPositions[i] = bonesPositions[i + 1] + (bonesPositions[i] - bonesPositions[i + 1]).normalized * bonesLength[i];
                }

                for (int i = 1; i < bonesPositions.Length; i++)
                {
                    bonesPositions[i] = bonesPositions[i - 1] + (bonesPositions[i] - bonesPositions[i - 1]).normalized * bonesLength[i - 1];
                }

                if ((bonesPositions[bonesPositions.Length - 1] - target.position).sqrMagnitude < delta * delta)
                {
                    break;
                }
            }

            if (pole != null)
            {
                for (int i = 1; i < bonesPositions.Length - 1; i++)
                {
                    var plane = new Plane(bonesPositions[i + 1] - bonesPositions[i - 1], bonesPositions[i - 1]);
                    var projectedPole = plane.ClosestPointOnPlane(pole.position);
                    var projectedBone = plane.ClosestPointOnPlane(bonesPositions[i]);
                    var angle = Vector3.SignedAngle(projectedBone - bonesPositions[i - 1], projectedPole - bonesPositions[i - 1], plane.normal);
                    bonesPositions[i] = Quaternion.AngleAxis(angle, plane.normal) * (bonesPositions[i] - bonesPositions[i - 1]) + bonesPositions[i - 1];
                }
            }

            for (int i = 0; i < bonesPositions.Length; i++)
            {
                if (i == bonesPositions.Length - 1)
                    bones[i].rotation = target.rotation * Quaternion.Inverse(startingTargetRotation) * startingBoneRotation[i];
                else
                    bones[i].rotation = Quaternion.FromToRotation(startingBoneDirectionToNext[i], bonesPositions[i + 1] - bonesPositions[i]) * startingBoneRotation[i];
                bones[i].position = bonesPositions[i];
            }
        }

        for (int i = 0; i < bonesPositions.Length; i++)
        {
            bones[i].position = bonesPositions[i];
        }
    }

    private void OnDrawGizmos()
    {
        var current = this.transform;

        for (int i = 0; i < chainLength && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.blue;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);

            current = current.parent;
        }
    }
}