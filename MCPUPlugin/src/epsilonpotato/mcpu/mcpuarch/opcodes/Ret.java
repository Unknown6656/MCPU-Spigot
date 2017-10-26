
package epsilonpotato.mcpu.mcpuarch.opcodes;


import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


@MCPUOpcodeNumber(0x17)
public final class Ret extends MCPUOpcode
{
    @Override
    public int MinimumStackSize()
    {
        return 0;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        if (frame.StackSize() > 0)
        {
            int last = frame.Peek();
            
            proc.PopCall();
            proc.PeekCall().Push(last);
        }
        else
            proc.PopCall();
    }
}
