package com.unknown6656.opcodes;

import com.unknown6656.UnaryOpcode;

public final class Abs extends UnaryOpcode
{
    public Abs()
    {
        super(x -> x > 0 ? x : -x);
    }
}
