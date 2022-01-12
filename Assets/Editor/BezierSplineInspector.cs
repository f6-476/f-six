using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Custom inspector modified from CatLikeCoding
[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
    private const int lineSteps = 10;
    private const float directionScale = 0.5f;

    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private void OnSceneGUI () {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;
		
        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < spline.points.Length; i += 3) {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);
			
            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);
			
            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }
        ShowDirections();
    }

    private void ShowDirections () {
        Handles.color = Color.green;
        Vector3 point = spline.GetBezierPoint(0f).position;
        Handles.DrawLine(point, point + spline.GetBezierTangent(0f) * directionScale);
        for (int i = 1; i <= lineSteps; i++) {
            point = spline.GetBezierPoint(i / (float)lineSteps).position;
            Handles.DrawLine(point, point + spline.GetBezierTangent(i / (float)lineSteps) * directionScale);
        }
    }

    private Vector3 ShowPoint (int index) {
        Vector3 point = handleTransform.TransformPoint(spline.points[index]);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.points[index] = handleTransform.InverseTransformPoint(point);
        }
        return point;
    }
    public override void OnInspectorGUI () {
        DrawDefaultInspector();
        spline = target as BezierSpline;
        if (GUILayout.Button("Add Curve")) {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }
    }
}
