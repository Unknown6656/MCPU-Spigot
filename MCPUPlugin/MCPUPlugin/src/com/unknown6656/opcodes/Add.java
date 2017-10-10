package com.unknown6656.opcodes;

import com.unknown6656.BinaryOpcode;

public final class Add extends BinaryOpcode
{
    public Add()
    {
        super((x, y) -> x + y);
    }
}
