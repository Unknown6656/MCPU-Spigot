package com.unknown6656.opcodes;


public final class Neq extends BinaryBooleanOpcode
{
    public Neq()
    {
        super((x, y) -> x != y);
    }
}
