package com.unknown6656.opcodes;

import com.unknown6656.BinaryOpcode;

public final class Pow extends BinaryOpcode
{
    public Pow()
    {
        super((x, y) -> (int)Math.pow(x, y));
    }
}
