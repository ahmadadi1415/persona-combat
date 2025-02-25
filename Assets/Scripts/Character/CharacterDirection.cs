using UnityEngine;

public enum RelativeDirection { AHEAD, BEHIND, ONSIDE }
public class CharacterDirection
{
    public static RelativeDirection GetRelativePosition(Vector3 source, Vector3 target, Vector2 sourceDirection)
    {
        Vector2 directionToTarget = (target - source).normalized;
        Vector2 playerDirection = GetCardinalDirection(directionToTarget);

        if (playerDirection == sourceDirection) return RelativeDirection.AHEAD;
        if (playerDirection == -sourceDirection) return RelativeDirection.BEHIND;
        return RelativeDirection.ONSIDE;
    }

    private static Vector2 GetCardinalDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return new Vector2(Mathf.Sign(direction.x), 0); // Face Left (-1,0) or Right (1,0)
        else
            return new Vector2(0, Mathf.Sign(direction.y)); // Face Up (0,1) or Down (0,-1)
    }
}