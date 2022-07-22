using System.Collections;
using System.Collections.Generic;
using System;

public static class HexGlobals
{
    //! ========== FOR HEXAGONS WITH POINTS UP
    
    // for this to work, both vars have to be static    
    public static float Radius = Game._hexHeight * 0.5f;
    public static float Height = 2 * Radius;
    public static float RowHeight = 1.5f * Radius;
    public static float HalfWidth = (float)Math.Sqrt((Radius * Radius) - ((Radius / 2) * (Radius / 2)));
    public static float Width = 2 * HalfWidth;
    public static float ExtraHeight = Height - RowHeight;
    public static float Edge = RowHeight - ExtraHeight;
}
