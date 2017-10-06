package com.unknown6656.opcodes;


public final class Sub extends BinaryOpcode
{
    public Sub()
    {
        super((x, y) -> x - y);
    }
}
