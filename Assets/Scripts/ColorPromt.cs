using System;

public class ColorPromt : Promt
{
    public override string ToString()
    {
        return "COLOR";
    }
    public override bool Check(EditablePieces currentPiece, Piece selected)
    {
        return currentPiece.Form.color == selected.Color;
    }
}