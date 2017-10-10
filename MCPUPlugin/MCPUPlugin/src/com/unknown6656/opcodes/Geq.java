package com.unknown6656.opcodes;


public final class Geq extends BinaryBooleanOpcode
{
    public Geq()
    {
        super((x, y) -> x >= y);
    }
}
