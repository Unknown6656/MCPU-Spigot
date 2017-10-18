package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Add extends BinaryOpcode
{
    public Add()
    {
        super((x, y) -> x + y);
    }
}
