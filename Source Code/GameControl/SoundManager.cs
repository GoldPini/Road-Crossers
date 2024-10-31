using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AllSoundEffectsSO allSoundEffectsSO;
    [SerializeField] private AudioSource soundEffectObject;
    
    private List<GameObject> soundEffectObjectsList = new List<GameObject>();
    private Queue<GameObject> carPassingAmbientSoundEffectObjectsQueue = new Queue<GameObject>();
    private float maxCarPassingSoundsAlive = 12;

    public AudioClip RandomEngineSound { 
        get
        {
            return allSoundEffectsSO.EngineSounds[Random.Range(0, allSoundEffectsSO.EngineSounds.Length)];
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        BaseVheicle.OnAnyVehicleHonking += BaseVheicle_OnAnyVehicleHonking;
        BaseVheicle.OnAnyVehicleExploded += BaseVheicle_OnAnyVehicleExploded;
        RigidBodyCharacterController.OnPlayerDeathImpact += Player_OnPlayerDeathImpact;
        Projectile.OnAnyProjectileFired += Projectile_OnAnyProjectileFired;
        Projectile.OnAnyProjectileExploded += Projectile_OnAnyProjectileExploded;
        CollectableItem.OnAnyItemPicked += CollectableItem_OnAnyItemPicked;
        GameStartCountdownUI.OnCountdownStarted += GameStartCountdownUI_OnCountdownStarted;
        RigidBodyCharacterController.OnLevelFailure += Player_OnLevelFailure;
        GameManager.OnLevelSuccess += GameManager_OnLevelSuccess;
        GameManager.OnGameReset += GameManager_OnGameReset;
    }

    private void FixedUpdate()
    {
        int i = carPassingAmbientSoundEffectObjectsQueue.Count;
        while (i > maxCarPassingSoundsAlive && carPassingAmbientSoundEffectObjectsQueue.Count > 0)
        {
            GameObject carPassingSoundEffect = carPassingAmbientSoundEffectObjectsQueue.Dequeue();
            if (carPassingSoundEffect != null)
                Destroy(carPassingSoundEffect);
        }        
    }

    private void GameManager_OnGameReset(object sender, System.EventArgs e)
    {
        foreach(GameObject soundEffect in soundEffectObjectsList)
        {
            if(soundEffect != null)
                Destroy(soundEffect);
        }

        foreach (GameObject soundEffect in carPassingAmbientSoundEffectObjectsQueue)
        {
            if (soundEffect != null)
                Destroy(soundEffect);
        }
    }

    private void GameManager_OnLevelSuccess(object sender, System.EventArgs e)
    {
        playSound2D(allSoundEffectsSO.LevelSuccessSound, 0.5f);
    }

    private void Player_OnLevelFailure(object sender, OnGameFailureEventArgs e)
    {
        playSound2D(allSoundEffectsSO.LevelFailureSound, 0.2f);
    }

    private void GameStartCountdownUI_OnCountdownStarted(object sender, System.EventArgs e)
    {
        playSound2D(allSoundEffectsSO.ThreeSecondsCountdown, 0.08f);
    }

    private void CollectableItem_OnAnyItemPicked(object sender, System.EventArgs e)
    {
        playSound3D(allSoundEffectsSO.ItemCollectedSounds, (sender as CollectableItem).transform);
    }

    private void Projectile_OnAnyProjectileFired(object sender, System.EventArgs e)
    {
        playSound3D(allSoundEffectsSO.ProjectileShotSounds, (sender as Projectile).transform, 0.6f);

    }

    private void Projectile_OnAnyProjectileExploded(object sender, System.EventArgs e)
    {
        playSound3D(allSoundEffectsSO.ExplosionSounds, (sender as Projectile).transform, 0.6f);
    }

    private void Player_OnPlayerDeathImpact(object sender, System.EventArgs e)
    {
        playSound3D(allSoundEffectsSO.ImpactSounds, (sender as RigidBodyCharacterController).transform, 1.3f);
    }

    private void BaseVheicle_OnAnyVehicleExploded(object sender, System.EventArgs e)
    {
        playSound3D(allSoundEffectsSO.ExplosionSounds, (sender as BaseVheicle).transform, 0.6f);
    }

    private void BaseVheicle_OnAnyVehicleHonking(object sender, System.EventArgs e)
    {
        playSound3D(allSoundEffectsSO.HornSounds, (sender as BaseVheicle).transform);
    }

    private void playSound3D(AudioClip[] clip, Transform positionTransform, float volume = 1f)
    {
        AudioClip audioClip = clip[Random.Range(0, clip.Length)];
        spawnSoundEffectObject(audioClip, positionTransform ,volume);
    }

    private void playSound3D(AudioClip clip, Transform positionTransform, float volume = 1f)
    {
        spawnSoundEffectObject(clip, positionTransform ,volume);
    }

    private AudioSource playSound2D(AudioClip[] clip, float volume = 1f)
    {
        AudioClip audioClip = clip[Random.Range(0, clip.Length)];

        return playSound2D(audioClip, volume);
    }

    private AudioSource playSound2D(AudioClip clip, float volume = 1f)
    {

        float clipLength = clip.length;

        AudioSource audioSource = Instantiate(soundEffectObject);

        audioSource.clip = clip;

        audioSource.volume = volume;

        audioSource.spatialBlend = 0;

        audioSource.Play();

        soundEffectObjectsList.Add(audioSource.gameObject);

        Destroy(audioSource.gameObject, clipLength);

        return audioSource;
    }

    public void PlayFootStepSound(Transform positionTransform)
    {
        spawnSoundEffectObject(allSoundEffectsSO.FootstepSound, positionTransform, 2f);
    }

    public void PlayTrafficAmbientSound()
    {
        playSound2D(allSoundEffectsSO.TrafficAmbientSounds, 0.12f);
    }

    public void PlayCarPassingAmbientSound()
    {
        AudioSource carPassingAmbientSource = playSound2D(allSoundEffectsSO.CarsPassingAmbientSounds, 0.12f);
        carPassingAmbientSoundEffectObjectsQueue.Enqueue(carPassingAmbientSource.gameObject);
    }

    private void spawnSoundEffectObject(AudioClip audioClip, Transform positionTransform, float volume)
    {
        float clipLength = audioClip.length;

        AudioSource audioSource = Instantiate(soundEffectObject);

        audioSource.gameObject.transform.position = positionTransform.position;

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.spatialBlend = 1;

        audioSource.Play();

        soundEffectObjectsList.Add(audioSource.gameObject);

        Destroy(audioSource.gameObject, clipLength);
    }
}
