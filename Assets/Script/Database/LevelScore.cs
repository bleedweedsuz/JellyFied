using SQLite4Unity3d;

public class LevelScore
{
    [PrimaryKey, AutoIncrement]
    public int LevelId { get; set; }
    public string LevelName { get; set; }
    public int LastScore { get; set; }
    public bool Lock { get; set; }
    public override string ToString()
    {
        return string.Format("[LevelScore: LevelId={0}, LevelName={1},  LastScore={2}], Lock={3}", LevelId, LevelName, LastScore,Lock);
    }
}
