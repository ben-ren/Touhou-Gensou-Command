public interface IResourceReceiver
{
    //Setters
    void SetHealth(int value);
    void SetPower(int value);
    void SetGraze(int value);
    void SetFuel(int value);
    void SetMoney(int value);
    void SetBombs(int value);

    // Getters
    int GetHealth();
    int GetPower();
    int GetGraze();
    int GetFuel();
    int GetMoney();
    int GetBombs();
}
