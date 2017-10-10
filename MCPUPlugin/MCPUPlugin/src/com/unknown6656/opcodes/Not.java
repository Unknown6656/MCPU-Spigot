package com.unknown6656.opcodes;

import com.unknown6656.UnaryOpcode;

public final class Not extends UnaryOpcode
{
    public Not()
    {
        super(x -> ~x);
    }
}
