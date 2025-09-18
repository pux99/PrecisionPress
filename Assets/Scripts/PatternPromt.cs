using System;
[Serializable]
public class PatternPromt : Promt
{
    public override string ToString()
    {
        return "pattern";
    }
    public override bool Check(EditablePieces currentPiece, Piece selected)
    {
        return currentPiece.Pattern.sprite == selected.Pattern;
    }
}