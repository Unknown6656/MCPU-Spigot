package com.unknown6656.opcodes;

import com.unknown6656.UnaryOpcode;

public final class Decr extends UnaryOpcode
{
    public Decr()
    {
        super(x -> x + 1);
    }
}
