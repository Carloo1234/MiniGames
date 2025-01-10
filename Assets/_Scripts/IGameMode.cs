// IGameMode.cs
public interface IGameMode
{
    void Initialize();
    void Update();
    void FixedUpdate();
    void OnPlayerScored();
    void OnPlayerDied();
}
