package com.unknown6656.opcodes;

import com.unknown6656.BinaryBooleanOpcode;

public final class Geq extends BinaryBooleanOpcode
{
    public Geq()
    {
        super((x, y) -> x >= y);
    }
}
