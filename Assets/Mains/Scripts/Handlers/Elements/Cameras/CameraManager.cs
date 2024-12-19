using UnityEngine;
using YNL.Patterns.Singletons;

public class CameraManager : Singleton<CameraManager>
{
    public CameraMovementManager Movement;
    public CameraDoorManager Door;
    public CameraAudioManager Audio;
}