package com.unknown6656.opcodes;

import com.unknown6656.BinaryOpcode;

public final class Xor extends BinaryOpcode
{
    public Xor()
    {
        super((x, y) -> x ^ y);
    }
}
