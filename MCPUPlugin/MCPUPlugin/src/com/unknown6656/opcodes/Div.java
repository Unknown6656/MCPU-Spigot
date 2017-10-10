package com.unknown6656.opcodes;


public final class Div extends BinaryOpcode
{
    public Div()
    {
        super((x, y) -> x / y);
    }
}
