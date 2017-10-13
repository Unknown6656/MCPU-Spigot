package com.unknown6656.opcodes;

import com.unknown6656.UnaryOpcode;

public final class Loge extends UnaryOpcode
{
    public Loge()
    {
        super(x -> (int)Math.log(x));
    }
}
