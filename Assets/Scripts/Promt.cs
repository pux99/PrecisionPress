using System;
[Serializable]
public abstract class Promt
{
    public abstract bool Check(EditablePieces currentPiece, Piece selected);
}

public enum PromtType
{
    Color,
    Pattern,
    Form,
}