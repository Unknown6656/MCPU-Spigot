package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;

@MCPUOpcodeNumber(0x50)
public final class UUID extends MCPUOpcode
{
    @Override
    public int MinimumArgumentCount()
    {
        return 0;
    }
    
    @Override
    public int MinimumStackSize()
    {
        return 0;
    }

    @Override
    public void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int uuid = -1;
        
        try
        {
            uuid = proc.getComponentID();
        }
        catch (Exception e)
        {
        }
        finally
        {
            frame.Push(uuid);   
        }
    }
}
