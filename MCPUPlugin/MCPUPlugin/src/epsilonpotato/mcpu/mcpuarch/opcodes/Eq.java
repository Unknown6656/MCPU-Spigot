package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;

public final class Eq extends BinaryBooleanOpcode
{
    public Eq()
    {
        super((x, y) -> x == y);
    }
}
