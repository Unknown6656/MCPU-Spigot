package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x1a)
public final class Leq extends BinaryBooleanOpcode
{
    public Leq()
    {
        super((x, y) -> x <= y);
    }
}
