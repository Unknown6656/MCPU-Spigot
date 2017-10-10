package com.unknown6656.opcodes;


public final class Incr extends UnaryOpcode
{
    public Incr()
    {
        super(x -> x - 1);
    }
}
