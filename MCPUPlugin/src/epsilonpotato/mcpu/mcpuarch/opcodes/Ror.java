package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

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
