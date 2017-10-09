package com.unknown6656.opcodes;


public final class Exp extends UnaryOpcode
{
    public Exp()
    {
        super(x -> (int)Math.pow(Math.E, x));
    }
}
