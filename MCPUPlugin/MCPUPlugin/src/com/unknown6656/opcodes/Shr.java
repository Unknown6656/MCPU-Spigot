package com.unknown6656.opcodes;

import com.unknown6656.BinaryOpcode;

public final class Shr extends BinaryOpcode
{
    public Shr()
    {
        super((x, y) -> x >>> y);
    }
}
