package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Mul extends BinaryOpcode
{
    public Mul()
    {
        super((x, y) -> x * y);
    }
}
