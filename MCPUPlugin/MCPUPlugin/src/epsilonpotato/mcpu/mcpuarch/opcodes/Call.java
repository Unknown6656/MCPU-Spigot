package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


/**
 * Requires 2 args:
 * <pre>
 * call x y
 *      ^ ^-- argument count
 *      `---- function/label 
 * </pre>
 */
public final class Call extends MCPUOpcode
{
    @Override
    public int MinimumArgumentCount()
    {
        return 2;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        MCPUCallframe calee = new MCPUCallframe();
        
        calee.Arguments = new int[arguments[1]];
        
        proc.PushCall(calee);
        
        proc.InstructionPointer = arguments[0] -1;
    }
}
