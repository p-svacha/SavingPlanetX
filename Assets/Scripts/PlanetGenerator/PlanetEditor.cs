using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet Planet;
    Editor GeneralEditor;
    Editor ColorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if(check.changed)
            {
                Planet.GeneratePlanet();
            }
        }

        if(GUILayout.Button("Generate Planet"))
        {
            Planet.GeneratePlanet();
        }

        DrawSettingsEditor(Planet.Settings, Planet.OnGeneralSettingsUpdated, ref Planet.GeneralSettingsFoldout, ref GeneralEditor);
        DrawSettingsEditor(Planet.ColorSettings, Planet.OnColorSettingsUpdated, ref Planet.ColorSettingsFoldout, ref ColorEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed && onSettingsUpdated != null)
                        onSettingsUpdated();
                }
            }
        }
    }

    private void OnEnable()
    {
        Planet = (Planet)target;
    }
}
