using System;

public class FormPromt : Promt
{
    public override string ToString()
    {
        return "form";
    }
    public override bool Check(EditablePieces currentPiece, Piece selected)
    {
        if (currentPiece == null || selected == null) return false;

        if (currentPiece.Form != null && currentPiece.Form.sprite != null)
            return currentPiece.Form.sprite == selected.form;

        return currentPiece.CurrentForm == selected.form;
    }
}