using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView)), CanEditMultipleObjects]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;

        // draw field of view angle
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.ViewRadius);
        Vector3 viewAngleA = fov.DirFromAngle(-fov.ViewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.ViewAngle / 2, false);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.ViewRadius);
        Handles.color = Color.blue;

        // draw nearby sphere
        Handles.DrawWireDisc(fov.transform.position, Vector3.up, fov.MinViewRadius);
        Handles.DrawWireDisc(fov.transform.position, Vector3.right, fov.MinViewRadius);
        Handles.DrawWireDisc(fov.transform.position, Vector3.forward, fov.MinViewRadius);
        Handles.color = Color.red;

        // draw attack radius visual
        Handles.DrawWireDisc(fov.transform.position, Vector3.up, fov.AttackRadius);
        viewAngleA = fov.DirFromAngle(-fov.AttackAngle / 2, false);
        viewAngleB = fov.DirFromAngle(fov.AttackAngle / 2, false);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.AttackRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.AttackRadius);
        foreach (Transform visibleTarget in fov.visibleTargets)
        {
            Handles.DrawLine(fov.transform.position, visibleTarget.position);
        }
    }
}
