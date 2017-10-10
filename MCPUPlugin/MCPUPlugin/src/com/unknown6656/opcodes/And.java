package com.unknown6656.opcodes;

import com.unknown6656.BinaryOpcode;

public final class And extends BinaryOpcode
{
    public And()
    {
        super((x, y) -> x & y);
    }
}
