package com.unknown6656.opcodes;


public final class Ror extends BinaryOpcode
{
    public Ror()
    {
        super((x, y) ->
        {
            y = ((y % 32) + 32) % 32;
            
            return (x >>> y) | (x << (31 - y));
        });
    }
}
