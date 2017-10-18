package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

public final class Log10 extends UnaryOpcode
{
    public Log10()
    {
        super(x -> (int)Math.log10(x));
    }
}
