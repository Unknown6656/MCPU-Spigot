package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x34)
public final class Ror extends BinaryOpcode
{
    public Ror()
    {
        super((x, y) ->
        {
            y = ((y % 32) + 32) % 32;
            
            return (x >>> y) | (x << (31 - y));
        });
    }
}
