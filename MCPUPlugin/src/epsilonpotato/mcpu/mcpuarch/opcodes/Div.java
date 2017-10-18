package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Div extends BinaryOpcode
{
    public Div()
    {
        super((x, y) -> x / y);
    }
}
