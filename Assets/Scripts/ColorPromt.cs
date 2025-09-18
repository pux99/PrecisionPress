using System;

public class ColorPromt : Promt
{
    public override string ToString()
    {
        return "Color";
    }
    public override bool Check(EditablePieces currentPiece, Piece selected)
    {
        return currentPiece.Form.color == selected.Color;
    }
}