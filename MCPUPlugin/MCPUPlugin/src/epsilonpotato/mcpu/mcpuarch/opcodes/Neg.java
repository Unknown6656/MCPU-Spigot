package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

public final class Neg extends UnaryOpcode
{
    public Neg()
    {
        super(x -> -x);
    }
}
