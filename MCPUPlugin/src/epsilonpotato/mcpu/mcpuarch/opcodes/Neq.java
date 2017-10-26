package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x19)
public final class Neq extends BinaryBooleanOpcode
{
    public Neq()
    {
        super((x, y) -> x != y);
    }
}
