package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Pow extends BinaryOpcode
{
    public Pow()
    {
        super((x, y) -> (int)Math.pow(x, y));
    }
}
