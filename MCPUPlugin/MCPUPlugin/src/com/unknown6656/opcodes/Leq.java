package com.unknown6656.opcodes;


public final class Leq extends BinaryBooleanOpcode
{
    public Leq()
    {
        super((x, y) -> x <= y);
    }
}
