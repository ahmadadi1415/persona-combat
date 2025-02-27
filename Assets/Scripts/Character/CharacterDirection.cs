using UnityEngine;

public enum RelativeDirection { AHEAD, BEHIND, ONSIDE }
public class CharacterDirection
{
    public static RelativeDirection GetRelativePosition(Vector3 source, Vector3 target, Vector2 sourceDirection)
    {
        // Compute normalized direction vector from source to target.
        Vector2 toTarget = (target - source).normalized;

        // Compute dot product between facing direction and direction to target.
        float dot = Vector2.Dot(sourceDirection.normalized, toTarget);

        // Define thresholds: 
        // If the angle is less than ~45° (dot > 0.707) -> AHEAD.
        // If the angle is more than ~135° (dot < -0.707) -> BEHIND.
        // Otherwise, the target is roughly on the side.
        if (dot >= 0.707f)
            return RelativeDirection.AHEAD;
        else if (dot <= -0.707f)
            return RelativeDirection.BEHIND;
        else
            return RelativeDirection.ONSIDE;
    }

    private static Vector2 GetCardinalDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return new Vector2(Mathf.Sign(direction.x), 0); // Face Left (-1,0) or Right (1,0)
        else
            return new Vector2(0, Mathf.Sign(direction.y)); // Face Up (0,1) or Down (0,-1)
    }

    public static Vector2 GetFacingDirectionToTarget(Vector3 source, Vector3 target)
    {
        Vector2 toTarget = (target - source).normalized;
        return GetCardinalDirection(toTarget);
    }
}