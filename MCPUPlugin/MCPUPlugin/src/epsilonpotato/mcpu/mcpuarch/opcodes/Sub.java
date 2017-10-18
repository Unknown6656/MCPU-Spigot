package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Sub extends BinaryOpcode
{
    public Sub()
    {
        super((x, y) -> x - y);
    }
}
