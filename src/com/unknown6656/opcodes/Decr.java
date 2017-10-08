package com.unknown6656.opcodes;


public final class Decr extends UnaryOpcode
{
    public Decr()
    {
        super(x -> x + 1);
    }
}
