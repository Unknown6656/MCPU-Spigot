package com.unknown6656.opcodes;


public final class Mul extends BinaryOpcode
{
    public Mul()
    {
        super((x, y) -> x * y);
    }
}
