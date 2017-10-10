package com.unknown6656.opcodes;

public final class Shl extends BinaryOpcode
{
    public Shl()
    {
        super((x, y) -> x << y);
    }
}
