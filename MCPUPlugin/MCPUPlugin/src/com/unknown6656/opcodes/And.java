package com.unknown6656.opcodes;


public final class And extends BinaryOpcode
{
    public And()
    {
        super((x, y) -> x & y);
    }
}
