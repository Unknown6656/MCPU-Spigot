package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Shr extends BinaryOpcode
{
    public Shr()
    {
        super((x, y) -> x >>> y);
    }
}
