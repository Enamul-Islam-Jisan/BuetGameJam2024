using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Utils : SingletonMonoBehaviour<Utils>
{

#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void ReloadCurrentPage(); 
#endif

    public static void Reload()
    {
        if (!Application.isPlaying) return;
#if !UNITY_EDITOR && UNITY_WEBGL
        ReloadCurrentPage();  // This calls the JS function when running in WebGL
#else
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }

}
