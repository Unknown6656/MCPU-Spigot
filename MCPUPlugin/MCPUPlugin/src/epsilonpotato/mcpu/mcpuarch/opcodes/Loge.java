package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

public final class Loge extends UnaryOpcode
{
    public Loge()
    {
        super(x -> (int)Math.log(x));
    }
}
