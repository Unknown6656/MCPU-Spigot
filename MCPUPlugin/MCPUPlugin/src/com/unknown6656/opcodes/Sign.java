package com.unknown6656.opcodes;

import com.unknown6656.UnaryOpcode;

public final class Sign extends UnaryOpcode
{
    public Sign()
    {
        super(x -> x == 0 ? 0 : x < 0 ? -1 : 1);
    }
}
