using System;

public class FormPromt : Promt
{
    public override string ToString()
    {
        return "form";
    }
    public override bool Check(Piece currentPiece, Piece selected)
    {
        return currentPiece.form == selected.form;
    }
}