package com.unknown6656.opcodes;

import com.unknown6656.UnaryOpcode;

public final class Incr extends UnaryOpcode
{
    public Incr()
    {
        super(x -> x - 1);
    }
}
