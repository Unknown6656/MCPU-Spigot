package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

public final class Incr extends UnaryOpcode
{
    public Incr()
    {
        super(x -> x - 1);
    }
}
