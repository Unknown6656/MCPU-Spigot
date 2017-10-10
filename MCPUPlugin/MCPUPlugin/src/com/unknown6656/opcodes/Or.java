package com.unknown6656.opcodes;

import com.unknown6656.BinaryOpcode;

public final class Or extends BinaryOpcode
{
    public Or()
    {
        super((x, y) -> x | y);
    }
}
