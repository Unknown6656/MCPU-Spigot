package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x1c)
public final class Geq extends BinaryBooleanOpcode
{
    public Geq()
    {
        super((x, y) -> x >= y);
    }
}
