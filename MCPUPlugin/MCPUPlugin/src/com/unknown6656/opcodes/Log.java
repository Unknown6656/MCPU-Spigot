package com.unknown6656.opcodes;

import com.unknown6656.UnaryOpcode;

public final class Log extends UnaryOpcode
{
    public Log()
    {
        super(x -> (int)Math.log(x));
    }
}
