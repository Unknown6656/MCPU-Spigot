package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


/**
 * Requires 2 args:
 * 
 * <pre>
 * call x y
 *      ^ ^-- argument count
 *      `---- function/label 
 * first callee argument: bottom of stack
 * last callee argument: top of stack
 * </pre>
 */
@MCPUOpcodeNumber(0x16)
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
        MCPUCallframe callee = new MCPUCallframe();
        int count = arguments[1];
        
        callee.Arguments = new int[count];
        
        for (int i = 0; i < count; ++i)
            callee.Arguments[count - 1 - i] = frame.Pop();
        
        proc.PushCall(callee);
        proc.InstructionPointer = arguments[0] - 1;
    }
}
