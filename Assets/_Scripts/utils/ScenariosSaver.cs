#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class ScenariosSaver
{
    [MenuItem("Tools/SaveScenariosToResources")]
    public static void Go()
    {
        var saver = GameObject.Find("GameManager/SaveGameManager").GetComponent<SaveGameManager>();
        var slots = GameObject.Find("GameManager/SaveSlots").GetComponent<SaveSlots>();
        List<Scenario> scenarios = slots.GetScenarioReferences();
        saver.LoadScenarios();

        /*
        List<string> writtenControllers = new List<string>();
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            var currentController = enemy.transform.root.gameObject.GetComponentInChildren<Animator>().runtimeAnimatorController;
            if (!writtenControllers.Contains(currentController.name))
            {
                string enumName = "AnimationList_" + currentController.name;
                string filePathAndName = "Assets/Scripts/NPC_animationEnums/" + enumName + ".cs";
                using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
                {
                    streamWriter.WriteLine("public enum " + enumName);
                    streamWriter.WriteLine("{");
                    AnimationClip[] clips = currentController.animationClips;
                    for (int i = 0; i < clips.Length; i++)
                    {
                        streamWriter.Write("\t" + clips[i].name);
                        if (i < clips.Length - 1)
                        {
                            streamWriter.WriteLine(",");
                        }
                        else
                        {
                            streamWriter.WriteLine();
                        }
                    }
                    streamWriter.Write("};");
                    writtenControllers.Add(currentController.name);
                }
                AssetDatabase.Refresh();
            }
        }*/
    }
}
#endif