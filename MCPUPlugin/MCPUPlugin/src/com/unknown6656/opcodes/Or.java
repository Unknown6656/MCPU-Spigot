package com.unknown6656.opcodes;


public final class Or extends BinaryOpcode
{
    public Or()
    {
        super((x, y) -> x | y);
    }
}
