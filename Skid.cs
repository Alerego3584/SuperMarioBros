﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Skid : Animation
{
    public Skid(Texture2D texture) : base(texture, "Skid", 0.1f)
    {
        frames = new Rectangle[1];
        frames[0] = new Rectangle(330, 0, 16, 16);
    }
}

