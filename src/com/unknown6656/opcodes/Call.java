package com.unknown6656.opcodes;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


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
