package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;
import epsilonpotato.mcpu.mcpuarch.MCPUSyscalls;


@MCPUOpcodeNumber(0x15)
public final class Syscall extends MCPUOpcode
{
    @Override
    public int MinimumArgumentCount()
    {
        return 1;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int num = arguments[1];
        MCPUSyscalls.Syscall call = MCPUSyscalls.syscalls.get(num);
        
        call.Execute(frame, proc, arguments);
    }
}
