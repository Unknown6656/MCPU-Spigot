package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

public final class Sign extends UnaryOpcode
{
    public Sign()
    {
        super(x -> x == 0 ? 0 : x < 0 ? -1 : 1);
    }
}
