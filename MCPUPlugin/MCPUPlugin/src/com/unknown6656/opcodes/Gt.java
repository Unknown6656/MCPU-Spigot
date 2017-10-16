package com.unknown6656.opcodes;

import com.unknown6656.BinaryBooleanOpcode;

public final class Gt extends BinaryBooleanOpcode
{
    public Gt()
    {
        super((x, y) -> x > y);
    }
}
