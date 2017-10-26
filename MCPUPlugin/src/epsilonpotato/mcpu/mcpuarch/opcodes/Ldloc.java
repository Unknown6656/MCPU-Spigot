package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


@MCPUOpcodeNumber(0x05)
public final class Ldloc extends MCPUOpcode
{
    @Override
    public int MinimumArgumentCount()
    {
        return 1;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int local = frame.Locals[arguments[0]];
        
        frame.Push(local);
    }
}
