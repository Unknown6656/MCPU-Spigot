package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Shl extends BinaryOpcode
{
    public Shl()
    {
        super((x, y) -> x << y);
    }
}
