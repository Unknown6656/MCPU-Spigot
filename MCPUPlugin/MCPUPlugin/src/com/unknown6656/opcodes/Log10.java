package com.unknown6656.opcodes;

import com.unknown6656.UnaryOpcode;

public final class Log10 extends UnaryOpcode
{
    public Log10()
    {
        super(x -> (int)Math.log10(x));
    }
}
