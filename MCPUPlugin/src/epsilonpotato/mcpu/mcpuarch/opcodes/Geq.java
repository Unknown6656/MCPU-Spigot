package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;

public final class Geq extends BinaryBooleanOpcode
{
    public Geq()
    {
        super((x, y) -> x >= y);
    }
}
