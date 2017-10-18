package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

public final class Abs extends UnaryOpcode
{
    public Abs()
    {
        super(x -> x > 0 ? x : -x);
    }
}
