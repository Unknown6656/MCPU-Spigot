package com.unknown6656.opcodes;

import com.unknown6656.BinaryBooleanOpcode;

public final class Neq extends BinaryBooleanOpcode
{
    public Neq()
    {
        super((x, y) -> x != y);
    }
}
