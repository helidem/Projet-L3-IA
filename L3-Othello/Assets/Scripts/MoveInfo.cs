using System.Collections.Generic;

public class MoveInfo
{
    public Joueur Joueur { get; set; }
    public Position Position { get; set; }
    public List<Position> Captures { get; set; }

}
