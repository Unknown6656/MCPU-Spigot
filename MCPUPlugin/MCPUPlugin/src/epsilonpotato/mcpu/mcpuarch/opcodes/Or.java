package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Or extends BinaryOpcode
{
    public Or()
    {
        super((x, y) -> x | y);
    }
}
