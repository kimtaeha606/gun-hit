using System;


public static class GameSignals
{
    // B(입력/샷) -> A(코어)
    public static event Action ShootRequested;
    public static void RaiseShootRequested() => ShootRequested?.Invoke();

    // B(히트 판정) -> A(코어)
    public static event Action<FruitTarget> FruitHit;
    public static void RaiseFruitHit(FruitTarget fruit) => FruitHit?.Invoke(fruit);

    // B(미스/원판 명중) -> A(코어)
    public static event Action DiskHitOrMiss;
    public static void RaiseDiskHitOrMiss() => DiskHitOrMiss?.Invoke();

    // A(코어) -> UI/기타
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

