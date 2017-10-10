package com.unknown6656.opcodes;


public final class Eq extends BinaryBooleanOpcode
{
    public Eq()
    {
        super((x, y) -> x == y);
    }
}
