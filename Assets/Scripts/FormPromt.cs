using System;

public class FormPromt : Promt
{
    public override string ToString()
    {
        return "form";
    }
    public override bool Check(EditablePieces currentPiece, Piece selected)
    {
        return currentPiece.Form.sprite == selected.form;
    }
}