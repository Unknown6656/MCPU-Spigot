package com.unknown6656.opcodes;


public final class Mod extends BinaryOpcode
{
    public Mod()
    {
        super((x, y) -> x % y);
    }
}
