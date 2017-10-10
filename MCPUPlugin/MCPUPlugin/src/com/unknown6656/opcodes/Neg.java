package com.unknown6656.opcodes;


public final class Neg extends UnaryOpcode
{
    public Neg()
    {
        super(x -> -x);
    }
}
