package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

public final class Exp extends UnaryOpcode
{
    public Exp()
    {
        super(x -> (int)Math.pow(Math.E, x));
    }
}
