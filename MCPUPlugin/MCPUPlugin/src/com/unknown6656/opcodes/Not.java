package com.unknown6656.opcodes;


public final class Not extends UnaryOpcode
{
    public Not()
    {
        super(x -> ~x);
    }
}
