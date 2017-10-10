package com.unknown6656.opcodes;

import com.unknown6656.BinaryBooleanOpcode;

public final class Leq extends BinaryBooleanOpcode
{
    public Leq()
    {
        super((x, y) -> x <= y);
    }
}
