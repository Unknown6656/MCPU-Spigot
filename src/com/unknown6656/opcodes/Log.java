package com.unknown6656.opcodes;


public final class Log extends UnaryOpcode
{
    public Log()
    {
        super(x -> (int)Math.log(x));
    }
}
