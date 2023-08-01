using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public enum Types
    {
        None, TimerTens, TimerOnes, HoldMinimum, HoldMaximum, BetweenMinimum, BetweenMaximum, MultiplePresses, ControlsRotate, ControlsFlipHorizontal, ControlsFlipVertical, ControlsFlipLeftDiagonal, ControlsFlipRightDiagonal, Defocus
    }
    public Types Type;
    public int Variable;

    public Condition(Types type, int variable)
    {
        Type = type;
        Variable = variable;
    }
}
