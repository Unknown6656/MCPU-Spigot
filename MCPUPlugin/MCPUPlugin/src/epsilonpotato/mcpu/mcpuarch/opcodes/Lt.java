package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;

public final class Lt extends BinaryBooleanOpcode
{
    public Lt()
    {
        super((x, y) -> x < y);
    }
}
