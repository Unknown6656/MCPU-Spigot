package com.unknown6656.opcodes;


public final class Pow extends BinaryOpcode
{
    public Pow()
    {
        super((x, y) -> (int)Math.pow(x, y));
    }
}
