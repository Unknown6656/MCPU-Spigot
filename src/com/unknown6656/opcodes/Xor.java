package com.unknown6656.opcodes;


public final class Xor extends BinaryOpcode
{
    public Xor()
    {
        super((x, y) -> x ^ y);
    }
}
