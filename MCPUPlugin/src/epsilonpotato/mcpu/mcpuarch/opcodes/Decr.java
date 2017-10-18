package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

public final class Decr extends UnaryOpcode
{
    public Decr()
    {
        super(x -> x + 1);
    }
}
