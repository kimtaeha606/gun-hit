using System;


public static class GameSignals
{
    // B(�Է�/��) -> A(�ھ�)
    public static event Action ShootRequested;
    public static void RaiseShootRequested() => ShootRequested?.Invoke();

    // B(�߻��) -> A(�ھ�)
    public static event Action BulletFired;
    public static void RaiseBulletFired() => BulletFired?.Invoke();

    // B(��Ʈ ����) -> A(�ھ�)
    public static event Action<FruitTarget> FruitHit;
    public static void RaiseFruitHit(FruitTarget fruit) => FruitHit?.Invoke(fruit);

    // B(�̽�/���� ����) -> A(�ھ�)
    public static event Action DiskHitOrMiss;
    public static void RaiseDiskHitOrMiss() => DiskHitOrMiss?.Invoke();

    // A(�ھ�) -> UI/��Ÿ
    public static event Action<int> StageStarted;
    public static void RaiseStageStarted(int stageIndex) => StageStarted?.Invoke(stageIndex);

    public static event Action<int> AmmoSet;
    public static void RaiseAmmoSet(int ammo) => AmmoSet?.Invoke(ammo);

    public static event Action<int> AmmoChanged;
    public static void RaiseAmmoChanged(int ammo) => AmmoChanged?.Invoke(ammo);

    public static event Action GameOver;
    public static void RaiseGameOver() => GameOver?.Invoke();

    public static event Action<int> StageCleared;
    public static void RaiseStageCleared(int stageIndex) => StageCleared?.Invoke(stageIndex);

    public static event Action<FruitTarget> FruitDestroyed;
    public static void RaiseFruitDestroyed(FruitTarget fruit) => FruitDestroyed?.Invoke(fruit);
}


