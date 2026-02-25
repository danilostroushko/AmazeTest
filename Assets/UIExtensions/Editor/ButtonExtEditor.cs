using DG.DOTweenEditor.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonExt))]
[CanEditMultipleObjects]
public class ButtonExtEditor : UnityEditor.UI.ButtonEditor
{
    SerializedProperty p_enableState;
    SerializedProperty p_disableState;

    SerializedProperty p_animateTransform;
    SerializedProperty p_autoSelectAO;
    SerializedProperty p_animateOnDisable;
    SerializedProperty p_tweenDuration;
    SerializedProperty p_hoverScale;
    SerializedProperty p_upScale;
    SerializedProperty p_downScale;

    SerializedProperty p_hoverEase;
    SerializedProperty p_hoverEaseCurve;

    SerializedProperty p_downEase;
    SerializedProperty p_downEaseCurve;

    private bool showStates = false;
    private bool showAnimations = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        p_enableState = serializedObject.FindProperty("enableState");
        p_disableState = serializedObject.FindProperty("disableState");

        p_animateTransform = serializedObject.FindProperty("animationObject");
        p_autoSelectAO = serializedObject.FindProperty("autoSelectAO");
        p_animateOnDisable = serializedObject.FindProperty("animateOnDisable");
        p_tweenDuration = serializedObject.FindProperty("duration");
        p_hoverScale = serializedObject.FindProperty("upScale");
        p_upScale = serializedObject.FindProperty("hoverScale");
        p_downScale = serializedObject.FindProperty("downScale");

        p_hoverEase = serializedObject.FindProperty("hoverEase");
        p_hoverEaseCurve = serializedObject.FindProperty("hoverScaleEase");

        p_downEase = serializedObject.FindProperty("downEase");
        p_downEaseCurve = serializedObject.FindProperty("downScaleEase");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        showStates = EditorGUILayout.BeginFoldoutHeaderGroup(showStates, "States");
        if (showStates) 
        {
            EditorGUILayout.PropertyField(p_enableState);
            EditorGUILayout.PropertyField(p_disableState);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showAnimations = EditorGUILayout.BeginFoldoutHeaderGroup(showAnimations, "Animations");
        if (showAnimations)
        {
            EditorGUILayout.PropertyField(p_animateTransform);
            EditorGUILayout.PropertyField(p_autoSelectAO, new GUIContent("Auto select target"));
            EditorGUILayout.PropertyField(p_animateOnDisable);
            EditorGUILayout.PropertyField(p_tweenDuration);
            EditorGUILayout.PropertyField(p_hoverScale);
            EditorGUILayout.PropertyField(p_upScale);
            EditorGUILayout.PropertyField(p_downScale);

            EditorGUILayout.PropertyField(p_hoverEase);//TODO: EditorGUIUtils.FilteredEasePopup("Ease", easeType);
            if (p_hoverEase.enumValueFlag == 37)
            {
                p_hoverEaseCurve.animationCurveValue = EditorGUILayout.CurveField(p_hoverEaseCurve.animationCurveValue);
            }

            EditorGUILayout.PropertyField(p_downEase);
            if (p_downEase.enumValueFlag == 37)
            {
                p_downEaseCurve.animationCurveValue = EditorGUILayout.CurveField(p_downEaseCurve.animationCurveValue);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        serializedObject.ApplyModifiedProperties();
    }
}