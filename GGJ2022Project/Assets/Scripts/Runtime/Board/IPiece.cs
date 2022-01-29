namespace GGJ
{
    public interface IBoardPiece
    {
        public BoardPiece Piece { get; }

        public bool IsTangible { get; }
    }
}
