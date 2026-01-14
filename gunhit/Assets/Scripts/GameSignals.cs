using System;


public static class GameSignals
{
    // Input system -> gameplay: player requested a shot.
    public static event Action ShootRequested;
    public static void RaiseShootRequested() => ShootRequested?.Invoke();

    // Weapon/bullet system -> gameplay: bullet actually fired.
    public static event Action BulletFired;
    public static void RaiseBulletFired() => BulletFired?.Invoke();

    // Target -> gameplay: fruit target was hit (before it is destroyed).
    public static event Action<FruitTarget> FruitHit;
    public static void RaiseFruitHit(FruitTarget fruit) => FruitHit?.Invoke(fruit);

    // Target -> gameplay: disk was hit or missed (result resolved).
    public static event Action DiskHitOrMiss;
    public static void RaiseDiskHitOrMiss() => DiskHitOrMiss?.Invoke();

    // Gameplay -> UI: stage started.
    public static event Action<int> StageStarted;
    public static void RaiseStageStarted(int stageIndex) => StageStarted?.Invoke(stageIndex);

    // Gameplay -> UI: ammo set to absolute value.
    public static event Action<int> AmmoSet;
    public static void RaiseAmmoSet(int ammo) => AmmoSet?.Invoke(ammo);

    // Gameplay -> UI: ammo changed to new value after a shot/reload.
    public static event Action<int> AmmoChanged;
    public static void RaiseAmmoChanged(int ammo) => AmmoChanged?.Invoke(ammo);

    // Gameplay -> UI: game over.
    public static event Action GameOver;
    public static void RaiseGameOver() => GameOver?.Invoke();

    // Gameplay -> UI: stage cleared.
    public static event Action<int> StageCleared;
    public static void RaiseStageCleared(int stageIndex) => StageCleared?.Invoke(stageIndex);

    // Target -> gameplay: fruit target destroyed (after break animation).
    public static event Action<FruitTarget> FruitDestroyed;
    public static void RaiseFruitDestroyed(FruitTarget fruit) => FruitDestroyed?.Invoke(fruit);
}
