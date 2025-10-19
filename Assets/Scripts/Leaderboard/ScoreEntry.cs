[System.Serializable]
public class ScoreEntry
{
    public string initials;
    public int score;
    
    public ScoreEntry(string initials, int score)
    {
        this.initials = initials;
        this.score = score;
    }
}