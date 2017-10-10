package com.unknown6656.opcodes;

import com.unknown6656.BinaryOpcode;

public final class Mul extends BinaryOpcode
{
    public Mul()
    {
        super((x, y) -> x * y);
    }
}
