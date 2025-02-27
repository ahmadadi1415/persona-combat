public enum MoveType { ATTACK, DEFEND, SPELL, RUN }
public class MoveData
{
    public MoveType MoveType;
    
    // Power in percentage, 0.1, 1, 1.5
    public float Power;
}