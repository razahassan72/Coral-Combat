using UnityEditor;
using UnityEngine;

public class MeshExtractor : MonoBehaviour
{
    [MenuItem("Tools/Mixamo/Extract Mesh From Selected")]
    static void ExtractMesh()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("❌ No GameObject selected!");
            return;
        }

        SkinnedMeshRenderer smr = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
        if (smr == null || smr.sharedMesh == null)
        {
            Debug.LogError("❌ No SkinnedMeshRenderer with a valid mesh found!");
            return;
        }
        if (!smr.sharedMesh.isReadable)
        {
            Debug.LogError("❌ The mesh is not readable. Enable 'Read/Write Enabled' on the FBX import settings.");
            return;
        }


        Debug.Log("✅ Selected GameObject: " + Selection.activeGameObject.name);
        Debug.Log("✅ Mesh to extract: " + smr.sharedMesh.name);

        string savePath = EditorUtility.SaveFilePanelInProject("Save Extracted Mesh", smr.sharedMesh.name, "asset", "Choose save location for extracted mesh");
        if (string.IsNullOrEmpty(savePath))
        {
            Debug.LogWarning("⚠️ Save operation cancelled.");
            return;
        }

        Mesh newMesh = Object.Instantiate(smr.sharedMesh);
        AssetDatabase.CreateAsset(newMesh, savePath);
        AssetDatabase.SaveAssets();
        Debug.Log("✅ Mesh extracted and saved to: " + savePath);
    }
}
