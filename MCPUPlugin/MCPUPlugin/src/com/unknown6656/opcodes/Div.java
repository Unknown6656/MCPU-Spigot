package com.unknown6656.opcodes;

import com.unknown6656.BinaryOpcode;

public final class Div extends BinaryOpcode
{
    public Div()
    {
        super((x, y) -> x / y);
    }
}
