package com.unknown6656.opcodes;

import com.unknown6656.BinaryOpcode;

public final class Mod extends BinaryOpcode
{
    public Mod()
    {
        super((x, y) -> x % y);
    }
}
