package com.unknown6656.opcodes;

import com.unknown6656.BinaryBooleanOpcode;

public final class Eq extends BinaryBooleanOpcode
{
    public Eq()
    {
        super((x, y) -> x == y);
    }
}
