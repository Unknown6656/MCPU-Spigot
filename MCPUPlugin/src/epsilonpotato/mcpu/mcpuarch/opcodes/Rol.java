package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Rol extends BinaryOpcode
{
    public Rol()
    {
        super((x, y) ->
        {
            y = ((y % 32) + 32) % 32;
            
            return (x << y) | (x >>> (31 - y));
        });
    }
}