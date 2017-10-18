package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Mod extends BinaryOpcode
{
    public Mod()
    {
        super((x, y) -> x % y);
    }
}
