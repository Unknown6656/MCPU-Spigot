package com.unknown6656.opcodes;

import com.unknown6656.BinaryOpcode;

public final class Sub extends BinaryOpcode
{
    public Sub()
    {
        super((x, y) -> x - y);
    }
}