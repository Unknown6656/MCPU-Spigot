package com.unknown6656.opcodes;

import com.unknown6656.UnaryOpcode;

public final class Neg extends UnaryOpcode
{
    public Neg()
    {
        super(x -> -x);
    }
}
