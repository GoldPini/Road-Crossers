using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "ScriptableObjects/All Sound Effects SO")]
public class AllSoundEffectsSO : ScriptableObject
{
    public AudioClip[] EngineSounds;
    public AudioClip[] TrafficAmbientSounds;
    public AudioClip[] CarsPassingAmbientSounds;
    public AudioClip[] HornSounds;
    public AudioClip[] ImpactSounds;
    public AudioClip FootstepSound;
    public AudioClip[] ProjectileShotSounds;
    public AudioClip[] ExplosionSounds;
    public AudioClip[] ItemCollectedSounds;
    public AudioClip ThreeSecondsCountdown;
    public AudioClip LevelSuccessSound;
    public AudioClip LevelFailureSound;
}
