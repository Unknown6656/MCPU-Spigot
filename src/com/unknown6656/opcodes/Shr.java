package com.unknown6656.opcodes;

public final class Shr extends BinaryOpcode
{
    public Shr()
    {
        super((x, y) -> x >>> y);
    }
}
