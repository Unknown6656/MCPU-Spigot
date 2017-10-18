package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;

public final class Leq extends BinaryBooleanOpcode
{
    public Leq()
    {
        super((x, y) -> x <= y);
    }
}
