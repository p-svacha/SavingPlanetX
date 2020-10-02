using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Initializing,
    Idle,
    BuildMode,
    DayCycle,
    EventDialog,
    AlertFlash,
    MoveToDisaster,
    DisasterOccuring,
    WaitAfterDisaster
}
