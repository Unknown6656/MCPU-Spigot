package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Xor extends BinaryOpcode
{
    public Xor()
    {
        super((x, y) -> x ^ y);
    }
}
