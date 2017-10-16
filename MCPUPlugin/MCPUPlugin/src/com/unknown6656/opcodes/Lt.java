package com.unknown6656.opcodes;

import com.unknown6656.BinaryBooleanOpcode;

public final class Lt extends BinaryBooleanOpcode
{
    public Lt()
    {
        super((x, y) -> x < y);
    }
}
