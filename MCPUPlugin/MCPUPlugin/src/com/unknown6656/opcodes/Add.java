package com.unknown6656.opcodes;


public final class Add extends BinaryOpcode
{
    public Add()
    {
        super((x, y) -> x + y);
    }
}
