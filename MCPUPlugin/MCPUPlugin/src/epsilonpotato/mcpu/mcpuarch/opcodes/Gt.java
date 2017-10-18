package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;

public final class Gt extends BinaryBooleanOpcode
{
    public Gt()
    {
        super((x, y) -> x > y);
    }
}
