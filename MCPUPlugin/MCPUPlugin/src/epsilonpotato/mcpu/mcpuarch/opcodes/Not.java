package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

public final class Not extends UnaryOpcode
{
    public Not()
    {
        super(x -> ~x);
    }
}
