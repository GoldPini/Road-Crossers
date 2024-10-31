using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    private void Awake()
    {
        LevelGenerator.ResetStaticData();
        GameManager.ResetStaticData();
        CollectableItem.ResetStaticData();
        Player_brain.ResetStaticData();
        RigidBodyCharacterController.ResetStaticData();
        RigidBodyVheicleController.ResetStaticData();
        Projectile.ResetStaticData();
        RoadSegment.ResetStaticData();
        GamePausedMenuUI.ResetStaticData();
        LevelUI.ResetStaticData();
        GameStartCountdownUI.ResetStaticData();
    }
}
