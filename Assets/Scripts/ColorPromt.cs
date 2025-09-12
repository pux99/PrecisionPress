using System;

public class ColorPromt : Promt
{
    public override string ToString()
    {
        return "Color";
    }
    public override bool Check(Piece currentPiece, Piece selected)
    {
        return currentPiece.Color == selected.Color;
    }
}