using System;
[Serializable]
public class PatternPromt : Promt
{
    public override string ToString()
    {
        return "PATRON"; //"PATTERN";
    }
    public override bool Check(EditablePieces currentPiece, Piece selected)
    {
        return currentPiece.Pattern.sprite == selected.Pattern;
    }
}