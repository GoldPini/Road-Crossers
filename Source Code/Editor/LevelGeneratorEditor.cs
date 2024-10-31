//using UnityEditor;

//[CustomEditor(typeof(LevelGenerator))]
//public class LevelGeneratorEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        LevelGenerator levelGen = target as LevelGenerator;

        

//        if (levelGen.GameMode == LevelMode.Finite)
//        {
//            levelGen.TotalSegments = EditorGUILayout.IntField("Total Segments", levelGen.TotalSegments);
//        }

//        if (levelGen.GameSpawnMode == SpawnMode.DistanceBased)
//        {
//            levelGen.MinDistanceFromLastSegment = EditorGUILayout.FloatField(
//                "Min Distance from last segment", levelGen.MinDistanceFromLastSegment);
//            //levelGen.SpawnBatches = EditorGUILayout.IntField("Spawn Batches", levelGen.SpawnBatches);
//        }


//    }
//}
