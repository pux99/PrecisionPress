using System;
using Unity.VisualScripting;

public class PatternPromt : Promt
{
    public override string ToString()
    {
        return "pattern";
    }
    public override bool Check(Piece currentPiece, Piece selected)
    {
        return currentPiece.Pattern == selected.Pattern;
    }
}