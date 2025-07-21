public interface ISaveable
{
    // ref = Pass exact GameData reference.
    public void SaveData(ref GameData data);

    public void LoadData(GameData data);
}
