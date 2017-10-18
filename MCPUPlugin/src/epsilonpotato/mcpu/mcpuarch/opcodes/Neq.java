package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;

public final class Neq extends BinaryBooleanOpcode
{
    public Neq()
    {
        super((x, y) -> x != y);
    }
}
