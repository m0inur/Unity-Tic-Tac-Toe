using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine.Serialization;

[CreateAssetMenu (menuName = "Singletons/MasterManager")]
public class MasterManager : SingletonScriptableObject<MasterManager> {
    [SerializeField]
    private GameSettings gameSettings;
    public static GameSettings GameSettings { get { return Instance.gameSettings; } }
}