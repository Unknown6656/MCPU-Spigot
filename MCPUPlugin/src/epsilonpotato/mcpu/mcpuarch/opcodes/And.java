package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class And extends BinaryOpcode
{
    public And()
    {
        super((x, y) -> x & y);
    }
}
